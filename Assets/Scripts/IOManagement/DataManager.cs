using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private List<int> ProfileIDs;

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
        ProfileIDs = new List<int>();
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
    {
        using( FileStream ProfileStream = new FileStream( ProfileFilePath, FileMode.Append, FileAccess.Write ))
        {
            using ( StreamWriter ProfileWriter = new StreamWriter( ProfileStream ) )
            {
                ProfileWriter.WriteLine($"Name: {NewProfile.Name}");
                ProfileWriter.WriteLine($"ID: { NewProfile.ProfileID }");
                ProfileWriter.WriteLine($"Alignment: {(int)NewProfile.Alignment}");
                ProfileWriter.WriteLine($"Max Health: {NewProfile.Health}");
                ProfileWriter.WriteLine($"Move Speed: {NewProfile.MovementSpeed}");
            }
        }
        Debug.Log("New Profile Written To Profiles.dat");
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
                    int LineLength = CurrentLine.Length; //NOTE: Validate Line is long enough to peek it 

                    if( LineLength > 6 )
                    {
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
                    }

                    if( LineLength > 4 )
                    {
                        if( String.Equals(CurrentLine.Substring(0,2), "ID" ))
                        {
                            int IDLength = CurrentLine.Length - 4;
                            int Value;
                            if( Int32.TryParse( CurrentLine.Substring(4,IDLength ), out Value ) )
                            {
                                CurrentProfile.ProfileID = Value;
                                Debug.Log($"ProfileID found for {CurrentProfile.Name}, there ID is: {CurrentProfile.ProfileID}");
                                LoadedProfiles.Add( CurrentProfile );
                                continue;
                            }
                        }
                    }

                    if( LineLength >= 10 )
                    {
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
                                continue;
                            }
                        }

                    }


                }
            }
        }
        return LoadedProfiles;
    }

    public bool
    DeleteProfiles( List<int> ProfilesToDelete )
    {
        //NOTE: There is a much better way to do this, that should
        //involve hash tables and operating on a In memory profile table
        List<PieceRequest> Profiles = LoadAllProfiles();

        using( FileStream ProfileStream = new FileStream( ProfileFilePath, FileMode.Create, FileAccess.Write ))
        {
            Debug.Log("Overwrite Profile Written to Profile.dat");
        }

        foreach( PieceRequest Profile in Profiles )
        {
            bool IDToDelete = false;
            for( int i = 0; i < ProfilesToDelete.Count; i++)
            {
                if( Profile.ProfileID == ProfilesToDelete[i] )
                {
                    IDToDelete = true;
                    ProfilesToDelete.RemoveAt( i );
                    break;
                }
            }

            if( !IDToDelete )
            {
                SaveProfile( in Profile );
            }

        }
       return true;
    }

    public int
    GenerateProfileID()
    {
        int ID = 0;
        bool ValidID = false;
        int index = 0;
        if( ProfileIDs.Count > 0 )
        {
            while( !ValidID )
            { //NOTE: loop is exponential as we loop through the table for each element. Could be optimzed to reduce some collisions.
                ID = ProfileIDs[index]++;
                for( int subIndex = 0; subIndex < ProfileIDs.Count; subIndex++ )
                {
                    if( ID == ProfileIDs[ subIndex ] )
                    {
                        ValidID = false;
                        break;
                    }
                    else
                    {
                        ValidID = true;
                    }
                }
                index++;
            }
        }
        return ID;
    }

}
