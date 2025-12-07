using UnityEngine;
using UnityEngine.InputSystem;

public class DimensionalCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public Camera cam;

    [Header("Configuración 3D")]
    public Vector3 offset3D = new Vector3(0, 5, -8);
    public float fov3D = 60f;

    [Header("Configuración 2D")]
    public Vector3 offset2D = new Vector3(0, 10, 0);
    public float orthoSize = 5f;

    private bool is2D = false;
    private InputSystem_Actions inputActions; // Referencia al sistema de inputs
    private PlayerController3D playerScript;



    void Awake()
    {
        inputActions = new InputSystem_Actions();
        playerScript = target.GetComponent<PlayerController3D>();
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
        // Seguir al objetivo
        transform.position = target.position + (is2D ? offset2D : offset3D);
        transform.LookAt(target);

        // FIX: NO permitir cambiar de dimensión si no estamos en el suelo
        if (inputActions.Player.DimensionSwitch.triggered && playerScript.controller.isGrounded)
        {
            ToggleDimension();
        }
    }

    void ToggleDimension()
    {
        is2D = !is2D;
        if (is2D) SwitchTo2D();
        else SwitchTo3D();
    }

    void SwitchTo3D()
    {
        cam.orthographic = false;
        cam.fieldOfView = fov3D;

        // Reactivamos el salto en 3D
        if (playerScript != null) playerScript.canJump = true;
    }

    void SwitchTo2D()
    {
        cam.orthographic = true;
        cam.orthographicSize = orthoSize;
        transform.rotation = Quaternion.Euler(90, 0, 0);

        // Prohibimos el salto en 2D
        if (playerScript != null) playerScript.canJump = false;

    }
}