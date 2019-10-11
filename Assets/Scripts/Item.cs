using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string Description;
    public Sprite Icon;
    public AudioClip SFX;
    public bool Crafted = false;
}
