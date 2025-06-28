using UnityEngine;

public class CameraSync : MonoBehaviour
{
    private Camera MainCam;
    private Camera MyCam;
    private Transform MainCamTransform;
    private Transform MyTransform;

    private void
    Awake()
    {
        MyCam = this.GetComponent<Camera>();
        MainCam = Camera.main;
        MainCamTransform = MyCam.transform;
        MyTransform = this.transform;
    }

    private void
    Update()
    {
        MyCam.fieldOfView = MainCam.fieldOfView;
        MyTransform.position = MainCamTransform.position;
        MyTransform.rotation = MainCamTransform.rotation;
    }
}
