using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private string ProfileFilePath = Application.dataPath + "/Profiles/Profiles.dat";

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

    //TODO: We should save all entities and their state
    public bool
    SaveGame()
    {
        return true;
    }

    //TODO:  We should open up a dialogue and choose a save file to load
    public bool
    LoadGame()
    {
        return true;
    }

    public void
    SaveProfile( in PieceRequest NewProfile )
    { //TODO: We May need to create unique ID's for each profile so we can delete specific Profiles that may have similar stats
        using( FileStream ProfileStream = new FileStream( ProfileFilePath, FileMode.Append, FileAccess.Write ))
        {
            using ( StreamWriter ProfileWriter = new StreamWriter( ProfileStream ) )
            {
                ProfileWriter.WriteLine($"Name: {NewProfile.Name}");
                ProfileWriter.WriteLine($"Alignment: {(int)NewProfile.Alignment}");
                ProfileWriter.WriteLine($"Max Health: {NewProfile.Health}");
                ProfileWriter.WriteLine($"Move Speed: {NewProfile.MovementSpeed}");
            }
        }
        Debug.Log("New Profile Written To Profiles.dat");
    }

    public void
    LoadProfile( PieceRequest Profile )
    {
        //TODO: Make a profile browser then we can pick a item from the
        //browser and load it here
    }

    public List<PieceRequest>
    LoadAllProfiles()
    {
        List<PieceRequest> LoadedProfiles = new List<PieceRequest>();

        using ( FileStream ProfileStream = new FileStream(ProfileFilePath, FileMode.Open, FileAccess.Read ) )
        {
            using( StreamReader ProfileReader = new StreamReader(ProfileStream ) )
            {
                PieceRequest CurrentProfile = new PieceRequest();
                string? CurrentLine = "";
                while( ProfileReader.Peek() > 0 )
                {
                    CurrentLine = ProfileReader.ReadLine();
                    if( String.Equals(CurrentLine.Substring(0,4), "Name") )
                    {
                        int NameLength = CurrentLine.Length - 6;
                        if( CurrentProfile.Name != "" )
                        {
                            CurrentProfile = new PieceRequest();
                        }
                        CurrentProfile.Name = CurrentLine.Substring(6,NameLength);
                        Debug.Log($"Name Found: {CurrentProfile.Name}");
                        continue;
                    }

                    if(String.Equals( CurrentLine.Substring(0,9), "Alignment"))
                    {
                        int Value;
                        if( Int32.TryParse( CurrentLine.Substring(11,1), out Value ) )
                        {
                            CurrentProfile.Alignment = (Faction)Value;
                            Debug.Log($"Alignment for {CurrentProfile.Name}, they are {CurrentProfile.Alignment}");
                            continue;
                        }
                    }

                    if(String.Equals(CurrentLine.Substring(0,10), "Max Health"))
                    {
                        int HealthValueLength = CurrentLine.Length - 11;
                        int Value;
                        if( Int32.TryParse( CurrentLine.Substring(11,HealthValueLength), out Value ) )
                        {
                            CurrentProfile.Health = Value;
                            Debug.Log($"Max health found for {CurrentProfile.Name}, there health is: {CurrentProfile.Health}");
                            continue;
                        }
                    }

                    if(String.Equals(CurrentLine.Substring(0,10), "Move Speed"))
                    {
                        int MoveSpeedLength = CurrentLine.Length - 11;
                        int Value;
                        if( Int32.TryParse( CurrentLine.Substring(11,MoveSpeedLength), out Value ) )
                        {
                            CurrentProfile.MovementSpeed = Value;
                            Debug.Log($"Move Speed found for {CurrentProfile.Name}, there move speed is: {CurrentProfile.MovementSpeed}");
                            LoadedProfiles.Add( CurrentProfile );
                            continue;
                        }
                    }

                }
            }
        }
        return LoadedProfiles;
    }

    public bool
    DeleteProfile()
    {
        //TODO: Implement Delete Profile
        return true;
    }

}
