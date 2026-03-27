using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float CameraSpeed = 5f;
    public float CameraScrollingSpeed = 5f;

    void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;
        if ( Input.GetKey ( KeyCode.UpArrow ) ) direction += Vector3.up;
        if ( Input.GetKey ( KeyCode.LeftArrow ) ) direction += Vector3.left;
        if ( Input.GetKey ( KeyCode.RightArrow ) ) direction += Vector3.right;
        if ( Input.GetKey ( KeyCode.DownArrow ) ) direction += Vector3.down;
        direction = direction.normalized;

        if (direction != Vector3.zero )
        {
            gameObject.transform.position += direction * CameraSpeed;
        }

        float cameraScroll = Input.GetAxis ( "Mouse ScrollWheel" );
        Camera.main.orthographicSize += cameraScroll * CameraScrollingSpeed;
    }
}
