using UnityEngine;

public class DockingPort : MonoBehaviour
{
    [SerializeField] private DockReceiver dockReceiver; // 부모 도킹리시버
    private Collider2D col; // 콜라이더
    public Collider2D Col => col;
    public GridPos mountingPoint => new GridPos(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));

    void Start()
    {
        dockReceiver = GetComponentInParent<DockReceiver>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        dockReceiver.NotifyTriggerEnter(this, collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        dockReceiver.NotifyTriggerExit(this, collision);
    }
}
