using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Tutorial : MonoBehaviour
{
    private UIManager UIM;

    [HideInInspector]
    public bool[] ArtefactsFound = new bool[5];

    private Item[] InputItems;

    public Recipe[] Recipies;
    public int[] ResearchCosts;

    public Display DiscoveryDisplay;
    public TextMeshProUGUI DiscoveryRewardText;

    public Transform ContentGO;
    public GameObject NewItemGO;

    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI CountdownText;
    public Image CountdownImage;
    public int Money;
    public float TimeLimit;
    private float Countdown;

    [HideInInspector]
    public int CurrentResearch;

    public TextMeshProUGUI HealthyChildrenText;
    public TextMeshProUGUI IllChildrenText;
    public TextMeshProUGUI DeadChildrenText;

    public Image HealthyChildrenImage;
    public Image IllChildrenImage;
    public Image DeadChildrenImage;

    public int TotalPopulation;
    private int HealthyChildren;
    private int IllChildren;
    private int DeadChildren;

    public float DiseaseOutbreakFrequency;
    private float DiseaseOutbreakTimer;
    public int MinimumSurplusOfIllChildren;
    public float IllnessBaseOfExponent;
    public float ProbabilityOfDeath;
    public float StrengthOfPandemicCoefficient;
    public float MassDeathThreshold;
    public float MassDeathCoefficient;
    
    [Header("Tutorial UI Stuff")]
    public Button NextButton;
    public TextMeshProUGUI NextButtonText;
    public Button CreateButton;
    private int TutorialStage;
    public string[] StageText;
    public GameObject Panels;
    public GameObject BarPanel;
    public GameObject TimerAndMoneyPanel;
    public GameObject BlueButtonPanel;
    public GameObject ItemPanel;
    public GameObject ResearchPanel;

    private void Start()
    {
        foreach (Recipe r in Recipies)
        {
            r.Output.Crafted = false;
        }

        UIM = gameObject.GetComponent<UIManager>();

        InputItems = new Item[UIM.InputSlots.Length];

        ChangeMoney(0);
        CurrentResearch = 2;
        Countdown = TimeLimit;
        HealthyChildren = TotalPopulation;
        TutorialProgression();
    }

    private void Update()
    {
        Countdowns();
        UpdateUI();
    }

    public void TutorialProgression()
    {
        bool NextStage = true;
        NextButtonText.text = StageText[TutorialStage] + "\n\nPress here to continue";

        Panels.SetActive(false);
        BarPanel.SetActive(false);
        TimerAndMoneyPanel.SetActive(false);
        BlueButtonPanel.SetActive(false);
        ItemPanel.SetActive(false);
        ResearchPanel.SetActive(false);

        switch(TutorialStage)
        {
            case 2:
                Panels.SetActive(true);
                BarPanel.SetActive(true);
                break;
            case 3:
                Panels.SetActive(true);
                TimerAndMoneyPanel.SetActive(true);
                break;
            case 5:
                Panels.SetActive(true);
                ItemPanel.SetActive(true);
                break;
            case 6:
                Panels.SetActive(true);
                ResearchPanel.SetActive(true);
                CreateButton.interactable = true;
                foreach (Transform t in ContentGO)
                {
                    t.GetComponent<TutorialDragAndDrop>().enabled = true;
                }
                if (!Recipies[0].Output.Crafted)
                {
                    NextStage = false;
                }
                break;
            case 8:
                Panels.SetActive(true);
                BlueButtonPanel.SetActive(true);
                foreach (GameObject go in UIM.ResearchButtons)
                {
                    go.GetComponent<Button>().interactable = true;
                }
                if (CurrentResearch != 3)
                {
                    NextStage = false;
                }
                break;
            case 9:
                if (!Recipies[1].Output.Crafted)
                {
                    NextStage = false;
                }
                break;
            case 12:
                Panels.SetActive(true);
                BarPanel.SetActive(true);
                break;
            case 14:
                UIM.ReturnToMenu();
                break;
        }
        
        if (NextStage)
        {
            TutorialStage++;
        }
    }

    #region UI Manager functions
    public void CloseDiscoveryWindow()
    {
        UIM.DiscoveryWindow.SetActive(false);
    }

    public void ChooseSlotsQuantity(int Slots)
    {
        CurrentResearch = Slots;

        for (int i = 2; i < UIM.InputSlots.Length; i++)
        {
            if (Slots >= i + 1)
            {
                UIM.InputSlots[i].SetActive(true);
            }
            else
            {
                UIM.InputSlots[i].SetActive(false);
            }
        }
    }
    #endregion

    void Countdowns()
    {
        if (Countdown > 0)
        {
            Countdown -= Time.deltaTime;
        }
        else
        {
            Countdown = 0;
        }

        if (DiseaseOutbreakTimer > 0)
        {
            DiseaseOutbreakTimer -= Time.deltaTime;
        }
        else
        {
            DiseaseOutbreakTimer = DiseaseOutbreakFrequency;
            DiseaseOutbreak();
        }
    }

    void UpdateUI()
    {
        HealthyChildrenText.text = "Healthy children: " + HealthyChildren.ToString();
        IllChildrenText.text = "Ill children: " + IllChildren.ToString();
        DeadChildrenText.text = "Dead children: " + DeadChildren.ToString();
        CountdownText.text = Mathf.Ceil(Countdown).ToString();

        if (Countdown / TimeLimit < 0.2f && CountdownText.color != Color.red)
        {
            CountdownText.color = Color.red;
        }

        HealthyChildrenImage.fillAmount = (float)HealthyChildren / (HealthyChildren + IllChildren + DeadChildren);
        DeadChildrenImage.fillAmount = (float)DeadChildren / (HealthyChildren + IllChildren + DeadChildren);
        CountdownImage.fillAmount = Countdown / TimeLimit;
    }

    public void Create()
    {
        for (int i = 0; i < CurrentResearch; i++)
        {
            if (UIM.InputSlots[i].GetComponent<Display>().ThisItem == null)
            {
                return;
            }
        }

        ChangeMoney(-ResearchCosts[CurrentResearch]);

        for (int i = 0; i < InputItems.Length; i++)
        {
            InputItems[i] = UIM.InputSlots[i].GetComponent<Display>().ThisItem;
        }

        foreach (Recipe r in Recipies)
        {
            if (!r.Output.Crafted && r.Input.Length == CurrentResearch)
            {
                if (CheckRecipe(r))
                {
                    SuccessfulResearch(r);
                }
            }
        }

        for (int i = 0; i < CurrentResearch; i++)
        {
            if (InputItems[i] != null)
            {
                Display InputSlotDisplay = UIM.InputSlots[i].GetComponent<Display>();
                InputSlotDisplay.ThisItem = null;
                InputSlotDisplay.UpdateDisplay();
            }
        }
    }

    bool CheckRecipe(Recipe Target)
    {
        List<Item> UnusedInput = new List<Item>();
        for (int i = 0; i < CurrentResearch; i++)
        {
            UnusedInput.Add(InputItems[i]);
        }

        for (int i = 0; i < CurrentResearch; i++) /// Recipe components
        {
            bool MatchFound = false;
            for (int j = 0; j < UnusedInput.Count; j++) /// All unused input
            {
                if (UnusedInput[j] == Target.Input[i])
                {
                    MatchFound = true;
                    UnusedInput.Remove(UnusedInput[j]);
                    break;
                }
            }

            if (!MatchFound)
            {
                return false;
            }
        }
        return true;
    }

    void SuccessfulResearch(Recipe Discovery)
    {
        Discovery.Output.Crafted = true;
        ChangeMoney(Discovery.MoneyReward);
        int NumberOfCuredChildren = Mathf.Min(IllChildren, Discovery.CureReward);
        IllChildren -= NumberOfCuredChildren;
        HealthyChildren += NumberOfCuredChildren;

        /// Discovery Window
        UIM.DiscoveryWindow.SetActive(true);
        DiscoveryRewardText.text = "Money: " + (Discovery.MoneyReward > 0 ? "+" : "") + Discovery.MoneyReward + " / Cured: " + (Discovery.CureReward > 0 ? "+" : "") + Discovery.CureReward;
        DiscoveryDisplay.ThisItem = Discovery.Output;
        DiscoveryDisplay.UpdateDisplay();

        /// Item Panel
        GameObject NewItem = Instantiate(NewItemGO);
        NewItem.transform.SetParent(ContentGO);
        Display NewItemDisplay = NewItem.GetComponent<Display>();
        NewItemDisplay.ThisItem = Discovery.Output;
        NewItemDisplay.UpdateDisplay();
    }

    public void ChangeMoney(int Change)
    {
        Money += Change;
        MoneyText.text = "$ " + Money.ToString();

        for (int i = 0; i < UIM.ResearchButtons.Length; i++)
        {
            UIM.ResearchButtons[i].SetActive((Money >= ResearchCosts[i + 2]) ? true : false);
        }
    }

    void DiseaseOutbreak()
    {
        float SickPopulationPercent = (float)(IllChildren + DeadChildren) / TotalPopulation;
        float StrengthOfPandemic = 1 + SickPopulationPercent * StrengthOfPandemicCoefficient;
        int GotSick = (int)(Random.Range(MinimumSurplusOfIllChildren, MinimumSurplusOfIllChildren + StrengthOfPandemic) + Mathf.Pow(IllnessBaseOfExponent, StrengthOfPandemic));
        int Died = (int)(Random.Range(0, IllChildren) * ProbabilityOfDeath * StrengthOfPandemic * (SickPopulationPercent > MassDeathThreshold ? MassDeathCoefficient : 1));

        GotSick = Mathf.Min(GotSick, HealthyChildren);
        Died = Mathf.Min(Died, IllChildren);

        HealthyChildren -= GotSick;
        IllChildren += (GotSick - Died);
        DeadChildren += Died;
    }
}
