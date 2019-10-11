using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public float WaitTime;

    public void MenuButtonPressed(int Action)
    {
        StartCoroutine(Wait(Action));
    }

    public void LevelSelected(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }

    IEnumerator Wait(int Action)
    {
        yield return new WaitForSeconds(WaitTime);

        switch (Action)
        {
            case 1:
                SceneManager.LoadScene(1);
                break;
            case 2:
                Application.Quit();
                break;
        }
    }
}
