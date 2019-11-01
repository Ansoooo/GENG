﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CommandPattern
{
    public class CommandPattern : MonoBehaviour
    {
        // Object created for control
        public Transform myObj, bullet;
        public bool attacking = false;
        public bool shooting = false;
        public float attackingTimer = 0.25f;
        public float playerHealth = 100f;
        public float speed = 500.0f;

        // keyboard functions
        private Command A, D, K, J, Space;
        // object initialization position
        private Vector3 myObjInitPos;

        void Start()
        {
            // keyboard command functions
            A = new MoveLeft();
            D = new MoveRight();
            K = new AttackFunction();
            J = new PunchFunction();
            Space = new JumpFunction();
            myObjInitPos = myObj.position;
        }

        void Update()
        {
            if(playerHealth <= 0)
            {
                //add respawn func here.
                Debug.Log("respawnPlayer");
            }
            if (attacking == true)
                attacking = false;
            if(attackingTimer <= 0)
            {
                attackingTimer = 0;
            }
            else
            {
                attackingTimer -= Time.deltaTime;
            }

            if (shooting == true)
            {
                Transform newBullet = Instantiate(bullet, myObj.position, Quaternion.identity) as Transform;
                //newBullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * speed);
                newBullet.GetComponent<Rigidbody>().AddForce(speed, 0, 0);
                shooting = false;
            }

            if (GetComponent<Rigidbody>().velocity.y < 0.0f) //When after peak of jump, increase gravity.
            {
                GetComponent<Rigidbody>().velocity = Vector3.up * Physics2D.gravity * 4;
            }

            if (Input.GetKey(KeyCode.A))
            {
                A.Execute(myObj, A);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                D.Execute(myObj, D);
            }
            if (Input.GetKey(KeyCode.K))
            {
                K.Execute(myObj, K);
            }
            else if (Input.GetKey(KeyCode.J))
            {
                J.Execute(myObj, J);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                Space.Execute(myObj, Space);
            }
            
        }
    }

    public abstract class Command
    {
        // set value
        protected float moveSpd = 2.0f;
        protected float jumpSpeed = 20.0f;

        // Execute functions and save it to command
        public abstract void Execute(Transform myObj, Command command);

        // move function for object
        public virtual void Move(Transform myObj) { }

        // jump function for object
        public virtual void Jump(Transform myObj) { }
        //
        public virtual void Attack(Transform bullet) { }
        public virtual void Punch(Transform myObj) { }
    }

    public class MoveLeft : Command
    {
        // call function for object move left and save for command
        public override void Execute(Transform myObj, Command command)
        {
            // move object
            Move(myObj);
        }

        // move object for 0.5f
        public override void Move(Transform myObj)
        {
            TutorialText accessTutorialText = GameObject.Find("TutorialText").GetComponent<TutorialText>();
            if (accessTutorialText.stageNum == 1f)
            { accessTutorialText.updateText(accessTutorialText.stageNum, 1f); }

            if (Physics.Raycast(new Vector3(myObj.position.x, myObj.position.y, myObj.position.z), -Vector3.up, myObj.GetComponent<Collider>().bounds.extents.y + 0.1f))
                myObj.Translate(-myObj.right * moveSpd);
            else
                myObj.Translate(-myObj.right * moveSpd * 0.5f);
        }
    }

    public class MoveRight : Command
    {
        // call function for object move right and save for command
        public override void Execute(Transform myObj, Command command)
        {
            // move object
            Move(myObj);
        }

        // move object for 0.5f
        public override void Move(Transform myObj)
        {
            TutorialText accessTutorialText = GameObject.Find("TutorialText").GetComponent<TutorialText>();
            if (accessTutorialText.stageNum == 1.2f)
            { accessTutorialText.updateText(accessTutorialText.stageNum, 1.2f); }

            if (Physics.Raycast(new Vector3(myObj.position.x, myObj.position.y, myObj.position.z), -Vector3.up, myObj.GetComponent<Collider>().bounds.extents.y + 0.1f))
                myObj.Translate(myObj.right * moveSpd);
            else
                myObj.Translate(myObj.right * moveSpd * 0.5f);
        }
    }

    public class JumpFunction : Command
    {
        // call function for object move front and save for command
        public override void Execute(Transform myObj, Command command)
        {
            // move object
            Jump(myObj);
        }

        // move object for 0.5f
        public override void Jump(Transform myObj)
        {
            TutorialText accessTutorialText = GameObject.Find("TutorialText").GetComponent<TutorialText>();
            if (accessTutorialText.stageNum == 1.4f)
            { accessTutorialText.updateText(accessTutorialText.stageNum, 1.4f); }

            if (Physics.Raycast(new Vector3(myObj.position.x, myObj.position.y, myObj.position.z), -Vector3.up, myObj.GetComponent<Collider>().bounds.extents.y + 0.1f))
                myObj.GetComponent<Rigidbody>().velocity = Vector3.up * jumpSpeed;
        }
    }

    public class AttackFunction : Command
    {
        // call function for object move front and save for command
        public override void Execute(Transform myObj, Command command)
        {
            Attack(myObj);
        }

        // move object for 0.5f
        public override void Attack(Transform bullet)
        {
            CommandPattern shootCommandPattern = bullet.gameObject.GetComponent<CommandPattern>();
            shootCommandPattern.shooting = true;     
        }
    }

    public class PunchFunction : Command 
    {
        public override void Execute(Transform myObj, Command command)
        {
            Punch(myObj);
        }

        public override void Punch(Transform myObj)
        {
            CommandPattern accessCommandPattern = myObj.gameObject.GetComponent<CommandPattern>();
            if (accessCommandPattern.attackingTimer == 0)
            {
                accessCommandPattern.attacking = true;
                accessCommandPattern.attackingTimer = 0.25f;
            }
        }
    }
}