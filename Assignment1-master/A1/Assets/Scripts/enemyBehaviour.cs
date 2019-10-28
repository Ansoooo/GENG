//Anson Cheng 100585118
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Factory method from https://www.youtube.com/watch?v=n3YAuCsIWqA 

public interface typeSpawner
{
    void assessHealth();
    bool getHealthStatus();
    void takeDmg(float a);

    void jump(Transform a);
    void walk(Transform a, Transform b);
    void process(Transform a, Transform b);
}
public class largeEnemy : typeSpawner
{
    private Collider CL;
    private Rigidbody RB;

    public float health;
    public int healthStatus;
    public float jumpHeight;
    public float walkSpeed;

    public largeEnemy()
    {
        health = 150f;
        healthStatus = 1;
        jumpHeight = 25f;
        walkSpeed = 0.5f;
    }

    public void assessHealth()
    {
        if (health <= 0f)
            healthStatus = 0;
    }
    public bool getHealthStatus()
    {
        if (healthStatus == 0)
        {
            return false;
        }
        return true;
    }
    public void takeDmg(float amount)
    {
        health -= amount;
    }

    public void attack(Transform enemy, Transform player)
    {
        
    }

    public void jump(Transform enemy)
    {
        CL = enemy.GetComponent<Collider>();
        RB = enemy.GetComponent<Rigidbody>();

        if (Physics.Raycast(new Vector3(enemy.position.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
        {
            if (!Physics.Raycast(new Vector3(enemy.position.x + enemy.localScale.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
            {
                RB.velocity = new Vector3(1f, 1f, 0) * jumpHeight;
            }
            else if (!Physics.Raycast(new Vector3(enemy.position.x - enemy.localScale.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
            {
                RB.velocity = new Vector3(-1f, 1f, 0) * jumpHeight;
            }
        }
    }
    public void walk(Transform enemy, Transform player)
    {
        CL = enemy.GetComponent<Collider>();
        if(Physics.Raycast(new Vector3(enemy.position.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
        {
            if (enemy.position.x > player.position.x + player.transform.localScale.x + 5)
            {
                enemy.position = enemy.position + new Vector3(-walkSpeed, 0f, 0f);
            }
            else if (enemy.position.x < player.position.x - player.transform.localScale.x - 5)
            {
                enemy.position = enemy.position + new Vector3(walkSpeed, 0f, 0f);
            }
        }
    }
    public void process(Transform enemy, Transform player)
    {
        assessHealth();
        walk(enemy, player);
        jump(enemy);
    }
}
public class smallEnemy : typeSpawner
{
    private Collider CL;
    private Rigidbody RB;

    public float health;
    public int healthStatus;
    public float jumpHeight;
    public float walkSpeed;

    public smallEnemy()
    {
        health = 100f;
        healthStatus = 2;
        jumpHeight = 25f;
        walkSpeed = 1f;
    }

    public void assessHealth()
    {
        if (health <= 0f)
            healthStatus = 0;
        else if (health <= 50f)
            healthStatus = 1;
    }
    public bool getHealthStatus()
    {
        if (healthStatus == 0)
        {
            return false;
        }
        return true;
    }
    public void takeDmg(float amount)
    {
        health -= amount;
    }

    public void jump(Transform enemy)
    {
        CL = enemy.GetComponent<Collider>();
        RB = enemy.GetComponent<Rigidbody>();

        if (Physics.Raycast(new Vector3(enemy.position.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
        {
            if (!Physics.Raycast(new Vector3(enemy.position.x + enemy.localScale.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
            {
                RB.velocity = new Vector3(1f, 1, 0) * jumpHeight;
            }
            else if (!Physics.Raycast(new Vector3(enemy.position.x - enemy.localScale.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
            {
                RB.velocity = new Vector3(-1f, 1, 0) * jumpHeight;
            }
        }
    }
    public void walk(Transform enemy, Transform player)
    {
        CL = enemy.GetComponent<Collider>();

        if (healthStatus == 2)
        {
            if (Physics.Raycast(new Vector3(enemy.position.x, enemy.position.y, enemy.position.z), -Vector3.up, CL.bounds.extents.y + 0.1f))
            {
                if (enemy.position.x > player.position.x + player.transform.localScale.x + 5)
                {
                    enemy.position = enemy.position + new Vector3(-walkSpeed, 0f, 0f);
                }
                else if (enemy.position.x < player.position.x - player.transform.localScale.x - 5)
                {
                    enemy.position = enemy.position + new Vector3(walkSpeed, 0f, 0f);
                }
            }
        }
        else if(healthStatus == 1) //run!
        {
            if (enemy.position.x > player.position.x)
            {
                enemy.position = enemy.position + new Vector3(walkSpeed, 0f, 0f);
            }
            else if (enemy.position.x < player.position.x)
            {
                enemy.position = enemy.position + new Vector3(-walkSpeed, 0f, 0f);
            }
        }
    }
    public void process(Transform enemy, Transform player)
    {
        assessHealth();
        walk(enemy, player);
        jump(enemy);
    }
}
public class spawnerFactory
{
    public typeSpawner GetType(string type)
    {
        switch (type)
        {
            case ("largeEnemy"):
                return new largeEnemy();
            case ("smallEnemy"):
                return new smallEnemy();
            default:
                return new largeEnemy();
        }
    }
}

public class enemyBehaviour : MonoBehaviour
{
    public GameObject player;

    public List<typeSpawner> spawner = new List<typeSpawner>();
    spawnerFactory sFactory = new spawnerFactory();
    public int typeSwitcher;

    public float attackPlayerTimer = 0.75f;

    private void Awake()
    {
        if (typeSwitcher == 1)
        {
            spawner.Add(sFactory.GetType("largeEnemy"));
        }
        else if (typeSwitcher == 2)
        {
            spawner.Add(sFactory.GetType("smallEnemy"));
        }
    }

    void manageAttacks()
    {
        CommandPattern.CommandPattern accessCommandPattern = player.gameObject.GetComponent<CommandPattern.CommandPattern>();

        //PLAYER - ENEMY ATTACKS
        if (transform.position.x > player.transform.position.x - player.transform.localScale.x - 10 && transform.position.x < player.transform.position.x + player.transform.localScale.x + 10 && transform.position.y > player.transform.position.y - player.transform.localScale.y - 10 && transform.position.y < player.transform.position.y + player.transform.localScale.y + 10)
        {
            attackPlayerTimer -= Time.deltaTime;
            if (attackPlayerTimer < 0 && !(accessCommandPattern.playerHealth <= 0))
            {
                //attack player here
                accessCommandPattern.playerHealth -= 10f;
                attackPlayerTimer = 0.75f;
                //Debug.Log(gameObject.name + "attacks");
            }

            //Use if player initiates an attack
            if (accessCommandPattern.attacking == true)
            {
                spawner[0].takeDmg(10f);
                //Debug.Log(gameObject.name + " says: ow");
            }
        }
        else // reset timer if player leaves zone
        {
            attackPlayerTimer = 1;
        }

        //ENEMY DEATHS and falling off platform
        if (spawner[0].getHealthStatus() == false || gameObject.transform.position.y < -40)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        manageAttacks();
        spawner[0].process(transform, player.transform);
    }
}

