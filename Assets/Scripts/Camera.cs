using UnityEngine;
using Unity.Cinemachine;

public class
CameraMovement : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera MainCamera;
    [SerializeField]
    private float CameraMoveSpeed;

    private void
    Update()
    {
        if( Input.GetMouseButton( 2 ) )
        {
            Vector3 MouseDelta = Input.mousePositionDelta;
            Vector3 CameraVector = new Vector3( MouseDelta.x, 0, MouseDelta.y );
            CameraVector *= CameraMoveSpeed;

            MainCamera.transform.position += CameraVector;
        }
    }
}
