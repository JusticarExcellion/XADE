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
    private float MaxCamHeight;
    [SerializeField]
    private float MinCamHeight;
    [SerializeField]
    private float OffsetWeight;
    [SerializeField]
    private float CameraRotationSpeed;
    [SerializeField]
    private float CameraRotationAmount;

    private int CurrentLevel;

    [SerializeField]
    private Transform CurrentLevelTransform;
    private Transform CameraTransform;
    private MultiLevelControls MLC;

    private void
    Awake()
    {
        CameraTransform = MainCamera.transform;
        MLC = FindFirstObjectByType<MultiLevelControls>();
        if( MLC == null )
        {
            Debug.LogError("ERROR: NO MULTI LEVEL CONTROL FOUND!!!");
        }
        else
        {
            CurrentLevelTransform = MLC.Levels[0].transform;
        }
        CurrentLevel = 0;
    }

    private void
    Update()
    {
        Vector2 ScrollDelta = Input.mouseScrollDelta;
        Vector3 CurrentPosition = LookAtPoint.position;
        Vector3 LookAtPosition = LookAtPoint.position;
        Vector3 CameraPosition = CameraTransform.position;
        Vector3 OffsetPosition = CameraPosition;
        float MinLevelCameraHeight = MinCamHeight + LookAtPosition.y;
        float MaxLevelCameraHeight = MaxCamHeight + LookAtPosition.y;

        float CurrentOffsetFactor = ( CameraPosition.y -  MinLevelCameraHeight ) / ( MaxLevelCameraHeight - MinLevelCameraHeight );
        float OffsetZ = ( CurrentOffsetFactor * OffsetWeight ) - OffsetWeight;
        OffsetPosition = CurrentPosition + ( LookAtPoint.forward * OffsetZ );
        OffsetPosition.y = CameraPosition.y + ScrollDelta.y * CameraZoomSpeed;


        OffsetPosition.y = Mathf.Clamp( OffsetPosition.y, MinLevelCameraHeight, MaxLevelCameraHeight );

        CameraTransform.position = OffsetPosition;

        Vector3 CameraVector = new Vector3();
        if( Input.GetMouseButton( 2 ) )
        {
            Vector3 MouseDelta = Input.mousePositionDelta;
            CameraVector += ( LookAtPoint.forward * -MouseDelta.y );
            CameraVector += ( LookAtPoint.right * -MouseDelta.x );
            CameraVector.y = CurrentLevelTransform.position.y;
        }

        if( !UIManager.Manager.TextModeActive() )
        {

            if( Input.GetKey( KeyCode.W ) )
            {
                CameraVector += LookAtPoint.forward;
            }

            if( Input.GetKey( KeyCode.A ) )
            {
                CameraVector += -LookAtPoint.right;
            }

            if( Input.GetKey( KeyCode.S ) )
            {
                CameraVector += -LookAtPoint.forward;
            }

            if( Input.GetKey( KeyCode.D ) )
            {
                CameraVector += LookAtPoint.right;
            }

            if( Input.GetKeyDown( KeyCode.Z ) )
            {//Move Up
                CurrentLevel++;
                if( CurrentLevel >= MLC.Levels.Length )
                {
                    CurrentLevel = MLC.Levels.Length - 1;
                }
                else
                {
                    CurrentLevelTransform = MLC.Levels[CurrentLevel].transform;
                }
            }

            if( Input.GetKeyDown( KeyCode.X ) )
            {//Move Down
                CurrentLevel--;
                if( CurrentLevel > -1 )
                {
                    CurrentLevelTransform = MLC.Levels[CurrentLevel].transform;
                }
                else
                {
                    CurrentLevel = 0;
                }
            }

        }

        float CurrentSpeed = (2 * CameraMoveSpeed) * CurrentOffsetFactor;
        CurrentSpeed = Mathf.Clamp( CurrentSpeed, CameraMoveSpeed, ( 2 * CameraMoveSpeed ) );
        CameraVector *= CurrentSpeed;
        LookAtPosition += CameraVector;
        LookAtPosition.y = Mathf.Lerp( LookAtPosition.y, CurrentLevelTransform.position.y, Time.deltaTime );
        LookAtPoint.position = LookAtPosition;

        Quaternion LookAtRotation = LookAtPoint.rotation;
        if( Input.GetKey( KeyCode.Q ) )
        {
            LookAtPoint.rotation = Quaternion.Euler( LookAtRotation.eulerAngles.x, LookAtRotation.eulerAngles.y - CameraRotationAmount,LookAtRotation.eulerAngles.z );
        }

        if( Input.GetKey( KeyCode.E ) )
        {
            LookAtPoint.rotation = Quaternion.Euler( LookAtRotation.eulerAngles.x, LookAtRotation.eulerAngles.y + CameraRotationAmount, LookAtRotation.eulerAngles.z );
        }

        Vector3 LookVector = LookAtPoint.position - OffsetPosition;
        Quaternion NewRot = Quaternion.LookRotation( LookVector );
        NewRot = Quaternion.Euler( NewRot.eulerAngles.x, CameraTransform.rotation.eulerAngles.y , 0 );

        CameraTransform.rotation = Quaternion.Slerp( CameraTransform.rotation, NewRot, Time.deltaTime * CameraRotationSpeed );
    }
}
