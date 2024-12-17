using UnityEngine;

/// <summary>
/// Base class for item usage logic, implemented as a ScriptableObject.  
/// Different items will have their own ItemUseData assets.
/// </summary>
public abstract class ItemUseData : ScriptableObject
{
    public abstract void Use(Item item, Character character);
}
