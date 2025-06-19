using UnityEngine;
using Unity.Cinemachine;

public class
CameraMovement : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera MainCamera;
    [Header("Camera Movement Properties:")]
    [SerializeField]
    private float CameraMoveSpeed;
    [SerializeField]
    private float CameraZoomSpeed;
    [SerializeField]
    private float MaxHeight;
    [SerializeField]
    private float MinHeight;

    private void
    Update()
    {
        Vector2 ScrollDelta = Input.mouseScrollDelta;
        Vector3 CameraVector = new Vector3(0,ScrollDelta.y,0);
        Vector3 CurrentPosition = MainCamera.transform.position;
        CameraVector *= CameraZoomSpeed;
        CurrentPosition += CameraVector;
        CurrentPosition.y = Mathf.Clamp( CurrentPosition.y, MinHeight, MaxHeight );
        MainCamera.transform.position = CurrentPosition;

        if( Input.GetMouseButton( 2 ) )
        {
            Vector3 MouseDelta = Input.mousePositionDelta;
            CameraVector = new Vector3( MouseDelta.x, 0 ,MouseDelta.y );
            CameraVector *= CameraMoveSpeed;
            CurrentPosition += CameraVector;
            MainCamera.transform.position = CurrentPosition;
        }
    }
}
