using System;
using UnityEngine;

// Card M5: health for the hive and the nest. Raises events; other scripts show bars, flashes and shakes.
public class BaseHealth : MonoBehaviour
{
    [Tooltip("Ticked on the nest (bees damage it). Unticked on the hive hit zone (wasps damage it).")]
    [SerializeField] private bool damagedByBees;

    public event Action<int, int> OnHealthChanged;   // (current, max)
    public event Action OnDamaged;
    public event Action OnDestroyed;

    public bool DamagedByBees => damagedByBees;
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    // Lower health, never below 0. At 0, tell the GameManager (once).
    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnDamaged?.Invoke();

        if (CurrentHealth == 0)
            OnDestroyed?.Invoke();
    }

    // Restart: full health from the level's value.
    public void ResetForNewRun(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
