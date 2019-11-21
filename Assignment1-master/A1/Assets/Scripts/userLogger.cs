using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class userLogger : MonoBehaviour
{
    const string DLL_NAME = "Project1"; // <- change dll name to name of dll
    [DllImport(DLL_NAME)]
    private static extern void resetFile();
    [DllImport(DLL_NAME)]
    private static extern void increLog(float _value, int _index, bool _continousSave);
    [DllImport(DLL_NAME)]
    private static extern void saveToFile();
    [DllImport(DLL_NAME)]
    private static extern void getFromFile();
    [DllImport(DLL_NAME)]
    private static extern float retriLog(int _index);

    public void resetSave()
    {
        resetFile();
    }
    public void incrementLog(float _value, int _index, bool _continousSave)
    {
        increLog(_value, _index, _continousSave);
    }
    public float retrieveLog(int _index)
    {
        return retriLog(_index);
    }
}
