using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class send : MonoBehaviour
{
    //Singleton
    private static send _instance;
    public static send instance
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
    public string serverIP;
    public string hostIP;
    public int port;
    public string myIP;
    public string opponentIP;

    public bool isHost;
    public int winner;

    public IPEndPoint[] remoteEndPoint;
    public UdpClient[] client;

    public string message = "";

    public GameObject player;
    public GameObject enemyPlayer;

    public GameObject displayText;
    public GameObject displayInterval;

    [Range(0.1f, 1.0f)]
    public float moveInterval;
    public float timer = 1.0f;
    public int sendCycle;

    //Funcs
    private void sendString(string message, int sendTo)
    {
        displayText.GetComponent<Text>().text = "Send: " + message;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            client[sendTo].Send(data, data.Length, remoteEndPoint[sendTo]);
        }
        catch
        {
            displayText.GetComponent<Text>().text = "Send: Failed to send message, " + message;
        }
    }

    public void sendAtta()
    {
        if(isHost == false)
            message = "!atta";
        sendString(message, 0);
    } //only client
    public void sendMove(float x, float y)
    {
        message = "!move#" + x.ToString() + "|" + y.ToString();
        sendString(message, 0);
    } //both
    public void sendScor()
    {
        if(isHost == true)
            message = "!scor#" + player.GetComponent<CommandPattern.CommandPattern>().playerHealth + "|" + enemyPlayer.GetComponent<enemyPlayer>().enemyPlayerHealth; //host's health, remoteClient health
        sendString(message, 0);
    } //only host
    public void sendJoin()
    {
        if (isHost == false)
            message = "!join#" + myIP;
        sendString(message, 0);
    } //only client

    public void sendFinalScor(bool winner)
    {
        //if (winner) // if winner = true, local client wins
        //    message = "SCORE " + myIP;
        //else
        //    message = "SCORE " + opponentIP; //we use host and remote client interchangably

        //Criteria for high score is the health left over (of the winning player) from the match:
        float highscore = Math.Max(player.GetComponent<CommandPattern.CommandPattern>().playerHealth, enemyPlayer.GetComponent<enemyPlayer>().enemyPlayerHealth);
        message = "SCORE#" + highscore.ToString();
        sendString(message, 1);
    } //only host
    public void sendRankJoin(bool who)
    {
        if (isHost == true)
        {
            if (who == true)
                message = "JOIN " + myIP;
            else
                message = "JOIN " + opponentIP;
        }
        sendString(message, 1);
    } //only host

    //Base Funcs
    void Start()
    {
        //Init the sizes
        remoteEndPoint = new IPEndPoint[2];
        client = new UdpClient[2];

        //Retrieve info from networking UI gameObject
        serverIP = networkingUI.instance.serverIP;
        hostIP = networkingUI.instance.hostIP;
        myIP = networkingUI.instance.myIP;
        isHost = networkingUI.instance.toggleHost;
        winner = 0;

        //Set the host and server send
        // 0 for other player (which is host)
        if (isHost == true) // for host declare to ranking server that we are waiting for players
        {
            //sendJoin(1);
        }
        if (isHost == false) // for host we init this later to let other player do that.
        {
            Debug.Log("initializing the ipaddr");
            remoteEndPoint[0] = new IPEndPoint(IPAddress.Parse(hostIP), port);
            client[0] = new UdpClient();
        }
        // 1 for the ranking server
        remoteEndPoint[1] = new IPEndPoint(IPAddress.Parse(serverIP), 8888); // for now 5000
        client[1] = new UdpClient();

        sendCycle = 0;
        displayInterval.GetComponent<Text>().text = "Interval: " + moveInterval;

        sendRankJoin(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0.0f && winner == 0)
        {
            if (sendCycle == 0)
            {
                sendMove(gameObject.transform.position.x, gameObject.transform.position.y);
            }
            else if (sendCycle == 1)
            {
                sendScor(); // if not host, this does nothing
                sendJoin(); // if not client, this does nothing
            }

            timer = moveInterval;
            if (sendCycle == 1)
            {
                sendCycle = 0;
            }
            else
            {
                sendCycle += 1;
            }
        }

        if (player.GetComponent<CommandPattern.CommandPattern>().playerHealth <= 0 && winner == 0)
        {
            sendScor();
            winner = 1;
        }
        else if (enemyPlayer.GetComponent<enemyPlayer>().enemyPlayerHealth <= 0 && winner == 0)
        {
            sendScor();
            winner = 2;
        }
        if(winner == 1)
        {
            sendFinalScor(false); // local client lost
            Application.Quit();
        }
        else if(winner == 2)
        {
            sendFinalScor(true); // remote client lost
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.Z) && moveInterval > 0.1f)
        {
            moveInterval -= 0.1f;
            displayInterval.GetComponent<Text>().text = "Interval: " + moveInterval;
        }
        else if(Input.GetKeyDown(KeyCode.X) && moveInterval < 1.0f)
        {
            moveInterval += 0.1f;
            displayInterval.GetComponent<Text>().text = "Interval: " + moveInterval;
        }

        if(Input.GetKeyDown(KeyCode.E)) //display secret UI stuff
        { 
            displayInterval.SetActive(!displayInterval.activeInHierarchy);
            displayText.SetActive(!displayText.activeInHierarchy);
            receive.instance.displayText.SetActive(!receive.instance.displayText.activeInHierarchy);
        }

        if(Input.GetKeyDown(KeyCode.R)) // to quick test server
        {
            sendFinalScor(true);
        }

        //Reset
        message = "";
    }
}
