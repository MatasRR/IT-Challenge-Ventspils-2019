using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public Item[] Input;
    public Item[] Output;
    public int MoneyReward;
    public int CureReward;
}
