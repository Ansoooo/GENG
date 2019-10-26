using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CommandPattern
{
    public class CommandPattern : MonoBehaviour
    {
        // Object created for control
        public Transform myObj, bullet;
        // keyboard functions
        private Command W, S, A, D, K, Space;
        // object initialization position
        private Vector3 myObjInitPos;

        void Start()
        {
            // keyboard command functions
            W = new MoveFront();
            S = new MoveBack();
            A = new MoveLeft();
            D = new MoveRight();
            K = new AttackFunction();
            Space = new JumpFunction();
            myObjInitPos = myObj.position;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                A.Execute(myObj, A);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                D.Execute(myObj, D);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                S.Execute(myObj, S);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                W.Execute(myObj, W);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                K.Execute(myObj, K);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Space.Execute(myObj, Space);
            }
        }
    }

    public abstract class Command
    {
        // set value
        protected float moveSpd = 10.0f;
        protected float jumpSpeed = 10.0f;
        protected float speed = 500.0f;

        // Execute functions and save it to command
        public abstract void Execute(Transform myObj, Command command);

        // move function for object
        public virtual void Move(Transform myObj) { }

        // jump function for object
        public virtual void Jump(Transform myObj) { }

        //
        public virtual void Attack(Transform bullet) { }
    }

    public class MoveFront : Command
    {
        // call function for object move front and save for command
        public override void Execute(Transform myObj, Command command)
        {
            // move object
            Move(myObj);
        }

        // move object for 0.5f
        public override void Move(Transform myObj)
        {
            myObj.Translate(myObj.forward * moveSpd);
        }
    }

    public class MoveBack : Command
    {
        // call function for object move back and save for command
        public override void Execute(Transform myObj, Command command)
        {
            // move object
            Move(myObj);
        }

        // move object for 0.5f
        public override void Move(Transform myObj)
        {
            myObj.Translate(-myObj.forward * moveSpd);
        }
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
            myObj.Translate(-myObj.right * moveSpd);
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
            myObj.Translate(myObj.right * moveSpd);
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
            myObj.position += Vector3.up * jumpSpeed;
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

        }
    }
}