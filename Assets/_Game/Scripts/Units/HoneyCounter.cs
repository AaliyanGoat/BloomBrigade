using System;
using UnityEngine;

// Counts honey for this run (section 4.4). Spending and saving come in Assignment 4.
public class HoneyCounter : MonoBehaviour
{
    [SerializeField] private GameTuning tuning;
    [SerializeField] private BaseHealth nest;

    public event Action<int> OnHoneyChanged;
    public int Honey { get; private set; }

    private int quartersPaid;   // how many quarters of nest health have already paid out

    // Watch the nest so each quarter of health lost pays honey.
    private void OnEnable() { nest.OnHealthChanged += HandleNestHealthChanged; }
    private void OnDisable() { nest.OnHealthChanged -= HandleNestHealthChanged; }

    // +honeyPerQuarter for every new quarter of the nest's health that is gone.
    private void HandleNestHealthChanged(int current, int max)
    {
        if (max <= 0) return;
        int quartersLost = (max - current) * 4 / max;
        while (quartersPaid < quartersLost)
        {
            quartersPaid++;
            Add(tuning.honeyPerQuarter);
        }
    }

    // Win bonus: winBonusPerLevel x level number.
    public void AddWinBonus(int levelNumber)
    {
        Add(tuning.winBonusPerLevel * levelNumber);
    }

    // Add honey and tell the HUD.
    public void Add(int amount)
    {
        Honey += amount;
        OnHoneyChanged?.Invoke(Honey);
    }

    // Restart: back to zero.
    public void ResetForNewRun()
    {
        Honey = 0;
        quartersPaid = 0;
        OnHoneyChanged?.Invoke(Honey);
    }
}
