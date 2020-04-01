using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class NetHandler : MonoBehaviour //functions from unity side
{
    private bool StartUp = false;

    NetWrapper ThisWrapper;
    void Start()
    {
        ThisWrapper = gameObject.GetComponent<NetWrapper>();
    }

    void Update()
    {
        if (ThisWrapper.ClientRunning && !StartUp)
        {
            if(ThisWrapper.Name == "PLAYER")
            {
               
            }
            else if(ThisWrapper.Name == "GHOST")
            {
               
            }
        }


        if (StartUp)
        {
            if (ThisWrapper.Name == "PLAYER")
            {
    
            }
            else if (ThisWrapper.Name == "GHOST")
            {
              
            }
            HandleMessage();
        }
    }

    void HandleMessage()
    {
        string Message = ThisWrapper.GetInboundMessage();
        StringReader strRdr = new StringReader(Message);

        string Header = strRdr.ReadLine();

        if (Header == "NULL")
        {
        }
        else if(Header == "CS")
        {
            string Type = strRdr.ReadLine();
            Debug.Log("Type: " + Type);
            if (Type == "UPDP") //udp player
            {

            }
            else if (Type == "UPDG") //udp ghost(player 2)
            {

            }
            else if (Type == "CS")
            {
                Debug.Log("CS ACTIVATED");
                string line;
                while ((line = strRdr.ReadLine()) != null)
                {
                    Debug.Log(line);
                }
            }
        }
        
    }
}
