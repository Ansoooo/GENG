using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    int score;
    public bool game;
    private userLogger log;

    public void resetScore()
    {
        score = 0; 
    }

    public void addScore()
    {
        score += 1;
        if (!GameObject.Find("HIDControls"))
            log.incrementLog(1, 1, true);
    }

    public int getScore()
    {
        return score;
    }

    void Start()
    {
            resetScore();
            log = GameObject.Find("UserLog").GetComponent<userLogger>();
        if (game)
        {
            if (!GameObject.Find("HIDControls"))
                log.resetSave();
        }
    }

    void Update()
    {
        if (game)
        {
            gameObject.GetComponent<UnityEngine.UI.Text>().text = "Score: " + score;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Text>().text = 
                "Session Score:" + "\n" + 
                "Punches Thrown: " + log.retrieveLog(0) + "\n" +
                "Punches Received: " + log.retrieveLog(2) + "\n" +
                "Kills Made: " + log.retrieveLog(1);
        }
    }
}
