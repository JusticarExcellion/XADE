using UnityEngine;
using UnityEngine.UI;

public class DrawHelper : MonoBehaviour
{
    public static DrawHelper Instance;
    private LineRenderer LR;
    private Material Line_MAT;

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
        Line_MAT = null;
        if( LR == null )
        {
            Debug.LogError("ERROR: LINE RENDERER NOT FOUND");
            return;
        }
        Line_MAT = LR.material;
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

    public bool
    DrawingLine()
    {
        return LR.enabled;
    }

    public void
    SetLineColor( Color color )
    {
        Line_MAT.color = color;
    }

    public void
    Measure( Vector3 Point1, Vector3 Point2 )
    {
        LR.enabled = true;
        Vector3 Direction = Point2 - Point1;
        float Distance = Direction.magnitude;
        Vector3 LineMidpoint = Vector3.Lerp( Point1, Point2, 0.5f );
        LR.SetPosition( 0, Point1 );
        LR.SetPosition( 1, Point2 );
        UIManager.Manager.DisplayLineDistance( LineMidpoint, Distance );
    }
}
