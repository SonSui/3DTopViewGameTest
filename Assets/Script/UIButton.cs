using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
    public void OnExitGameButtonDown()
    {
        Application.Quit();
    }

    public void OnRetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnGameStart()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
