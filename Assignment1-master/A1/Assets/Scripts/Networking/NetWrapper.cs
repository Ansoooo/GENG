using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class NetWrapper : MonoBehaviour
{
    public string Name = "";
    public bool ClientRunning = false;
    bool AssignedName = false;
    bool StartUp = false;
    float RefreshTime = 0.1f;
    const string DLL_NAME = "NetworkingDLL";
    [DllImport(DLL_NAME)]
    public static extern int JoinGame();

    [DllImport(DLL_NAME)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public static extern string GetName();

    [DllImport(DLL_NAME)]
    public static extern void SendPacket();

    [DllImport(DLL_NAME)]
    public static extern void RecievePacket();
    [DllImport(DLL_NAME)]
    public static extern void HandlePacket();

    [DllImport(DLL_NAME)]
    public static extern void SetStatus(bool temp);

    [DllImport(DLL_NAME)]
    public static extern bool GetStatus();

    [DllImport(DLL_NAME)]
    public static extern void AddMessage(string value);

    [DllImport(DLL_NAME)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public static extern string GetInboundMsg();

    [DllImport(DLL_NAME)]
    public static extern void RemoveInboundMsg();


    public void SendServerMessage(string value)
    {
        AddMessage(value);
    }

    public string GetInboundMessage()
    {
        return GetInboundMsg();
    }

    public void RemoveInboundMessage()
    {
        RemoveInboundMsg();
    }

    IEnumerator SendPacketRoute()
    {
        while (ClientRunning)
        {
            SendPacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }
    IEnumerator ReceivePacketRoute()
    {
        while (ClientRunning)
        {
            RecievePacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }
    IEnumerator HandlePacketRoute()
    {
        while (ClientRunning)
        {
            HandlePacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }

    void Awake()
    {
        JoinGame();
    }
    
    void Update()
    {
        ClientRunning = GetStatus();
        if (!StartUp && ClientRunning)
        {

            StartCoroutine(SendPacketRoute());
            StartCoroutine(ReceivePacketRoute());
            StartCoroutine(HandlePacketRoute());
            StartUp = true;
        }
        if (!AssignedName)
        {
            Name = GetName();
        }
    }

    private void OnApplicationQuit()
    {
        SetStatus(false);
        ClientRunning = false;
    }
}