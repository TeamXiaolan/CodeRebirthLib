using UnityEngine;

namespace CodeRebirthLib.ContentManagement.Items;

public class ShopItemPreset
{
    [field: SerializeField]
    public TerminalNode OrderRequestNode { get; private set; }

    [field: SerializeField]
    public TerminalNode OrderedItemNode { get; private set; }

    [field: SerializeField]
    public TerminalNode ItemInfoNode { get; private set; }
}