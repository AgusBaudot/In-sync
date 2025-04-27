using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void OnPlay(string sceneName)
    {
        Debug.Log("Play pressed");
        SceneManager.LoadScene(sceneName);
    }

    public void OnSettings(string sceneName)
    {
        Debug.Log("Settings pressed");
        SceneManager.LoadScene(sceneName);
    }

    public void OnExit()
    {
        Debug.Log("Exit pressed");
        Application.Quit();
    }
}
