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
    public int CurrentStage;
    public List<GameObject> ActivatableStageItems; // findallgameobjectswithtag?
    private bool FadingIn;


    private void Start()
    {
        Timer = Duration;

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
            FadingIn = false;
            StartCoroutine(FadeTo(0, Duration/3));
            NextStage();
        }

        if (Timer/Duration < 0.33f && !FadingIn)
        {
            FadingIn = true;
            StartCoroutine(FadeTo(1, Duration / 3));
        }
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

    IEnumerator FadeTo(float AlphaTarget, float TransitionDuration)
    {
        float Alpha = Background.color.a;
        for (float i = 0f; i < 1f; i += Time.deltaTime / TransitionDuration)
        {
            Color NewColor = new Color(Background.color.r, Background.color.g, Background.color.b, Mathf.Lerp(Alpha, AlphaTarget, i));
            Background.color = NewColor;
            yield return null;
        }
    }

    void EndIntro()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
