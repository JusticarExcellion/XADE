using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

//NOTE: I HATE THIS SHIT
public class
AudioFileDetails : ScriptableObject
{
    public string Path;
    public string Name;
}

//TODO: Change this to an audio file manager and have the Getting sets of
//audiofiledetails be handled by a seperate audiofileviewmanager
public class
AudioFile
{
    private int CurrentIndex = 0;
    private const int SetNumber = 5;
    public List<AudioFileDetails> Files;

    public AudioClip
    LoadClip( bool MusicResource, int i )
    {
        AudioClip clip = null;
        if( MusicResource )
        {
            Debug.Log("Loading Clip at: " + ( AudioManager.ResourceMusicPath + Files[ CurrentIndex + i ].Name) );
            clip = Resources.Load<AudioClip>( AudioManager.ResourceMusicPath + Files[ CurrentIndex + i ].Name );
        }
        else
        {
            Debug.Log("Loading Clip at: " + ( AudioManager.ResourceAmbiencePath + Files[ CurrentIndex + i ].Name) );
            clip = Resources.Load<AudioClip>( AudioManager.ResourceAmbiencePath + Files[ CurrentIndex + i ].Name );
        }

        if( !clip ) Debug.LogError($"FAILED TO FIND {Files[ CurrentIndex + i ].Path}!!!");
        return clip;
    }

    public List<AudioFileDetails>
    GrabNextSet()
    {
        CurrentIndex = ( (CurrentIndex + 5 ) < ( Files.Count - SetNumber ) ) ? (CurrentIndex + 5) : ( Files.Count - SetNumber );
        return GrabSet( CurrentIndex );
    }

    public List<AudioFileDetails>
    GrabPrevSet()
    {
        CurrentIndex = ( (CurrentIndex - 5 ) > 0 ) ? (CurrentIndex - 5) : 0;
        return GrabSet( CurrentIndex );
    }

    public List<AudioFileDetails>
    GrabSet( int StartIndex )
    {
        List<AudioFileDetails> FileSet = new List<AudioFileDetails>();

        for( int i = 0; i < SetNumber; ++i )
        {
            if( ( StartIndex + i ) < Files.Count )
            {
                FileSet.Add( Files[ StartIndex + i ] );
            }
            else
            {
                break;
            }
        }

        return FileSet;
    }

}

public class
AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource MusicSource;
    private AudioSource AmbienceSource;
    public static string MusicFolder    = "/Resources/Audio/Tracks/";
    public static string AmbienceFolder = "/Resources/Audio/Ambience/";
    public static string ResourceMusicPath    = "Audio/Tracks/";
    public static string ResourceAmbiencePath = "Audio/Ambience/";
    [HideInInspector]
    public AudioFile MusicFiles;
    [HideInInspector]
    public AudioFile AmbienceFiles;

    [SerializeField]
    private AudioMixerGroup Mixers;

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
        AmbienceSource = this.gameObject.AddComponent<AudioSource>();
        MusicSource.outputAudioMixerGroup = Mixers;
        AmbienceSource.outputAudioMixerGroup = Mixers;
        MusicSource.volume = .5f;
        AmbienceSource.volume = .25f;
        MusicSource.loop = true;
        AmbienceSource.loop = true;
        GetAllAudioTracksFromResources();
        return Valid;
    }

    public void
    PlayMusic()
    {
    }

    public void
    SetAmbience()
    {
    }

    public void
    SetMusicTrack( AudioClip clip )
    {
        MusicSource.clip = clip;
    }

    public void
    SetAmbienceTrack( AudioClip clip )
    {
        AmbienceSource.clip = clip;
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
    SetMusicPlay(  )
    {
        if ( !MusicSource.isPlaying )
        {
            MusicSource.Play();
        }
        else
        {
            MusicSource.Pause();
        }
    }

    public void
    SetAmbiencePlay(  )
    {
        if ( !AmbienceSource.isPlaying )
        {
            AmbienceSource.Play();
        }
        else
        {
            AmbienceSource.Pause();
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

    private void
    GetAllAudioTracksFromResources()
    {
        DirectoryInfo MusicTrackDirectory = new DirectoryInfo( Application.dataPath + MusicFolder );
        DirectoryInfo AmbienceTrackDirectory = new DirectoryInfo( Application.dataPath + AmbienceFolder );
        FileInfo[] musicInfo    = MusicTrackDirectory.GetFiles("*.wav");
        FileInfo[] ambienceInfo = AmbienceTrackDirectory.GetFiles("*.wav");
        MusicFiles    = new AudioFile();
        AmbienceFiles = new AudioFile();
        MusicFiles.Files   = new List< AudioFileDetails >();
        AmbienceFiles.Files = new List< AudioFileDetails >();

        for( int i = 0; i < musicInfo.Length; ++i )
        {
            string File = musicInfo[i].Name;
            Debug.Log( $"{File} Loaded!!!" );
            AudioFileDetails newAudioFile = ScriptableObject.CreateInstance<AudioFileDetails>();
            newAudioFile.Path = (ResourceMusicPath + File);
            newAudioFile.Name =  File.Substring(0, File.Length - 4);
            MusicFiles.Files.Add( newAudioFile );
        }

        for( int i = 0; i < ambienceInfo.Length; ++i )
        {
            string File = ambienceInfo[i].Name;
            Debug.Log( $"{File} Loaded!!!" );
            AudioFileDetails newAudioFile = ScriptableObject.CreateInstance<AudioFileDetails>();
            newAudioFile.Path = (ResourceMusicPath + File);
            newAudioFile.Name =  File.Substring(0, File.Length - 4);
            AmbienceFiles.Files.Add( newAudioFile );
        }
    }

    public bool
    IsMusicPlaying()
    {
        return MusicSource.isPlaying;
    }

    public bool
    IsAmbiencePlaying()
    {
        return AmbienceSource.isPlaying;
    }

}
