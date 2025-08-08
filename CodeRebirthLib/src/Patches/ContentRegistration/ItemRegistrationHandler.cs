using System.Collections.Generic;
using System.Linq;
using CodeRebirthLib.ConfigManagement;
using CodeRebirthLib.Data;
using UnityEngine;

namespace CodeRebirthLib.Patches.ContentRegistration;
static class ItemRegistrationHandler
{
    private static readonly List<Item> _allNewItems = [];
    static readonly Dictionary<string, List<RegistrationSettings<Item>>> _itemsToInject = [];
    static readonly List<ShopItemRegisterationSettings> _shopItemsToInject = [];
    private static readonly Dictionary<SpawnableItemWithRarity, RegistrationSettings<Item>> _itemSettingsMap = [];

    internal static void Init()
    {
        On.StartOfRound.Awake += StartOfRound_Awake;
        On.Terminal.Awake += Terminal_Awake;
        On.RoundManager.SpawnScrapInLevel += RoundManager_SpawnScrapInLevel;
    }

    private static void Terminal_Awake(On.Terminal.orig_Awake orig, Terminal self)
    {
        TerminalKeyword infoKeyword = self.terminalNodes.allKeywords.First(keyword => keyword.word == "info");
        TerminalKeyword buyKeyword = self.terminalNodes.allKeywords.First(keyword => keyword.word == "buy");
        TerminalKeyword confirmPurchaseKeyword = self.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "confirm");
        TerminalKeyword denyPurchaseKeyword = self.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "deny");
        TerminalNode cancelPurchaseNode = buyKeyword.compatibleNouns[0].result.terminalOptions[1].result; // TODO, I use these a couple times, maybe i should have em stored somewhere in LethalContent?

        foreach (ShopItemRegisterationSettings shopItemSettings in _shopItemsToInject)
        {
            self.buyableItemsList = self.buyableItemsList.Append(shopItemSettings.Item).ToArray();
            shopItemSettings.Item.creditsWorth = shopItemSettings.Cost;
            string simplifiedItemName = shopItemSettings.Item.itemName.Replace(" ", "-");

            TerminalNode? orderedItemNode = shopItemSettings.OrderedItemNode;
            if (orderedItemNode == null)
            {
                orderedItemNode = ScriptableObject.CreateInstance<TerminalNode>();
                orderedItemNode.name = $"{shopItemSettings.Item.itemName.Replace(" ", "-")}OrderedItemNode";
                orderedItemNode.displayText = $"Ordered [variableAmount] {shopItemSettings.Item.itemName}. Your new balance is [playerCredits].\n\nOur contractors enjoy fast, free shipping while on the job! Any purchased items will arrive hourly at your approximate location.\r\n\r\n";
                orderedItemNode.clearPreviousText = true;
                orderedItemNode.maxCharactersToType = 15;
            }
            orderedItemNode.buyItemIndex = self.buyableItemsList.Length - 1;
            orderedItemNode.isConfirmationNode = false;
            orderedItemNode.itemCost = shopItemSettings.Item.creditsWorth;
            orderedItemNode.playSyncedClip = 0;

            TerminalNode? orderRequestNode = shopItemSettings.OrderRequestNode;
            if (orderRequestNode == null)
            {
                orderRequestNode = ScriptableObject.CreateInstance<TerminalNode>();
                orderRequestNode.name = $"{simplifiedItemName}OrderRequestNode";
                orderRequestNode.displayText = $"You have requested to order {shopItemSettings.Item.itemName}. Amount: [variableAmount].\nTotal cost of items: [totalCost].\n\nPlease CONFIRM or DENY.\r\n\r\n";
                orderRequestNode.clearPreviousText = true;
                orderRequestNode.maxCharactersToType = 35;
            }
            orderRequestNode.buyItemIndex = self.buyableItemsList.Length - 1;
            orderRequestNode.isConfirmationNode = true;
            orderRequestNode.overrideOptions = true;
            orderRequestNode.itemCost = shopItemSettings.Item.creditsWorth;

            orderRequestNode.terminalOptions =
            [
                new()
                {
                    noun = confirmPurchaseKeyword,
                    result = orderedItemNode
                },
                new()
                {
                    noun = denyPurchaseKeyword,
                    result = cancelPurchaseNode
                }
            ];

            TerminalKeyword buyItemKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            buyItemKeyword.name = simplifiedItemName.ToLowerInvariant();
            buyItemKeyword.word = simplifiedItemName.ToLowerInvariant();
            buyItemKeyword.isVerb = false;
            buyItemKeyword.compatibleNouns = null;
            buyItemKeyword.specialKeywordResult = null;
            buyItemKeyword.defaultVerb = buyKeyword;
            buyItemKeyword.accessTerminalObjects = false;

            CompatibleNoun compatibleRequestNoun = new()
            {
                noun = buyItemKeyword,
                result = orderRequestNode
            };
            buyKeyword.compatibleNouns = buyKeyword.compatibleNouns.Append(compatibleRequestNoun).ToArray();

            TerminalNode? itemInfo = shopItemSettings.ItemInfoNode;
            if (itemInfo == null)
            {
                itemInfo = ScriptableObject.CreateInstance<TerminalNode>();
                itemInfo.name = $"{simplifiedItemName}InfoNode";
                itemInfo.displayText = $"[No information about this object was found.]\n\n";
                itemInfo.clearPreviousText = true;
                itemInfo.maxCharactersToType = 25;
            }

            self.terminalNodes.allKeywords = self.terminalNodes.allKeywords.Append(buyItemKeyword).ToArray();

            CompatibleNoun itemInfoCompatibleNoun = new()
            {
                noun = buyItemKeyword,
                result = itemInfo
            };
            infoKeyword.compatibleNouns = infoKeyword.compatibleNouns.Append(itemInfoCompatibleNoun).ToArray();

            CodeRebirthLibPlugin.ExtendedLogging($"Added {shopItemSettings.Item.itemName} to terminal (Item price: {orderRequestNode.itemCost}, Item Index: {orderRequestNode.buyItemIndex}, Terminal Keyword: {buyItemKeyword.word})");
        }
        orig(self);
    }

    internal static void AddItemToShop(ShopItemRegisterationSettings settings)
    {
        _shopItemsToInject.Add(settings);
        AddItemToAllList(settings.Item);
    }

    internal static void AddItemForLevel(string levelName, RegistrationSettings<Item> settings)
    {
        if (!_itemsToInject.TryGetValue(levelName, out List<RegistrationSettings<Item>> items))
        {
            items = new();
        }

        items.Add(settings);
        _itemsToInject[levelName] = items;
        AddItemToAllList(settings.Value);
    }

    internal static void AddItemToAllList(Item item)
    {
        if (!_allNewItems.Contains(item))
            _allNewItems.Add(item);
    }

    private static void StartOfRound_Awake(On.StartOfRound.orig_Awake orig, StartOfRound self)
    {
        orig(self);
        foreach (SelectableLevel level in StartOfRound.Instance.levels)
        {
            List<RegistrationSettings<Item>> items = [];

            if (_itemsToInject.TryGetValue("All", out List<RegistrationSettings<Item>> globalItems))
                items.AddRange(globalItems);

            if (_itemsToInject.TryGetValue(ConfigManager.GetLLLNameOfLevel(level.name), out List<RegistrationSettings<Item>> moonSpecificItems))
                items.AddRange(moonSpecificItems);

            foreach (RegistrationSettings<Item> item in items)
            {
                SpawnableItemWithRarity spawnDef = new SpawnableItemWithRarity
                {
                    spawnableItem = item.Value,
                    rarity = item.RarityProvider.GetWeight() // get an inital weight, incase a mod doesn't use any special code.
                };
                level.spawnableScrap.Add(spawnDef);
                _itemSettingsMap[spawnDef] = item;
            }
        }

        foreach (Item item in _allNewItems)
        {
            self.allItemsList.itemsList.Add(item);
        }

        _itemsToInject.Clear();
    }

    private static void RoundManager_SpawnScrapInLevel(On.RoundManager.orig_SpawnScrapInLevel orig, RoundManager self)
    {
        foreach (SpawnableItemWithRarity scrapWithRarity in self.currentLevel.spawnableScrap)
        {
            if (!_itemSettingsMap.TryGetValue(scrapWithRarity, out RegistrationSettings<Item> settings))
                continue;

            // update weights just before spawning scrap
            scrapWithRarity.rarity = settings.RarityProvider.GetWeight();
        }
        orig(self);
    }
}