using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class userLoggerProfiler : MonoBehaviour
{
    string path;
    public float[] fileOut;
    public float[] fileIn;
    StreamWriter clear;
    StreamWriter Out;
    StreamReader In;

    void clearText()
    {
        clear = new StreamWriter(path + "/UserLog.txt", false);
        clear.Close();
    }

    public void increValue(float _value, int _index)
    {
        fileOut[_index] += _value;

        writeText();
    }

    void writeText()
    {
        clearText();
        Out = new StreamWriter(path + "/UserLog.txt", true);
        for (int i = 0; i <= 4; i++)
        {
            Out.Write(fileOut[i] + " ");
        }
        Out.Close();
    }

    public void readText()
    {
        In = new StreamReader(path + "/UserLog.txt", true);
        Debug.Log(In.ReadLine());
        In.Close();
    }

    void Start()
    {
        path = Application.dataPath;
        for (int i = 0; i <= 4; i++)
        {
            fileOut[i] = 0;
        }
        clearText();
    }
}
