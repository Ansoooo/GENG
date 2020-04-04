using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyPlayer : MonoBehaviour
{
    //Vars
    public float enemyPlayerHealth = 100f;
    private GameObject healthUI;

    public float X, Y, pX, pY;
    public float vX, vY;
    public float timer;

    AnimationManager animator;

    //Funcs
    public void newPosi(float _X, float _Y)
    {
        pX = X; // previous X and Y is equal to old X and Y
        pY = Y;

        X = _X; // old X and Y is equal to new X and Y
        Y = _Y;

        //Calculate velocities
        vX = (X - pX) / timer;
        vY = (Y - pY) / timer;
        timer = 0; // reset timer
    }

    //Base Funcs
    void Start()
    {
        healthUI = GameObject.Find("EnemyPlayerHealth");
        animator = gameObject.GetComponentInChildren<AnimationManager>();

        vX = 0; //no velocity when we start
        vY = 0;

        X = gameObject.transform.position.x;
        Y = gameObject.transform.position.y;
        pX = X;
        pY = Y;
    }

    void Update()
    {
        healthUI.GetComponent<Text>().text = "Health: " + enemyPlayerHealth;

        timer += Time.deltaTime;
        gameObject.transform.position = new Vector3(pX + (vX * timer), pY + (vY * timer), gameObject.transform.position.z);

        //Animation stuff
        if(X == gameObject.transform.position.x && Y == gameObject.transform.position.y)
        {
            animator.switchAnimation(0);
        }
        else if(X < gameObject.transform.position.x)
        {
            animator.switchAnimation(1);
            animator.turnAround(true);
        }
        else if(X > gameObject.transform.position.x)
        {
            animator.switchAnimation(1);
            animator.turnAround(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (send.instance.isHost == true) // if host directly edit enemy health
            {
                enemyPlayerHealth -= 10;
                Debug.Log(enemyPlayerHealth);
            }

            send.instance.sendAtta(); // if not host, just send a update to host that you attacked.
        }
    }
}
