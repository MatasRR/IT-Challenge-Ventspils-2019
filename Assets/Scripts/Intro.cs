using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Intro : MonoBehaviour
{
    public float Duration;
    private float Timer;
    public Image Background;
    private Color BackgroundColor;
    public int CurrentStage;
    public List<GameObject> ActivatableStageItems; // findallgameobjectswithtag?
    private bool FadingIn;


    private void Start()
    {
        Timer = Duration;
        BackgroundColor = Background.color;

        foreach (GameObject go in ActivatableStageItems)
        {
            go.SetActive(false);
        }
        ActivatableStageItems[0].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndIntro();
        }

        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            Timer = Duration;
            BackgroundColor.a = Mathf.Lerp(1, 0, Duration / 3);
            FadingIn = false;
            NextStage();
        }

        if (Timer/Duration < 0.33f && !FadingIn)
        {
            FadingIn = true;
            BackgroundColor.a = Mathf.Lerp(0, 1, Duration / 3);
        }

        Background.color = BackgroundColor;
        //Debug.Log(BackgroundColor.a);
    }

    void NextStage()
    {
        if (CurrentStage == ActivatableStageItems.Count - 1)
        {
            EndIntro();
            return;
        }

        ActivatableStageItems[CurrentStage].SetActive(false);
        CurrentStage++;
        ActivatableStageItems[CurrentStage].SetActive(true);
    }

    void EndIntro()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
