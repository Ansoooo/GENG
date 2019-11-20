using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    int score;

    public void resetScore()
    {
        score = 0; 
    }

    public void addScore()
    {
        score += 1;
    }

    public int getScore()
    {
        return score;
    }

    void Start()
    {
        resetScore();
    }

    void Update()
    {
        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Score: " + score;
    }
}
