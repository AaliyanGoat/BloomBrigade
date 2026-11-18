using UnityEngine;

// The hive cannon "evolves": it shows a different model at Fire rate levels 3, 6 and 9
// (GameCatalog.evolutionLevels). Stage 0 is the starting hive.
public class HiveEvolution : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;   // one model per stage, all children of the cannon

    // Show only the model for the current stage.
    private void Start()
    {
        int stage = Mathf.Clamp(PlayerStats.EvolutionStage, 0, stages.Length - 1);
        for (int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == stage);
    }
}
