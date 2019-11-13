using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
   public void changeScenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

   public void quitGame()
    {
        Application.Quit();
    }
}
