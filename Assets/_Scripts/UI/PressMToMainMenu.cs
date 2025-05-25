using UnityEngine;
using UnityEngine.SceneManagement;

public class PressMToMainMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Main menu");
        }
    }
}
