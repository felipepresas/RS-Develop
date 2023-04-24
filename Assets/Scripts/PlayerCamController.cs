using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float smoothTime = 0.8f;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [SerializeField]
    private float heightOffset = 2f;
    [SerializeField]
    private float distanceOffset = 8f;
    public bool isDead ;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target != null) // Verifica si el objeto target es nulo antes de acceder a él
        {
            Vector3 targetPosition = target.position;
            Vector3 cameraOffset = target.position - transform.position;

            // Calcular la rotación que debe tener la cámara
            Quaternion targetRotation = Quaternion.LookRotation(cameraOffset);

            // Aplicar rotación específica en el eje X
            targetRotation *= Quaternion.Euler(0, 0, 0);

            // Rotar la cámara hacia la posición deseada
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Calcular la posición deseada de la cámara
            Vector3 desiredPosition = targetPosition - (cameraOffset.normalized * distanceOffset) + Vector3.up * heightOffset;

            // Suavizar la transición entre la posición actual de la cámara y la posición deseada
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
}
