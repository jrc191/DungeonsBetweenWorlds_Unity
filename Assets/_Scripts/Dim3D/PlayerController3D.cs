using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController3D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    [Header("Estados")]
    public bool canJump = true;
    private bool isMoving = false;
    private bool groundedPlayer;

    [Header("Referencias")]
    public CharacterController controller;

    private InputSystem_Actions inputActions;
    private Vector3 playerVelocity; // Aquí guardamos la fuerza vertical (gravedad/salto)
    private Vector3 targetPosition;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        if (controller == null) controller = GetComponent<CharacterController>();

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Si la distancia es casi cero, hemos llegado
            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
            return; // Terminamos por este frame para no leer input a la vez
        }
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        // 1. Verificamos suelo
        groundedPlayer = controller.isGrounded;

        // Reseteo de gravedad
        if (groundedPlayer && playerVelocity.y < 0) playerVelocity.y = -2.0f;

        // 2. Leemos el Input
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        // --- MODO 3D (Movimiento Libre) ---
        if (canJump)
        {
            Vector3 move = new Vector3(input.x, 0, input.y);
            controller.Move(move * Time.deltaTime * moveSpeed);

            // Salto y Gravedad (Solo en 3D)
            if (inputActions.Player.Jump.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        // --- MODO 2D (Movimiento Grid) ---
        else
        {
            // A. Si ya nos estamos moviendo, seguimos hasta llegar
            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (transform.position == targetPosition)
                {
                    isMoving = false;
                }
            }
            // B. Si estamos quietos, leemos input para dar el siguiente paso
            else
            {
                // Usamos Round para asegurar que el movimiento sea de 1 en 1 (Grid perfecto)
                // Y evitamos movernos si el input es muy pequeño (zona muerta)
                if (Mathf.Abs(input.x) > 0.5f || Mathf.Abs(input.y) > 0.5f)
                {
                    // Priorizamos ejes: si te mueves en diagonal, elegimos el eje más fuerte
                    // para evitar saltos en diagonal extraños.
                    if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                        input.y = 0;
                    else
                        input.x = 0;

                    Vector3 direction = new Vector3(Mathf.Round(input.x), 0, Mathf.Round(input.y));
                    // NUEVO: Mirar antes de pisar. Si hay algo, no moverse.
                    // Lanzamos un rayo desde mi posición, hacia la dirección deseada, de 1 metro de largo
                    if (!Physics.Raycast(transform.position, direction, 1.0f))
                    {
                        targetPosition = transform.position + direction;
                        isMoving = true;
                    }
                }
            }
        }
    }
}