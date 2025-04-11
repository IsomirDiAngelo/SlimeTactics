using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private GameObject player;

    void LateUpdate() {
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = 0; // Fix the camera in Y to reduce camera shaking
        transform.position = playerPosition + cameraOffset;
    }
}
