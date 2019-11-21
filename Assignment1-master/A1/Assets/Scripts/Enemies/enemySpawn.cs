using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    public GameObject largeEnemy;
    public GameObject smallEnemy;

    private int numSpawnPoints = 0;
    public float[] spawnX;
    public float[] spawnY;
    public float[] spawnZ;

    private ScoreText score;
    private bool scoreSpawn;

    void addSpawnPoint(float x, float y, float z)
    {
        spawnX[numSpawnPoints] = x;
        spawnY[numSpawnPoints] = y;
        spawnZ[numSpawnPoints] = z;
        numSpawnPoints += 1;
    }

    public void spawnEnemies(float L, float S, int location) //spawn L number of large enemies and S number of small enemies at spawn coordinates of location
    {
        if (location <= numSpawnPoints)
        {
            Vector3 position = new Vector3(spawnX[location], spawnY[location], spawnZ[location]);

            for (int i = 0; i < L; i++)
            {
                Instantiate(largeEnemy, position, Quaternion.identity);
            }
            for (int i = 0; i < S; i++)
            {
                Instantiate(smallEnemy, position, Quaternion.identity);
            }
        }
    }

    void Start()
    {
        addSpawnPoint(-250, 22, 0);
        addSpawnPoint(160, 22, 0);

        score = GameObject.Find("Score").GetComponent<ScoreText>();
        scoreSpawn = true;
    }

    void Update()
    {
        if(score.getScore() == 1 && scoreSpawn == true)
        {
            spawnEnemies(0, 1, 1);
            spawnEnemies(0, 2, 0);
            scoreSpawn = false;
        }
        else if(score.getScore() == 2)
        {
            scoreSpawn = true;
        }

        if(score.getScore() == 4 && scoreSpawn == true)
        {
            spawnEnemies(1, 0, 0);
            scoreSpawn = false;
        }
        else if (score.getScore() == 5)
        {
            scoreSpawn = true;
            ChangeScenes win = new ChangeScenes();
            win.changeScenes("ScoreScene");
        }
    }
}
