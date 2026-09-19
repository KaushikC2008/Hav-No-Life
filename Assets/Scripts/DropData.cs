using System;
using UnityEngine;

[Serializable]
public class DropData
{
    public enum DropType
    {
        Accessory,
        Consumable
    }

    [Header("Drop")]
    public DropType dropType;

    [Header("Item")]
    public Accessaries accessory;
    public ConsumableData consumable;

    [Header("Drop Chance")]
    [Range(0f, 100f)]
    public float dropChance = 100f;

    [Header("Amount")]
    public int minAmount = 1;
    public int maxAmount = 1;

    public int GetAmount()
    {
        return UnityEngine.Random.Range(minAmount, maxAmount + 1);
    }

    public bool DoesDrop()
    {
        return UnityEngine.Random.Range(0f, 100f) <= dropChance;
    }
}