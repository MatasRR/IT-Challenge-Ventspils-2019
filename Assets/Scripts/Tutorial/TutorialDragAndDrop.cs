using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialDragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Tutorial GM;
    private AudioManager AM;

    public int YDragLimit; // Different resolutions??
    private Item ThisItem;
    private GameObject[] Slots;

    private void Start()
    {
        ThisItem = GetComponent<Display>().ThisItem;
        Slots = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManager>().InputSlots;
        GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<Tutorial>();
        AM = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.mousePosition.y > YDragLimit)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        for (int i = 0; i < GM.CurrentResearch; i++)
        {
            RectTransform ItemSlot = Slots[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(ItemSlot, Input.mousePosition))
            {
                ItemSlot.GetComponent<Display>().ThisItem = ThisItem;
                ItemSlot.GetComponent<Display>().UpdateDisplay();
                if (ThisItem.SFX != null)
                {
                    AM.PlaySFX(ThisItem.SFX);
                }
                break;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }
}
