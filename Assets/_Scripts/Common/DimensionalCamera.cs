using UnityEngine;
using UnityEngine.InputSystem;
public class DimensionalCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // El jugador (PlayerDummy)
    public Camera cam;       // La cámara misma

    [Header("Configuración 3D (Perspectiva)")]
    public Vector3 offset3D = new Vector3(0, 5, -8);
    public float fov3D = 60f;

    [Header("Configuración 2D (Ortográfica)")]
    public Vector3 offset2D = new Vector3(0, 10, 0); // Justo encima
    public float orthoSize = 5f; // "Zoom" en modo 2D

    private bool is2D = false; // Estado actual

    void Start()
    {
        // Aseguramos que empezamos en 3D
        SwitchTo3D();
    }

    void Update()
    {
        // Seguir al objetivo
        transform.position = target.position + (is2D ? offset2D : offset3D);
        transform.LookAt(target);

        // Nuevo Input System: Verificamos si hay un teclado conectado antes de leerlo
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
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
        // 1. Cambiar el tipo de proyección a Perspectiva
        cam.orthographic = false;

        // 2. Ajustar el campo de visión (FOV)
        cam.fieldOfView = fov3D;

        // 3. (Opcional) Rotación manual si LookAt no es suficiente
    }

    void SwitchTo2D()
    {
        // 1. Cambiar el tipo de proyección a Ortográfica
        cam.orthographic = true;

        // 2. Ajustar el tamaño ortográfico (Zoom)
        cam.orthographicSize = orthoSize;

        // 3. Forzar rotación cenital perfecta (mirando hacia abajo)
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
