using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControl : MonoBehaviour
{
    private static playerControl _PControlInstance;
    public static playerControl singletonGetInstance()
    {
        if (_PControlInstance == null) //If no instance, make one.
        {
            _PControlInstance = new playerControl();
        }
            return _PControlInstance;
    }

    private float jumpVelocity = 20.0f;

    public bool groundState;
    public bool liveState;
    Collider cl;

    //RETRIEVE COMPONENTS
    private void Awake()
    {
        cl = GetComponent<Collider>();
    }

    //MANAGE PLAYER
    void updateState()
    {
        //Raycast method from https://answers.unity.com/questions/196381/how-do-i-check-if-my-rigidbody-player-is-grounded.html
        //++ +- -+ -- corners of player object, so we can corner jump.
        if (Physics.Raycast(new Vector3(transform.position.x + transform.localScale.x, transform.position.y, transform.position.z + transform.localScale.z), -Vector3.up, cl.bounds.extents.y + 0.1f) || Physics.Raycast(new Vector3(transform.position.x + transform.localScale.x, transform.position.y, transform.position.z - transform.localScale.z), -Vector3.up, cl.bounds.extents.y + 0.1f) || Physics.Raycast(new Vector3(transform.position.x - transform.localScale.x, transform.position.y, transform.position.z + transform.localScale.z), -Vector3.up, cl.bounds.extents.y + 0.1f) || Physics.Raycast(new Vector3(transform.position.x - transform.localScale.x, transform.position.y, transform.position.z - transform.localScale.z), -Vector3.up, cl.bounds.extents.y + 0.1f))
        {
            groundState = true;
        }
        else
        {
            groundState = false;
        }

        //If player falls off platform, they dead.
        if (transform.position.y < -50.0f)
        {
            liveState = false;
        }
    }
    void movePlayer()
    {
        //Horizontal planar movement.
        if (Input.GetKey(KeyCode.W)) // Forward
        {
            transform.Translate(0f, 0f, 75f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) // Backward
        {
            transform.Translate(0f, 0f, -75f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A)) // Left
        {
            transform.Translate(-75f * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D)) // Right
        {
            transform.Translate(75f * Time.deltaTime, 0f, 0f);
        }

        //Jump check and physics.
        if (groundState == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Rigidbody>().velocity = Vector3.up * jumpVelocity;
            }
        }
        
        if (GetComponent<Rigidbody>().velocity.y < 0.0f) //When after peak of jump, increase gravity.
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * Physics2D.gravity * 4;
        }


        //Death check and respawn player.
        if(liveState == false)
        {
            Vector3 respawn = new Vector3(100.0f, 22.5f, 0.0f);
            transform.position = respawn;
            liveState = true;
        }
    }

    //UPDATE
    void Update()
    {
        updateState();
        movePlayer();

        if(Input.GetKey("escape"))
        {
            Debug.Log("quit");
            Application.Quit();
        }
    }
}
