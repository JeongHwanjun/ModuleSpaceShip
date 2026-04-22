using UnityEngine;

public abstract class BaseMonobehaviour : MonoBehaviour
{
    public abstract void OnClickStart();
    public abstract void OnClickCancel(Collider2D hit);
}
