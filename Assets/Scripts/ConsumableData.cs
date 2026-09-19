using UnityEngine;

public enum ConsumableEffectType
{
    Heal,
    RestoreMana,
    BuffAttack,
    BuffSpecialAttack,
    BuffDefense,
    DebuffAttack,
    DebuffDefense
}

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class ConsumableData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    
    [TextArea]
    public string description;
    
    public Sprite icon;

    [Header("Effect")]
    public ConsumableEffectType effectType;
    public int amount;

    [Header("Duration")]
    public int duration;
}