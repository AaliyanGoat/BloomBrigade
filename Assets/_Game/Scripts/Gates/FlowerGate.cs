using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Cards M3 and M7: a flower multiplies the bees that pass through it, until its petals run out.
// Rule: each BONUS bee costs 1 petal. At 0 petals the flower closes for closedTime seconds.
public class FlowerGate : MonoBehaviour
{
    [Header("Tuning")]
    [SerializeField] private float spawnSpread = 0.6f;   // random X offset of bonus bees
    [SerializeField] private float closedTime = 6f;
    [SerializeField] private float closedScale = 0.6f;   // shrinks to this while closed
    [SerializeField] private float lowPetalFraction = 0.25f;

    [Header("Look")]
    [SerializeField] private Renderer flowerRenderer;
    [SerializeField] private ScalePunch flowerPunch;
    [SerializeField] private ScalePunch labelPunch;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private Image petalRing;            // radial fill shows petals left
    [SerializeField] private Color openColor = new Color(1f, 0.45f, 0.75f);
    [SerializeField] private Color closedColor = Color.gray;
    [SerializeField] private Color ringColor = Color.white;
    [SerializeField] private Color ringLowColor = Color.red;

    private FlowerSetup setup;
    private BeeSpawner spawner;
    private int petalsMax;        // from the level + the Bloom upgrade
    private int petalsLeft;
    private bool isOpen;
    private float reopenTime;
    private MaterialPropertyBlock block;

    // Called once by LevelLoader with this flower's settings from the LevelData.
    public void Setup(FlowerSetup flowerSetup, BeeSpawner beeSpawner)
    {
        setup = flowerSetup;
        spawner = beeSpawner;
        block = new MaterialPropertyBlock();
        label.text = setup.type == GateType.Multiply ? "x" + setup.value : "+" + setup.value;
        petalRing.gameObject.SetActive(!setup.neverWilts);   // no ring if it never wilts
        ResetForNewRun();
    }

    // A bee entered: give bonus bees once per bee, if the flower is open.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bee bee) || bee.IsDead || bee.HasUsed(this)) return;
        bee.MarkUsed(this);
        if (!isOpen) return;   // closed: the bee passes with no bonus

        int owed = setup.type == GateType.Multiply ? setup.value - 1 : setup.value;
        if (!setup.neverWilts) owed = Mathf.Min(owed, petalsLeft);

        int given = 0;
        for (int i = 0; i < owed; i++)
            if (spawner.SpawnBonusBee(bee.transform.position, this, spawnSpread)) given++;

        if (given > 0)
        {
            labelPunch.Punch();
            FeedbackManager.Instance.FlowerBonus(bee.transform.position, given);
        }

        if (setup.neverWilts) return;
        petalsLeft -= given;
        UpdateRing();
        if (petalsLeft <= 0) Close();
    }

    // Reopen when the closed timer runs out.
    private void Update()
    {
        if (!isOpen && Time.time >= reopenTime) Open(true);
    }

    // Out of petals: grey, small, no bonus for closedTime seconds.
    private void Close()
    {
        isOpen = false;
        reopenTime = Time.time + closedTime;
        SetColor(closedColor);
        flowerPunch.SetBaseScale(closedScale);
        FeedbackManager.Instance.FlowerClosed(transform.position);
    }

    // Full petals again; pops back to full size (with a chime unless it is a restart).
    private void Open(bool withFeedback)
    {
        isOpen = true;
        petalsMax = setup.petalsMax + PlayerStats.BonusPetals;
        petalsLeft = petalsMax;
        SetColor(openColor);
        flowerPunch.SetBaseScale(1f);
        UpdateRing();
        if (!withFeedback) return;
        flowerPunch.Punch();
        FeedbackManager.Instance.FlowerOpened(transform.position);
    }

    // Ring fill = petals left / petalsMax; turns red under lowPetalFraction.
    private void UpdateRing()
    {
        float fraction = petalsMax > 0 ? (float)petalsLeft / petalsMax : 1f;
        petalRing.fillAmount = fraction;
        petalRing.color = fraction < lowPetalFraction ? ringLowColor : ringColor;
    }

    // Tint the flower without creating a new material.
    private void SetColor(Color color)
    {
        block.SetColor("_BaseColor", color);
        flowerRenderer.SetPropertyBlock(block);
    }

    // Restart: open, full petals.
    public void ResetForNewRun()
    {
        Open(false);
    }
}
