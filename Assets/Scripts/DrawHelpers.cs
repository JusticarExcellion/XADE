using UnityEngine;
using UnityEngine.UI;

public class DrawHelper : MonoBehaviour
{
    public static DrawHelper Instance;
    private LineRenderer LR;

    private void
    Awake()
    {
        if( Instance != null && Instance != this )
        {
            Destroy( this );
            return;
        }
        Instance = this;
        DontDestroyOnLoad( this );
        Initialize();
    }

    private void
    Initialize()
    {
        LR = this.GetComponent<LineRenderer>();
        if( LR == null )
        {
            Debug.LogError("ERROR: LINE RENDERER NOT FOUND");
            return;
        }
        LR.enabled = false;
    }

    private void
    Start()
    {
    }

    private void
    Update()
    {
    }

    public void
    DrawLineStartEnd( Vector3 Start, Vector3 End, float MovementSpeed )
    {
        LR.enabled = true;
        Vector3 Direction = End - Start;

        if( Direction.magnitude > MovementSpeed )
        {
            Direction.Normalize();
            Direction *= MovementSpeed;
            End = Start + Direction;
        }
        float Distance = Direction.magnitude;
        Vector3 LineMidpoint = Vector3.Lerp( Start, End, 0.5f );
        if( Distance > .01f )
        {
            LR.SetPosition( 0, Start );
            LR.SetPosition( 1, End   );
            UIManager.Manager.DisplayLineDistance( LineMidpoint, Distance );
        }

    }

    public void
    DisableLine()
    {
        LR.enabled = false;
        UIManager.Manager.DisableLineDistance();
    }

    public void
    SetLineColor( Color color )
    {
        LR.startColor = color;
        LR.endColor = color;
    }

}
