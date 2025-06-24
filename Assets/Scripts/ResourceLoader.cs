using UnityEngine;

//TODO: We should use this to load models and set them up after we create them,
//and eventually we should use this to use as a resource browser, to place
//obstacles or other things
public class
ResourceLoader : MonoBehaviour
{
    public static ResourceLoader Instance;

    public void
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

    public bool
    Initialize()
    {
        //TODO: load resources here
        bool Valid = true;
        return Valid;
    }

    public bool
    LoadModel( GameObject Object, string ModelName )
    {
        bool Valid = true;

        return Valid;
    }
}
