using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class receive : MonoBehaviour
{
    //Singleton
    private static receive _instance;
    public static receive instance
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
    Thread receiver;
    UdpClient client;

    public int port;
    public string message = "";

    public GameObject player;
    public GameObject enemyPlayer;
    public GameObject displayText;

    //Funcs
    private void ReceiveData()
    {
        client = new UdpClient(port);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);
                message = text;
            }
            catch
            {
                Debug.Log("Waiting for Message");
            }
        }
    }

    public void interpretMessage(string _message)
    {
        displayText.GetComponent<Text>().text = "Recv: " + message;
   
        if (_message.Substring(0,5) == "!atta")
        {
            GameObject.Find("Player").GetComponent<CommandPattern.CommandPattern>().playerHealth -= 10;
        }

        if (_message.Substring(0,5) == "!move")
        {
            enemyPlayer.SetActive(true);
            string[] info = _message.Split('#'); //info[0] = !move,  info[1] = X|Y
            string[] coords = info[1].Split('|'); //coords[0] = X,  coords[1] = Y
         
            enemyPlayer.GetComponent<enemyPlayer>().newPosi(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        if (_message.Substring(0,5) == "!scor")
        {
            string[] info = _message.Split('#'); //info[0] = !scor,  info[1] = p1Health|p2Health
            string[] healthSet = info[1].Split('|');

            player.GetComponent<CommandPattern.CommandPattern>().playerHealth = int.Parse(healthSet[1]); //hosts health, this client's health
            enemyPlayer.GetComponent<enemyPlayer>().enemyPlayerHealth = int.Parse(healthSet[0]);
        }

        if (_message.Substring(0,5) == "!join")
        {
            string[] info = _message.Split('#'); //info[0] = !join,  info[1] = otherplayerip
            send.instance.remoteEndPoint[0] = new IPEndPoint(IPAddress.Parse(info[1]), port);
            send.instance.client[0] = new UdpClient();
            send.instance.opponentIP = info[1]; // store ip for winner case later
        }

        //Reset
        message = "";
    }

    //Base Funcs
    void Start()
    {
        receiver = new Thread(new ThreadStart(ReceiveData));
        receiver.IsBackground = true;
        receiver.Start();
    }

    void Update()
    {
        if (message != "")
        {
            //interpret message here
            interpretMessage(message);
        }
    }
}
