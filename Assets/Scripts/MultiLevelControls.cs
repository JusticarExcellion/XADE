using UnityEngine;


public class MultiLevelControls : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Levels;

    private void
    Awake()
    {
    }

    private void
    Start()
    {
    }

    private void
    Update()
    {
        if( Input.GetKeyDown( KeyCode.F1 ) )
        {
            DeactivateLevel( 0 );
        }
        if( Input.GetKeyDown( KeyCode.F2 ) )
        {
            DeactivateLevel( 1 );
        }
        if( Input.GetKeyDown( KeyCode.F3 ) )
        {
            DeactivateLevel( 2 );
        }
        if( Input.GetKeyDown( KeyCode.F4 ) )
        {
            DeactivateLevel( 3 );
        }
        if( Input.GetKeyDown( KeyCode.F5 ) )
        {
            DeactivateLevel( 4 );
        }
        if( Input.GetKeyDown( KeyCode.F6 ) )
        {
            DeactivateLevel( 5 );
        }
        if( Input.GetKeyDown( KeyCode.F7 ) )
        {
            DeactivateLevel( 6 );
        }
        if( Input.GetKeyDown( KeyCode.F8 ) )
        {
            DeactivateLevel( 7 );
        }
        if( Input.GetKeyDown( KeyCode.F9 ) )
        {
            DeactivateLevel( 8 );
        }
        if( Input.GetKeyDown( KeyCode.F10 ) )
        {
            DeactivateLevel( 9 );
        }
        if( Input.GetKeyDown( KeyCode.F11 ) )
        {
            DeactivateLevel( 10 );
        }
        if( Input.GetKeyDown( KeyCode.F12 ) )
        {
            DeactivateLevel( 11 );
        }
    }

    private void
    DeactivateLevel( int level )
    {
        if( level < Levels.Length )
        {
            Levels[ level ].SetActive( !Levels[ level ].activeSelf );
            Debug.Log("Toggled Level: " + level );
        }
    }
}
