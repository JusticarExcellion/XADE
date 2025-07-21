using UnityEngine;
using UnityEngine.UI;
using DrawXXL;

public class DrawHelper : MonoBehaviour
{
    public static DrawHelper Instance;

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
    }

    public void
    DrawLineStartEnd( Vector3 Start, Vector3 End, float MovementSpeed, Color color )
    {
        Vector3 Direction = End - Start;
        if( Direction.magnitude > MovementSpeed )
        {
            Direction.Normalize();
            Direction *= MovementSpeed;
            End = Start + Direction;
        }
        Vector3 Midpoint = ( Start + (Direction * 0.5f) );
        float Distance = Direction.magnitude;
        string Text = $"Distance: {Distance}";
        DrawText.WriteScreenspaceFramed( Text,  Midpoint );
        DrawBasics.Vector( Start, End, color );
    }

    public void
    Measure( Vector3 Point1, Vector3 Point2 )
    {
        Vector3 Direction = Point1 - Point2;
        Vector3 Midpoint = (Point2 + (Direction * 0.5f) );
        string Text = $"Distance: {Direction.magnitude}";
        DrawText.WriteScreenspaceFramed( Text,  Midpoint );
        DrawBasics.Vector( Point1, Point2, Color.green );
    }
}
