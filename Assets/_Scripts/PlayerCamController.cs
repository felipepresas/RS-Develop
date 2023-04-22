using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
  public Transform target;
    public float smoothTime = 0.8f;
    public float rotationSpeed = 5.0f;

    private Vector3 velocity = Vector3.zero;
    private float mouseX, mouseY;

    void LateUpdate()
    {
        Vector3 targetPosition = target.position;
        Vector3 cameraOffset = target.position - transform.position;

        // Calcular la rotación que debe tener la cámara
        Quaternion targetRotation = Quaternion.LookRotation(cameraOffset);

        // Aplicar rotación específica en el eje X
        targetRotation *= Quaternion.Euler(5, 0, 0);

        // Rotar la cámara hacia la posición deseada
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Calcular la posición deseada de la cámara
        Vector3 desiredPosition = targetPosition - (cameraOffset.normalized * 8) + Vector3.up * 2;

        // Suavizar la transición entre la posición actual de la cámara y la posición deseada
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
