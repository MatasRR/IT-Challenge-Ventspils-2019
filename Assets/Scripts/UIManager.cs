using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager GM;

    public GameObject ItemPanel;
    public GameObject ResearchPanel;
    public GameObject DiscoveryWindow;
    public GameObject VictoryWindow;
    public GameObject GameOverWindow;
    public GameObject PauseWindow;

    public GameObject[] InputSlots;
    public GameObject[] ResearchButtons;

    private void Start()
    {
        GM = gameObject.GetComponent<GameManager>();
    }

    public void SwitchItemVisibility()
    {
        ItemPanel.SetActive(!ItemPanel.activeSelf);
    }

    public void SwitchResearchVisibility()
    {
        ResearchPanel.SetActive(!ResearchPanel.activeSelf);
    }

    public void DoPause()
    {
        GM.Pause = !GM.Pause;
        PauseWindow.SetActive(!PauseWindow.activeSelf);
    }

    public void ChooseSlotsQuantity(int Slots)
    {
        ResearchPanel.SetActive(true);

        GM.CurrentResearch = Slots;

        for (int i = 2; i < InputSlots.Length; i++)
        {
            if (Slots >= i + 1)
            {
                InputSlots[i].SetActive(true);
            }
            else
            {
                InputSlots[i].SetActive(false);
            }
        }
    }

    public void CloseDiscoveryWindow()
    {
        GM.Pause = false;
        DiscoveryWindow.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Victory()
    {
        GM.Pause = true;
        VictoryWindow.SetActive(true);
    }

    public void GameOver()
    {
        GM.Pause = true;
        GameOverWindow.SetActive(true);
    }
}
