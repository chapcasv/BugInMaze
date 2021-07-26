using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData 
{
    private static string Path = Application.dataPath + "bugInMaze.test";
    public static void SaveStage(List<Stage> allStage) 
    {
        BinaryFormatter bin = new BinaryFormatter();
        FileStream fs = new FileStream(Path, FileMode.Create);

        bin.Serialize(fs, allStage);
        fs.Close();
    }

    public static List<Stage> LoadStage()
    {
        if (File.Exists(Path))
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(Path, FileMode.Open);
            List<Stage> allStage = bin.Deserialize(fs) as List<Stage>;
            fs.Close();
            return allStage;

        }
        else
        {
            return null;
        }
    }
}
