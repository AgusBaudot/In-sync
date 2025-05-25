using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToWinLooseScreen : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _canvas.transform.GetChild(1).gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            _canvas.transform.GetChild(0).gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
