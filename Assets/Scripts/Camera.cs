using UnityEngine;
using Unity.Cinemachine;

public class
CameraMovement : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera MainCamera;
    [SerializeField]
    private Transform LookAtPoint;
    [Header("Camera Movement Properties:")]
    [SerializeField]
    private float CameraMoveSpeed;
    [SerializeField]
    private float CameraZoomSpeed;
    [SerializeField]
    private float MaxHeight;
    [SerializeField]
    private float MinHeight;
    [SerializeField]
    private float OffsetWeight;
    [SerializeField]
    private float CameraRotationSpeed;
    [SerializeField]
    private float CameraRotationAmount;

    private Transform CameraTransform;

    private void
    Awake()
    {
        CameraTransform = MainCamera.transform;
    }

    private void
    Update()
    {
        Vector2 ScrollDelta = Input.mouseScrollDelta;
        Vector3 CurrentPosition = LookAtPoint.position;
        Vector3 OffsetPosition = CameraTransform.position;
        float OffsetZ = ( ( ( CameraTransform.position.y - MinHeight ) / ( MaxHeight - MinHeight ) ) * OffsetWeight ) - OffsetWeight;
        OffsetPosition = CurrentPosition + ( LookAtPoint.forward * OffsetZ );
        OffsetPosition.y = CameraTransform.position.y + ScrollDelta.y * CameraZoomSpeed;
        OffsetPosition.y = Mathf.Clamp( OffsetPosition.y, MinHeight, MaxHeight );
        CameraTransform.position = OffsetPosition;

        if( Input.GetMouseButton( 2 ) )
        {
            Vector3 MouseDelta = Input.mousePositionDelta;
            Vector3 LookAtPosition = LookAtPoint.position;
            Vector3 CameraVector = ( LookAtPoint.forward * -MouseDelta.y );
            CameraVector += ( LookAtPoint.right * -MouseDelta.x );
            CameraVector.y = 0;
            CameraVector *= CameraMoveSpeed;
            LookAtPosition += CameraVector;
            LookAtPoint.position = LookAtPosition;
        }

        if( Input.GetKey( KeyCode.Q ) )
        {
            LookAtPoint.rotation = Quaternion.Euler( LookAtPoint.rotation.eulerAngles.x, LookAtPoint.rotation.eulerAngles.y - CameraRotationAmount,LookAtPoint.rotation.eulerAngles.z );
        }

        if( Input.GetKey( KeyCode.E ) )
        {
            LookAtPoint.rotation = Quaternion.Euler( LookAtPoint.rotation.eulerAngles.x, LookAtPoint.rotation.eulerAngles.y + CameraRotationAmount, LookAtPoint.rotation.eulerAngles.z );
        }

        Vector3 LookVector = LookAtPoint.position - OffsetPosition;
        Quaternion NewRot = Quaternion.LookRotation( LookVector );
        NewRot = Quaternion.Euler( NewRot.eulerAngles.x, CameraTransform.rotation.eulerAngles.y , 0 );

        CameraTransform.rotation = Quaternion.Slerp( CameraTransform.rotation, NewRot, Time.deltaTime * CameraRotationSpeed );
    }
}
