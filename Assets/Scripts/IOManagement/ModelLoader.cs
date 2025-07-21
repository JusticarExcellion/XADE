using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ModelLoader
{
    private List<GameObject> Models;
    private const string ModelDirectoryFilePath = "/Prefabs/GamePieces/";

    public void
    GetFiveModels( GameObject[] FiveModels, int index )
    {
        if( index < 0 )
        {
            Debug.LogWarning("WARNING: TRYING TO ACCESS INVALID INDEX!!!");
            return;
        }
        if( index >= Models.Count ){ 
            Debug.LogWarning("WARNING: TRYING TO ACCESS INVALID INDEX!!!");
            return;
        }

        for( int i = index; i < Models.Count; ++i )
        {
            FiveModels[i] = Models[i];
        }
    }

    public void
    LoadAllModels()
    {
        Models = new List<GameObject>();
        DirectoryInfo ModelDirectoryInfo = new DirectoryInfo( Application.dataPath + ModelDirectoryFilePath );
        FileInfo[] ModelInfo = ModelDirectoryInfo.GetFiles("*.prefab");
    }
}
