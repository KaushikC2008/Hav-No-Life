using UnityEngine;
/*
Common Accessories
Beserker Ring
Iron Band
Health Charm
Quickstep Anklet
Apprentice Amulet
ViperFangAmulet
Mana Pearl
Sturdy Leather Bracer
Guardian's Shield-pin
Hunter's Monocle
*/
[CreateAssetMenu(fileName = "NewAccessory", menuName = "ScriptableObjects/AccessoryData")]
public class Accessaries : ScriptableObject
{
    [Header("Basic Info")]
    public string accessoryName;
    public Sprite icon;

    [TextArea]
    public string description;

    [Header("Stat Changes")]
    public int attackBonus;
    public int defenseBonus;
    public int maxHealthBonus;
    public int maxManaBonus;
    public int specialAttackBonus;
}