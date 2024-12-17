
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public void RestoreHealth(int amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
        Debug.Log("Character healed, current health: " + Health);
    }
}
