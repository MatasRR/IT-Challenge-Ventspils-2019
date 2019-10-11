﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private UIManager UIM;

    private Item[] InputItems;
    private Recipe[] Recipies;

    private Recipe ArtefactOfLifeRecipe;

    [HideInInspector]
    public bool Pause;

    [HideInInspector]
    public int CurrentResearch;

    [HideInInspector]
    public bool[] ArtefactsFound = new bool[5];


    [Header("General Numbers")]

    public int[] ResearchCosts;

    public int Money;
    public float TimeLimit;
    private float Countdown;

    public int TotalPopulation;
    private int HealthyChildren;
    private int IllChildren;
    private int DeadChildren;


    [Header("Disease Outbreaks")]

    public float DiseaseOutbreakFrequency;
    private float DiseaseOutbreakTimer;
    public int MinimumSurplusOfIllChildren;
    public float IllnessBaseOfExponent;
    public float ProbabilityOfDeath;
    public float StrengthOfPandemicCoefficient;
    public float MassDeathThreshold;
    public float MassDeathCoefficient;


    [Header("Artefacts")]

    public float TimeReduction;
    public float WealthIncrease;
    public float HerbalismIncresse;
    public float ResilienceIncrese;
    private bool ArtefactCreated;


    [Header("UI")]

    public Display DiscoveryDisplay;
    public TextMeshProUGUI DiscoveryRewardText;

    public Transform ContentGO;
    public GameObject NewItemGO;

    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI CountdownText;
    public Image CountdownImage;

    public TextMeshProUGUI HealthyChildrenText;
    public TextMeshProUGUI IllChildrenText;
    public TextMeshProUGUI DeadChildrenText;

    public Image HealthyChildrenImage;
    public Image IllChildrenImage;
    public Image DeadChildrenImage;

    public GameObject[] ArtefactUI;
    

    private void Start()
    {
        UIM = gameObject.GetComponent<UIManager>();
        InputItems = new Item[UIM.InputSlots.Length];

        Recipies = Resources.FindObjectsOfTypeAll<Recipe>();

        foreach (Recipe r in Recipies)
        {
            foreach (Item i in r.Output)
            {
                i.Crafted = false;
                if (i.name == "Artefact of Life")
                {
                    ArtefactOfLifeRecipe = r;
                }
            }
        }

        ChangeMoney(0);
        CurrentResearch = 2;
        Pause = false;
        Countdown = TimeLimit;
        HealthyChildren = TotalPopulation;
    }

    private void Update()
    {
        if (Pause)
        {
            return;
        }

        Countdowns();
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIM.DoPause();
        }

        if (AllArtefactsFound() && !ArtefactsFound[ArtefactsFound.Length - 1])
        {
            ArtefactsFound[ArtefactsFound.Length - 1] = true;
            SuccessfulResearch(ArtefactOfLifeRecipe, 0);
        }
    }

    void Countdowns()
    {
        if (Countdown > 0)
        {
            Countdown -= Time.deltaTime / (ArtefactsFound[3] ? TimeReduction : 1);
        }
        else
        {
            Countdown = 0;
            UIM.GameOver();
        }

        if (DiseaseOutbreakTimer > 0)
        {
            DiseaseOutbreakTimer -= Time.deltaTime / (ArtefactsFound[3] ? TimeReduction : 1);
        }
        else
        {
            DiseaseOutbreakTimer = DiseaseOutbreakFrequency;
            DiseaseOutbreak();
        }

        if (HealthyChildren <= 0 && IllChildren <= 0)
        {
            UIM.GameOver();
        }
    }

    void UpdateUI()
    {
        HealthyChildrenText.text = HealthyChildren.ToString();
        IllChildrenText.text = IllChildren.ToString();
        DeadChildrenText.text = DeadChildren.ToString();
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
        if (Pause)
        {
            return;
        }

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

        ArtefactCreated = true;

        foreach (Recipe r in Recipies)
        {
            if (r.Input.Length == CurrentResearch && CheckCraftability(r))
            {
                if (CheckInput(r))
                {
                    int No = Random.Range(0, r.Output.Length);
                    while (r.Output[No].Crafted)
                    {
                        No = Random.Range(0, r.Output.Length);
                    }

                    SuccessfulResearch(r, No);
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

    bool CheckCraftability(Recipe Target)
    {
        foreach (Item i in Target.Output)
        {
            if (!i.Crafted)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckInput (Recipe Target)
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

    void SuccessfulResearch(Recipe Discovery, int No)
    {
        Pause = true;
        Discovery.Output[No].Crafted = true;
        CheckForEffects(Discovery.Output[No].name);
        ChangeMoney(Discovery.MoneyReward);

        int NumberOfCuredChildren;
        if (Discovery.CureReward >= 0)
        {
            NumberOfCuredChildren = Mathf.Min((IllChildren), (int)(Discovery.CureReward * (ArtefactsFound[1] ? HerbalismIncresse : 1)));
        }
        else
        {
            NumberOfCuredChildren = -Mathf.Min(HealthyChildren, (int)(-Discovery.CureReward / (ArtefactsFound[1] ? HerbalismIncresse : 1)));
        }
        IllChildren -= NumberOfCuredChildren;
        HealthyChildren += NumberOfCuredChildren;

        /// Discovery Window
        UIM.DiscoveryWindow.SetActive(true);
        DiscoveryRewardText.text = "Money: " + (Discovery.MoneyReward > 0 ? "+" : "") + Discovery.MoneyReward + " / Cured: " + (Discovery.CureReward > 0 ? "+" : "") + Discovery.CureReward;
        DiscoveryDisplay.ThisItem = Discovery.Output[No];
        DiscoveryDisplay.UpdateDisplay();

        if (ArtefactCreated)
        {
            for (int i = 0; i < ArtefactsFound.Length; i++)
            {
                if (ArtefactsFound[i])
                {
                    ArtefactUI[i].SetActive(true);
                }
            }
        }
        else
        {
            /// Item Panel
            GameObject NewItem = Instantiate(NewItemGO);
            NewItem.transform.SetParent(ContentGO);
            Display NewItemDisplay = NewItem.GetComponent<Display>();
            NewItemDisplay.ThisItem = Discovery.Output[No];
            NewItemDisplay.UpdateDisplay();
        }
    }
    
    void CheckForEffects(string Name)
    {
        if (Name == "Artefact of Wealth")
        {
            ArtefactsFound[0] = true;
        }
        else if (Name == "Artefact of Herbalism")
        {
            ArtefactsFound[1] = true;
        }
        else if (Name == "Artefact of Resilience")
        {
            ArtefactsFound[2] = true;
        }
        else if (Name == "Artefact of Time")
        {
            ArtefactsFound[3] = true;
        }
        else if (Name == "Artefact of Life")
        {
            ArtefactsFound[4] = true;
        }
        else
        {
            ArtefactCreated = false;
        }
    }

    bool AllArtefactsFound()
    {
        for (int i = 0; i < ArtefactsFound.Length - 1; i++)
        {
            if (!ArtefactsFound[i])
            {
                return false;
            }
        }
        return true;
    }
    
    public void ChangeMoney(int Change)
    {
        Money += (int) (Change * (ArtefactsFound[0] ? (Change > 0 ? WealthIncrease : 1/WealthIncrease) : 1));
        MoneyText.text = "$ " + Money.ToString();

        for (int i = 0; i < UIM.ResearchButtons.Length; i++)
        {
            UIM.ResearchButtons[i].SetActive((Money >= ResearchCosts[i + 2]) ? true : false);
        }

        if (Money <= 0)
        {
            UIM.GameOver();
        }
    }

    void DiseaseOutbreak()
    {
        float SickPopulationPercent = (float)(IllChildren + DeadChildren) / TotalPopulation;
        float StrengthOfPandemic = 1 + SickPopulationPercent * StrengthOfPandemicCoefficient;
        int GotSick = (int) (Random.Range(MinimumSurplusOfIllChildren, MinimumSurplusOfIllChildren + StrengthOfPandemic) + Mathf.Pow(IllnessBaseOfExponent, StrengthOfPandemic));
        int Died = (int) (Random.Range(0, IllChildren) * ProbabilityOfDeath * StrengthOfPandemic * (SickPopulationPercent > MassDeathThreshold ? MassDeathCoefficient : 1));

        GotSick = Mathf.Min((int)(GotSick / (ArtefactsFound[2] ? ResilienceIncrese : 1)), HealthyChildren);
        Died = Mathf.Min((int)(Died / (ArtefactsFound[2] ? ResilienceIncrese : 1)), IllChildren);

        HealthyChildren -= GotSick;
        IllChildren += (GotSick - Died);
        DeadChildren += Died;
    }
}
