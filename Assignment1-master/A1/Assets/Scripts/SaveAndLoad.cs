using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveAndLoad
{
    public static void Save(string path, string content)
    {
        File.WriteAllText(Application.dataPath + "/" + path, content);
    }

    public static void Load(string path, ref string[] content)
    {
        using(StreamReader sr = new StreamReader(Application.dataPath + "/" + path))
        {
            content = sr.ReadToEnd().Split(',');
            sr.Close();
        }
    }
}