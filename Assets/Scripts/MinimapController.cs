using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform player;
    public bool rotateWithPlayer = true;
    public Vector3 cameraOffset = new Vector3(0, 70, 4);

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + cameraOffset;
            if (rotateWithPlayer)
            {
                transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}
