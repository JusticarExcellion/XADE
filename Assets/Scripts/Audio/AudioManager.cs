using UnityEngine;
using UnityEngine.Audio;

public class
AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource MusicSource;
    private AudioSource SoundSource;

    [SerializeField]
    private AudioMixerGroup Mixers;
    //TODO: We want to define a Music Folder and then load all the music in
    //there

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
        Initialize();
    }


    public bool
    Initialize()
    {
        bool Valid = true;
        MusicSource = this.gameObject.AddComponent<AudioSource>();
        SoundSource = this.gameObject.AddComponent<AudioSource>();
        return Valid;
    }

    public void
    PlayMusic()
    {
    }

    public void
    PlaySound()
    {
    }

    public void
    PlayAudio()
    {
    }

    public void
    SetMusicLoop( bool Looping )
    {
        MusicSource.loop = Looping;
    }

    public void
    SetMusicPlay( bool Playing )
    {
        if ( Playing )
        {
            MusicSource.Play(); 
        }
        else
        {
            MusicSource.Pause();
        }
    }

    public void
    NextSong()
    {
    }

    public void
    PrevSong()
    {
    }

    public void
    CreateSoundObject( Vector3 Position, AudioClip Sound )
    {
        GameObject go = new GameObject();
        go.transform.position = Position;
        SoundObject SO = go.AddComponent< SoundObject >();
        SO.StartSoundObject( Sound );
    }

}
