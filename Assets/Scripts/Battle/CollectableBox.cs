using UnityEngine;

/// <summary>
/// 可拾取箱子
/// </summary>
public class CollectableBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<Unit>();
        if (unit)
        {

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("CollectableBox collides with non-Unit!");
        }
    }
}