using UnityEngine;

public class HullPointGauge : MonoBehaviour
{
    private SpriteRenderer gauge;
    void Awake()
    {
        gauge = GetComponent<SpriteRenderer>();
    }

    public void UpdateHullPoint(float currentHP, float maxHP)
    {
        if(gauge == null)
        {
            Debug.LogWarning($"[HullPointGauge] gauge is not allocated");
            return;
        }
        float hullPointRatio = currentHP / maxHP;
        gauge.size = new Vector2(1, hullPointRatio);
    }
}