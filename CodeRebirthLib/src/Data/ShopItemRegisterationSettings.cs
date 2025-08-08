namespace CodeRebirthLib.Data;

class ShopItemRegisterationSettings(Item item, int cost, TerminalNode? orderRequestNode, TerminalNode? orderedItemNode, TerminalNode? itemInfoNode)
{
    public Item Item => item;
    public int Cost => cost;
    public TerminalNode? OrderRequestNode => orderRequestNode;
    public TerminalNode? OrderedItemNode => orderedItemNode;
    public TerminalNode? ItemInfoNode => itemInfoNode;
}