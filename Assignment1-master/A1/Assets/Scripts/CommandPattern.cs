using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public GameObject healthUI;
        public float speed = 500.0f;
        public float fallMult = 2.5f;
        public float lowfallMult = 2f;

        // keyboard functions
        private Command A, D, K, J, Space;
        // object initialization position
        private Vector3 myObjInitPos;

        // Mobile (HID) functions
        GameObject HIDControls;
        public bool LeftButton = false;
        public bool RightButton = false;
        public bool UpButton = false;
        public bool PunchButton = false;
        public void HIDControlLeftD()
        {
            LeftButton = true;
        }
        public void HIDControlLeftU()
        {
            LeftButton = false;
        }
        public void HIDControlRightD()
        {
            RightButton = true;
        }
        public void HIDControlRightU()
        {
            RightButton = false;
        }
        public void HIDControlJumpD()
        {
            UpButton = true;
        }
        public void HIDControlJumpU()
        {
            UpButton = false;
        }
        public void HIDControlPunchD()
        {
            PunchButton = true;
        }
        public void HIDControlPunchU()
        {
            PunchButton = false;
        }

        void Start()
        {
            HIDControls = GameObject.Find("HIDControls");
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
            //Attack-punching
            if(playerHealth <= 0 || myObj.transform.position.y < -40)
            {
                ChangeScenes death = new ChangeScenes();
                if (!HIDControls) // protect mobile users from the dreaded missing plugin
                {
                    death.changeScenes("ScoreScene");
                }
                else
                    death.changeScenes("GameScene");
                Debug.Log("respawnPlayer");
            }
            healthUI.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerHealth.ToString();
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

            //Attack-shooting
            if (shooting == true)
            {
                Transform newBullet = Instantiate(bullet, myObj.position, Quaternion.identity) as Transform;
                //newBullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * speed);
                newBullet.GetComponent<Rigidbody>().AddForce(speed, 0, 0);
                shooting = false;
            }

            //Jump Phys
            if (GetComponent<Rigidbody>().velocity.y < 0.0f) //When after peak of jump, increase gravity.
            {
                GetComponent<Rigidbody>().velocity += Vector3.up * Physics2D.gravity.y * (fallMult - 1) * Time.deltaTime;
            }
            else if (GetComponent<Rigidbody>().velocity.y > 0.0f && !Input.GetKey(KeyCode.Space))
            {
                GetComponent<Rigidbody>().velocity += Vector3.up * Physics2D.gravity.y * (lowfallMult - 1) * Time.deltaTime;
            }


            //COMMAND PATTERN INPUT
            if (Input.GetKey(KeyCode.A) || LeftButton == true)
            {
                A.Execute(myObj, A);
            }
            else if (Input.GetKey(KeyCode.D) || RightButton == true)
            {
                D.Execute(myObj, D);
            }
            else
            {
                AnimationManager accessAnimationManager = GameObject.Find("PlayerAnimationManager").GetComponent<AnimationManager>();
                accessAnimationManager.switchAnimation(0); // reset to idle animation
            }

            if (Input.GetKey(KeyCode.K))
            {
                K.Execute(myObj, K);
            }
            else if (Input.GetKey(KeyCode.J) || PunchButton == true)
            {
                J.Execute(myObj, J);
            }
            else if (Input.GetKey(KeyCode.Space) || UpButton == true)
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

            AnimationManager accessAnimationManager = GameObject.Find("PlayerAnimationManager").GetComponent<AnimationManager>();
            accessAnimationManager.turnAround(true);
            accessAnimationManager.switchAnimation(1);

            TestManager accessTestManager = GameObject.Find("TestRocket").GetComponent<TestManager>();
            accessTestManager.direction = true;

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

            AnimationManager accessAnimationManager = GameObject.Find("PlayerAnimationManager").GetComponent<AnimationManager>();
            accessAnimationManager.turnAround(false);
            accessAnimationManager.switchAnimation(1);

            TestManager accessTestManager = GameObject.Find("TestRocket").GetComponent<TestManager>();
            accessTestManager.direction = false;

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
            TutorialText accessTutorialText = GameObject.Find("TutorialText").GetComponent<TutorialText>();
            if (accessTutorialText.stageNum == 2f)
            { accessTutorialText.updateText(accessTutorialText.stageNum, 2f); }

            CommandPattern accessCommandPattern = myObj.gameObject.GetComponent<CommandPattern>();
            if (accessCommandPattern.attackingTimer == 0)
            {
                accessCommandPattern.attacking = true;
                accessCommandPattern.attackingTimer = 0.25f;
            }
        }
    }
}