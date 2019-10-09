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

    public GameObject ItemVisibilityButton;
    public GameObject ResearchVisibilityButton;
    public TextMeshProUGUI ItemVisibilityButtonText;
    public TextMeshProUGUI ResearchVisibilityButtonText;

    public int ButtonOverhang;

    public GameObject[] InputSlots;
    public GameObject[] ResearchButtons;

    private void Start()
    {
        GM = gameObject.GetComponent<GameManager>();
    }

    public void ChangeItemVisibility()
    {
        ItemPanel.SetActive(!ItemPanel.activeSelf);
        UpdateButtonPosition();
    }

    public void ChangeResearchVisibility()
    {
        ResearchPanel.SetActive(!ResearchPanel.activeSelf);
        UpdateButtonPosition();
    }

    public void DoPause()
    {
        GM.Pause = !GM.Pause;
        PauseWindow.SetActive(!PauseWindow.activeSelf);
    }

    public void ChooseSlotsQuantity(int Slots)
    {
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

    void UpdateButtonPosition()
    {
        float Temp;

        Temp = ItemPanel.activeSelf ? ItemPanel.GetComponent<RectTransform>().rect.height : ButtonOverhang;
        ItemVisibilityButton.transform.position = new Vector2(ItemVisibilityButton.transform.position.x, Temp);
        ItemVisibilityButtonText.text = ItemPanel.activeSelf ? "↓" : "↑";

        Temp = ResearchPanel.activeSelf ? ResearchPanel.GetComponent<RectTransform>().rect.width : ButtonOverhang;
        ResearchVisibilityButton.transform.position = new Vector2(Temp, ResearchVisibilityButton.transform.position.y);
        ResearchVisibilityButtonText.text = ResearchPanel.activeSelf ? "←" : "→";

        if (!ResearchPanel.activeSelf)
        {
            for (int i = 0; i < ResearchButtons.Length; i++)
            {
                ResearchButtons[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < ResearchButtons.Length; i++)
            {
                ResearchButtons[i].SetActive((GM.Money >= GM.ResearchCosts[i + 2]) ? true : false);
            }
        }
    }

    public void CloseDiscoveryWindow()
    {
        GM.Pause = false;
        DiscoveryWindow.SetActive(false);

        if (SceneManager.GetActiveScene().name != "Endless" && GM.ArtefactsFound[GM.ArtefactsFound.Length - 1])
        {
            Victory();
        }
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
