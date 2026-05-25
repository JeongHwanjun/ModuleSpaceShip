using UnityEngine;

public class ThrusterFlame : MonoBehaviour
{
    private Thruster thruster;

    void Awake()
    {
        thruster = GetComponentInParent<Thruster>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        thruster.OnFlameEnter(collision);
    }

    void OD(Collider2D collision)
    {
        thruster.OnFlameExit(collision);
    }
}