using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChestData", menuName = "ScriptableObjects/ChestData")]
public class ChestData : ScriptableObject
{
    [Header("Chest Configuration")]
    public string chestTypeName;
    
    [Header("Visuals")]
    public Sprite chestSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Possible Drops")]
    public List<DropData> drops;
}