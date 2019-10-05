using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Display : MonoBehaviour
{
    public bool IsSlot;
    public Item ThisItem;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public Image Icon;
    public GameObject Placeholder;

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (ThisItem != null)
        {
            Name.text = ThisItem.Name;
            Description.text = ThisItem.Description;
            Icon.sprite = ThisItem.Icon;

            if (Placeholder.activeSelf)
            {
                Placeholder.SetActive(false);
                Color SpriteColor = Icon.color;
                SpriteColor.a = 1;
                Icon.color = SpriteColor;
            }
        }
        else if (!Placeholder.activeSelf)
        {
            Name.text = "";
            Description.text = "";

            Placeholder.SetActive(true);
            Color SpriteColor = Icon.color;
            SpriteColor.a = 0;
            Icon.color = SpriteColor;
        }
    }
}
