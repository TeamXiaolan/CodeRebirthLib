using System;

namespace CodeRebirthLib.Data;

[Flags]
public enum StoreType // Maybe a more creative name
{
    ShipUpgrade = 1 << 0,
    Decor = 1 << 1,
}

class UnlockableItemRegistrationSettings(UnlockableItem unlockableItem, int cost, StoreType storeType)
{
    public UnlockableItem UnlockableItem => unlockableItem;
    public int Cost => cost;
    public StoreType StoreType => storeType; // TODO use this
}