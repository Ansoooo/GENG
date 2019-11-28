using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Factory method from https://www.youtube.com/watch?v=n3YAuCsIWqA 

public interface typeSpawner
{
    void assessHealth();
    bool getHealthStatus();
    void takeDmg(float a);
    float getHealth();

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

    public float getHealth()
    {
        return health;
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
    public float animGetHealth()
    {
        return healthStatus;
    }

    public void takeDmg(float amount)
    {
        health -= amount;
    }
    public float getHealth()
    {
        return health;
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
    private Renderer thisRend;
    public Text healthBarPrefab;
    private Text myHealthBar;

    private ScoreText score;
    private userLogger log;
    private userLoggerProfiler logProfiler;
    private AnimationManager anim;

    void manageAttacks()
    {
        CommandPattern.CommandPattern accessCommandPattern = player.gameObject.GetComponent<CommandPattern.CommandPattern>();

        //PLAYER - ENEMY ATTACKS
        if (transform.position.x > player.transform.position.x - player.transform.localScale.x - 10 && transform.position.x < player.transform.position.x + player.transform.localScale.x + 10 && transform.position.y > player.transform.position.y - player.transform.localScale.y - 10 && transform.position.y < player.transform.position.y + player.transform.localScale.y + 10)
        {
            attackPlayerTimer -= Time.deltaTime; // enemy attack cooldown

            //ENEMY ATTACK
            if (attackPlayerTimer < 0 && !(accessCommandPattern.playerHealth <= 0))
            {
                accessCommandPattern.playerHealth -= 10f;
                attackPlayerTimer = 0.75f;
                if (!GameObject.Find("HIDControls"))
                    log.incrementLog(1f, 2, true);
                else
                    logProfiler.increValue(1f, 2);
            }

            //PLAYER ATTACK
            if (accessCommandPattern.attacking == true)
            {
                spawner[0].takeDmg(10f);
                gameObject.GetComponent<Renderer>().material.color = player.GetComponent<Renderer>().material.color;
                if (!GameObject.Find("HIDControls"))
                    log.incrementLog(1f, 0, true);
                else
                    logProfiler.increValue(1f, 0);
            }
            else
                gameObject.GetComponent<Renderer>().material.color = thisRend.material.color;
        }
        else // reset enemy attack cooldown if player leaves a.range
        {
            attackPlayerTimer = 1;
        }

        //ENEMY DEATHS
        if (spawner[0].getHealthStatus() == false || gameObject.transform.position.y < -20)
        {
            score.addScore();
            RidHealthBar();
            Destroy(gameObject);
        }
    }

    void manageAnimation()
    {
        if (gameObject.transform.position.x > player.transform.position.x + player.transform.localScale.x + 5)
        {
            anim.switchAnimation(1);
                anim.turnAround(true);
        }
        else if (gameObject.transform.position.x < player.transform.position.x - player.transform.localScale.x - 5)
        {
            anim.switchAnimation(1);
                anim.turnAround(false);
        }
        else
            anim.switchAnimation(0);   
    }

    public float GetHealth()
    {
        return spawner[0].getHealth();
    }
    private void GetHealthBar()
    {
        myHealthBar = Instantiate(healthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        myHealthBar.transform.SetParent(GameObject.Find("Canvas").transform);
        myHealthBar.GetComponent<AttachText>().parent = gameObject;
        myHealthBar.gameObject.SetActive(true);
    }
    private void RidHealthBar()
    {
        Destroy(myHealthBar);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Bullet")) // CHANGE TO BULLET TAG
        {
            collision.gameObject.SetActive(false);
            spawner[0].takeDmg(20f);
        }
    }

    void Start()
    {
        if (typeSwitcher == 1)
        {
            spawner.Add(sFactory.GetType("largeEnemy"));
        }
        else if (typeSwitcher == 2)
        {
            spawner.Add(sFactory.GetType("smallEnemy"));
        }

        gameObject.transform.parent = GameObject.Find("EnemyInstances").transform;
        thisRend = GameObject.Find("largeEnemyPrefab").GetComponent<Renderer>();
        GetHealthBar();

        score = GameObject.Find("Score").GetComponent<ScoreText>();
        log = GameObject.Find("UserLog").GetComponent<userLogger>();
        logProfiler = GameObject.Find("UserLog").GetComponent<userLoggerProfiler>();
        anim = gameObject.GetComponentInChildren<AnimationManager>();
    }

    void Update()
    {
        manageAnimation();
        manageAttacks();
        spawner[0].process(transform, player.transform);
    }
}

