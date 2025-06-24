using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class
SoundObject : MonoBehaviour
{
    private AudioSource Source;
    private bool Started;

    private void
    Awake()
    {
        Source = this.GetComponent< AudioSource >();
        //TODO: Setup all of the Sound Objects stuff here
    }

    private void
    Update()
    {
        if( !Source.isPlaying && Started )
        {
            Source.Stop();
            Destroy( this.gameObject );
            Debug.Log("Sound Object has Finished!!!");
        }
    }

    public void
    StartSoundObject( AudioClip Clip )
    {
        Source.clip = Clip;
        Source.Play();
        Started = true;
    }
}
