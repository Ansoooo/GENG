using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class networkingUI : MonoBehaviour
{
    //Singleton
    private static networkingUI _instance;
    public static networkingUI instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }



    //Vars
    public GameObject player;

    public GameObject _myIP;
    public GameObject _hostIP;
    public GameObject _toggleHost;
    public GameObject _serverIP;
    public GameObject _start;

    public string myIP;
    public string hostIP;
    public bool toggleHost;
    public string serverIP;

    public bool inited;

    //Funcs
    public void toggleHostFunc(bool input)
    {
        toggleHost = input;
    }

    public void myIPFunc(string input)
    {
        myIP = input;
    }

    public void hostIPFunc(string input)
    {
        hostIP = input;
    }

    public void serverIPFunc(string input)
    {
        serverIP = input;
    }

    public void startFunc()
    {
        _myIP.SetActive(false);
        _hostIP.SetActive(false);
        _toggleHost.SetActive(false);
        _serverIP.SetActive(false);
        player.SetActive(true);
        _start.SetActive(false);
        inited = true;
    }

    //Base Funcs
    void Start()
    {
        toggleHost = true;
        inited = false;
    }

    void Update()
    {
        Screen.fullScreen = false; //BEGONE FULLSCREEN

        if (inited == false)
        {
            if (toggleHost == true)
            {
                _hostIP.SetActive(false);
                _serverIP.SetActive(true);
            }
            else
            {
                _hostIP.SetActive(true);
                _serverIP.SetActive(false);
            }
        }
    }
}
