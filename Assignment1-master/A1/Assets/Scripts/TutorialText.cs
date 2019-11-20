using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    GameObject HIDControls;
    public float stageNum = 0f;
    private float nextStageNum = 0f;
   

    public void progressStage()
    {
        stageNum = nextStageNum;
        updateText(stageNum, 0);
    } // Delay for approval text

    public void updateText(float _stageNum, float _stageKey)
    {
        switch (_stageNum)
        {
            //Movement Stages
            case (1f): // Left
                {
                    if (HIDControls)
                    {
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press Left Arrow to move Left";
                    }
                    else
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press A to move Left"; // instructions given to player
                    if (_stageKey == 1) // move onto next stage
                    {
                        stageNum = 1.1f;
                        updateText(stageNum, 0);
                    }
                    return;
                }
            case (1.1f):
                {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "Good!";
                    nextStageNum = 1.2f;
                    Invoke("progressStage", 2.0f);
                    return;
                }
            case (1.2f): // Right
                {
                    if (HIDControls)
                    {
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press Right Arrow to move Right";
                    }
                    else
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press D to move Right";
                    if (_stageKey == 1.2f)
                    {
                        stageNum = 1.3f;
                        updateText(stageNum, 0);
                    }
                    return;
                }
            case (1.3f):
                {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "Good!";
                    nextStageNum = 1.4f;
                    Invoke("progressStage", 2.0f);
                    return;
                }
            case (1.4f): // Jump
                {
                    if (HIDControls)
                    {
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press Up Arrow to Jump";
                    }
                    else
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press Space to Jump";
                    if (_stageKey == 1.4f)
                    {
                        stageNum = 1.5f;
                        updateText(stageNum, 0);
                    }
                    return;
                }
            case (1.5f):
                {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "Good!";
                    nextStageNum = 2f;
                    Invoke("progressStage", 2.0f);
                    return;
                }
            case (2f): // Punch
                {
                    if (HIDControls)
                    {
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press P to Punch";
                    }
                    else
                        gameObject.GetComponent<UnityEngine.UI.Text>().text = "Press J to Punch";
                    if (_stageKey == 2f)
                    {          
                        stageNum = 2.1f;
                        updateText(stageNum, 0);
                    }
                    else
                    {
                        enemySpawn tutorialSpawn = GameObject.Find("Enemies").GetComponent<enemySpawn>();
                        tutorialSpawn.spawnEnemies(0, 1, 1);
                    }
                    return;
                }
            case (2.1f):
                {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "Good!";
                    nextStageNum = -1f;
                    Invoke("progressStage", 2.0f);
                    return;
                }
            case (-1f): // End Tutorial
                {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "";
                    return;
                }
            default:
                 {
                    gameObject.GetComponent<UnityEngine.UI.Text>().text = "Invalid StageNum";
                    return;
                 }
        }
    }

    void Start()
    {
        HIDControls = GameObject.Find("HIDControls");
        stageNum = 1f;
        updateText(stageNum, 0);
    }
}
