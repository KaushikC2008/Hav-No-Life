using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DropManager : MonoBehaviour
{
    [Header("Reroll Settings")]
    [SerializeField] private int maxRerolls = 5;

    [Header("Fallback Reward")]
    [SerializeField] private int fallbackGold = 100;
    [SerializeField] private int fallbackHealthPotions = 2;
    [SerializeField] private ConsumableData healthPotion;

    private static DropManager _instance;
    public static DropManager Instance
    {
        get
        {
            // If the instance is somehow null, search the scene for it automatically!
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<DropManager>();

                // If it's still not in the scene, spawn one automatically out of thin air!
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("DropManager_AutoCreated");
                    _instance = singletonObject.AddComponent<DropManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GiveDrops(List<DropData> drops)
    {
        if (drops == null || drops.Count == 0)
            return;

        DropData selectedDrop = drops[Random.Range(0, drops.Count)];

        TryGiveDrop(selectedDrop, drops, 0);
    }

    public string GiveSingleDropAndGetText(List<DropData> drops)
    {
        if (drops == null || drops.Count == 0)
            return "";

        // 1. Gather all items that successfully pass their percentage roll
        List<DropData> successfulDrops = new List<DropData>();
        foreach (var drop in drops)
        {
            if (drop != null && drop.DoesDrop())
            {
                successfulDrops.Add(drop);
            }
        }

        if (successfulDrops.Count == 0)
            return "";

        // 2. Separate successful drops into accessories and consumables for priority handling
        List<DropData> successfulAccessories = new List<DropData>();
        List<DropData> successfulConsumables = new List<DropData>();

        foreach (var drop in successfulDrops)
        {
            if (drop.dropType == DropData.DropType.Accessory && drop.accessory != null)
            {
                // Optional safety: skip if player already owns it so it doesn't try to award a duplicate
                if (GameManager.Instance?.PlayerData?.ownedAccessories != null &&
                    !GameManager.Instance.PlayerData.ownedAccessories.Contains(drop.accessory))
                {
                    successfulAccessories.Add(drop);
                }
            }
            else if (drop.dropType == DropData.DropType.Consumable && drop.consumable != null)
            {
                successfulConsumables.Add(drop);
            }
        }

        DropData selectedDrop = null;

        // 3. Priority Rule: If any accessory won its roll, pick randomly from those accessories!
        if (successfulAccessories.Count > 0)
        {
            selectedDrop = successfulAccessories[Random.Range(0, successfulAccessories.Count)];
        }
        // 4. Fallback Rule: If no accessories won, pick randomly from successful consumables instead
        else if (successfulConsumables.Count > 0)
        {
            selectedDrop = successfulConsumables[Random.Range(0, successfulConsumables.Count)];
        }

        if (selectedDrop == null)
            return "";

        // 5. Grant the final chosen item
        int amount = selectedDrop.GetAmount();

        if (selectedDrop.dropType == DropData.DropType.Accessory && selectedDrop.accessory != null)
        {
            GameManager.Instance.PlayerData.ownedAccessories.Add(selectedDrop.accessory);
            return $"\n Found Accessory: {selectedDrop.accessory.accessoryName}!";
        }
        else if (selectedDrop.dropType == DropData.DropType.Consumable && selectedDrop.consumable != null)
        {
            GiveConsumable(selectedDrop.consumable, amount);
            return $"\n Found Item: {selectedDrop.consumable.itemName} x{amount}";
        }

        return "";
    }

    private void TryGiveDrop(DropData drop, List<DropData> allDrops, int rerollCount)
    {
        if (drop == null)
            return;
        if (!drop.DoesDrop())
            return;

        if (drop.dropType == DropData.DropType.Accessory)
        {
            if (drop.accessory == null)
                return;
            if (GameManager.Instance.PlayerData.ownedAccessories.Contains(drop.accessory))
            {
                Debug.Log("Already owns " + drop.accessory.accessoryName + ". Rerolling...");
                if (rerollCount >= maxRerolls)
                {
                    GiveFallbackReward();
                    return;
                }
                DropData rerolledDrop = allDrops[Random.Range(0, allDrops.Count)];
                TryGiveDrop(rerolledDrop, allDrops, rerollCount + 1);
                return;
            }
            GameManager.Instance.PlayerData.ownedAccessories.Add(drop.accessory);
            Debug.Log("Obtained accessory: " + drop.accessory.accessoryName);
            return;
        }

        if (drop.dropType == DropData.DropType.Consumable)
        {
            if (drop.consumable == null)
                return;
            int amount = drop.GetAmount();


            // FIXED: Added this line so consumables are actually added to your inventory!
            GiveConsumable(drop.consumable, amount);

            Debug.Log("Obtained consumable: " + drop.consumable.itemName + " x" + amount);
        }
    }

    private void GiveConsumable(ConsumableData consumable, int amount)
    {
        ConsumableEntry existingItem =
            GameManager.Instance.PlayerData.consumables.Find(
                x => x.item == consumable
            );

        if (existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            ConsumableEntry newItem =
                new ConsumableEntry();

            newItem.item = consumable;
            newItem.amount = amount;

            GameManager.Instance.PlayerData.consumables.Add(
                newItem
            );
        }
        Debug.Log("Obtained " + amount + "x " + consumable.itemName);
    }

    private void GiveFallbackReward()
    {
        GameManager.Instance.PlayerData.currentGold += fallbackGold;

        GiveConsumable(healthPotion, fallbackHealthPotions);

        Debug.Log("Fallback reward: +" + fallbackGold + " Gold and +" + fallbackHealthPotions + " Health Potions");
    }

    public string GiveChestDropsAndGetText(List<DropData> drops)
    {
        if (drops == null || drops.Count == 0)
            return "The chest was empty!";

        // 1. Gather all items that successfully pass their percentage roll
        List<DropData> successfulDrops = new List<DropData>();
        foreach (var drop in drops)
        {
            if (drop != null && drop.DoesDrop())
            {
                successfulDrops.Add(drop);
            }
        }

        if (successfulDrops.Count == 0)
            return "The chest was empty!";

        // 2. Separate successful drops into accessories and consumables
        List<DropData> successfulAccessories = new List<DropData>();
        List<DropData> successfulConsumables = new List<DropData>();

        foreach (var drop in successfulDrops)
        {
            if (drop.dropType == DropData.DropType.Accessory && drop.accessory != null)
            {
                // Optional safety: skip if player already owns it
                if (GameManager.Instance?.PlayerData?.ownedAccessories != null &&
                    !GameManager.Instance.PlayerData.ownedAccessories.Contains(drop.accessory))
                {
                    successfulAccessories.Add(drop);
                }
            }
            else if (drop.dropType == DropData.DropType.Consumable && drop.consumable != null)
            {
                successfulConsumables.Add(drop);
            }
        }

        List<DropData> finalChosenDrops = new List<DropData>();

        // 3. Rule: Max amount of accessories is 1
        if (successfulAccessories.Count > 0)
        {
            // Pick one random accessory from the successful ones
            DropData chosenAccessory = successfulAccessories[Random.Range(0, successfulAccessories.Count)];
            finalChosenDrops.Add(chosenAccessory);
        }

        // Shuffle the successful consumables so we pick a random mix if there are more than needed
        successfulConsumables = successfulConsumables.OrderBy(x => Random.value).ToList();

        // 4. Rule: Max amount of different items is 3 total
        int remainingSlots = 3 - finalChosenDrops.Count;
        for (int i = 0; i < successfulConsumables.Count && finalChosenDrops.Count < 3; i++)
        {
            finalChosenDrops.Add(successfulConsumables[i]);
        }

        if (finalChosenDrops.Count == 0)
            return "The chest was empty!";

        // 5. Grant the chosen items and build the return text message
        string rewardMessage = "Found Items:\n";
        foreach (var selectedDrop in finalChosenDrops)
        {
            int amount = selectedDrop.GetAmount();
            if (selectedDrop.dropType == DropData.DropType.Accessory && selectedDrop.accessory != null)
            {
                GameManager.Instance.PlayerData.ownedAccessories.Add(selectedDrop.accessory);
                rewardMessage += $"- Accessory: {selectedDrop.accessory.accessoryName}\n";
            }
            else if (selectedDrop.dropType == DropData.DropType.Consumable && selectedDrop.consumable != null)
            {
                GiveConsumable(selectedDrop.consumable, amount);
                rewardMessage += $"- {selectedDrop.consumable.itemName} x{amount}\n";
            }
        }

        return rewardMessage;
    }
}
