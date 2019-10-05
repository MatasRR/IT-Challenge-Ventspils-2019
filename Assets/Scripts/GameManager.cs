using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private UIManager UIM;

    [HideInInspector]
    public bool Pause;

    private bool[] ArtefactsFound = new bool[5];
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

    public int HealthyChildren;
    public int IllChildren;
    public int DeadChildren;

    private Item[] InputItems;

    [HideInInspector]
    public bool Pause;

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
        Pause = false;
        Countdown = TimeLimit;
    }

    private void Update()
    {
        if (Pause)
        {
            return;
        }

        if (Countdown > 0)
        {
            Countdown -= Time.deltaTime;
        }
        else
        {
            Countdown = 0;
            UIM.GameOver();
        }

        if (HealthyChildren <= 0 && IllChildren <= 0)
        {
            UIM.GameOver();
        }

        UpdateUI();
        if (Input.GetKeyDown(KeyCode.A))
        {
            ArtefactsFound[2] = true;
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

    bool CheckRecipe (Recipe Target)
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
        Pause = true;
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
        else if (Name == "Artefact of Stamina")
        {
            ArtefactsFound[2] = true;
        }
        else if (Name == "Artefact of Knowledge")
        {
            ArtefactsFound[3] = true;
        }
        else if (Name == "Artefact of Time")
        {
            ArtefactsFound[4] = true;
        }
        else if (Name == "Artefact of Life")
        {
            UIM.Victory();
        }
    }
    public void ChangeMoney(int Change)
    {
        Money += Change;
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

        GotSick = Mathf.Min(GotSick / (ArtefactsFound[2] ? 2 : 1), HealthyChildren);
        Died = Mathf.Min(Died / (ArtefactsFound[2] ? 2 : 1), IllChildren);

        HealthyChildren -= GotSick;
        IllChildren += (GotSick - Died);
        DeadChildren += Died;
    }
}
