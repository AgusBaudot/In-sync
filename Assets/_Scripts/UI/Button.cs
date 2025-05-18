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
        //SceneManager.LoadScene(sceneName);
    }

    public void OnExit()
    {
        Debug.Log("Exit pressed");
        Application.Quit();
    }

    public void OnNext(int index)
    {
        Debug.Log("Next floor pressed");
        //SceneManager.LoadScene(index);
    }

    public void OnHub(string sceneName)
    {
        Debug.Log("Hub pressed");
        //SceneManager.LoadScene(sceneName);
    }

    public void OnMainMenu(string sceneName)
    {
        Debug.Log("Main menu pressed");
        SceneManager.LoadScene(sceneName);
    }
}
