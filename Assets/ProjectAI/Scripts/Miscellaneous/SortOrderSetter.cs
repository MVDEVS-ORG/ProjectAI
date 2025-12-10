using UnityEngine;

public class SortOrderSetter : MonoBehaviour
{
    private void OnEnable()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -(int)transform.position.y;
    }
}
