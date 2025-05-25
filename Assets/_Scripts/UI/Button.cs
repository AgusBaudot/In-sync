using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void OnPlay(string sceneName)
    {
        Debug.Log("Play pressed");
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void OnSettings(string sceneName)
    {
        Debug.Log("Settings pressed");
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void OnExit()
    {
        Debug.Log("Exit pressed");
        Time.timeScale = 1;
        Application.Quit();
    }

    public void OnNext(int index)
    {
        Debug.Log("Next floor pressed");
        Time.timeScale = 1;
        //SceneManager.LoadScene(index);
    }

    public void OnHub(string sceneName)
    {
        Debug.Log("Hub pressed");
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void OnMainMenu(string sceneName)
    {
        Debug.Log("Main menu pressed");
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void TryAgain()
    {
        Debug.Log("Level restarted");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
