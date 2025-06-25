using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct
RadialButton
{
    public string Name;
    public Button Button;
    public Image  IconRenderer;
}

public class
RadialMenu : MonoBehaviour
{
    private RectTransform Menu;
    private Transform CameraTransform;
    public  RectTransform CanvasTransform;

    [SerializeField]
    private float ScaleFactor, MinScale, MaxScaleDistance;
    public TMP_Text Label;
    [HideInInspector]
    public Vector3 PivotPoint;
    public RadialButton[] Buttons;

    private void
    Awake()
    {
        Menu = this.GetComponent<RectTransform>();
        Initialize();
    }

    private void
    Initialize()
    {
        CameraTransform = Camera.main.transform;
        for( int i = 0; i < Buttons.Length; ++i )
        {
            Buttons[ i ].IconRenderer.gameObject.SetActive( false );
        }
    }

    private void
    Start()
    {
        for( int i = 0; i < Buttons.Length; ++i )
        {
            if( Buttons[ i ].Name == "")
            {
                Buttons[i].Button.interactable = false;
            }
        }
    }

    public void
    SetRadialButtonIcon( int ButtonNum, Sprite Icon )
    {
        if( ButtonNum > 3 && ButtonNum < 0 )
        {
            Debug.LogError("ERROR: TRIED TO ACCESS ICON RADIAL BUTTON THAT DOESN'T EXIST!!!");
            return;
        }

        Buttons[ ButtonNum ].IconRenderer.sprite = Icon;
        Buttons[ ButtonNum ].IconRenderer.gameObject.SetActive( true );
    }

    public void
    ClearRadial()
    {
        Destroy( this.gameObject );
    }

    private void
    Update()
    {
        Vector3 Direction = PivotPoint - CameraTransform.position;
        float DistanceScale = 1 - ( Direction.magnitude / MaxScaleDistance );
        DistanceScale *= ScaleFactor;
        DistanceScale = Mathf.Clamp( DistanceScale, MinScale, 1 );
        Menu.localScale = new Vector3 ( DistanceScale, DistanceScale, DistanceScale );

        Vector3 ViewportPoint = Camera.main.WorldToViewportPoint( PivotPoint );
        Vector3 CanvasPoint = new Vector3(
                ( ( ViewportPoint.x * CanvasTransform.sizeDelta.x ) - (CanvasTransform.sizeDelta.x * 0.5f ) ),
                ( ( ViewportPoint.y * CanvasTransform.sizeDelta.y ) - (CanvasTransform.sizeDelta.y * 0.5f ) ),
                0
                );

        Menu.anchoredPosition = CanvasPoint;
    }

    public void
    SetLabelText( int ButtonIndex )
    {
        if( ButtonIndex < 0 || ButtonIndex > Buttons.Length )
        {
            Debug.LogError("ERROR: TRYING TO ACCESS RADIAL BUTTON BEYOND THE BOUNDS");
            return;
        }

        Label.gameObject.SetActive( true );
        if( Buttons[ButtonIndex].Button.interactable )
        {
            Label.text = Buttons[ButtonIndex].Name;
        }
        else
        {
            ClearLabelText();
        }
    }

    public void
    ClearLabelText()
    {
        Label.gameObject.SetActive( false );
        Label.text = "";
    }

    public void
    SetRadialButtonName( int ButtonIndex, string Name )
    {
        if( ButtonIndex < 0 || ButtonIndex > Buttons.Length )
        {
            Debug.LogError("ERROR: TRYING TO ACCESS RADIAL BUTTON BEYOND THE BOUNDS");
            return;
        }
        Buttons[ ButtonIndex ].Name = Name;
    }
}
