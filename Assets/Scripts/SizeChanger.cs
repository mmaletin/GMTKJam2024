using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    public enum SizeChangerType
    {
        IncreaseAndDecrease,
        Increase,
        Decrease
    }

    public SizeChangerType sizeChangerType;

    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private HashSet<Rigidbody2D> _pressingRigidbodies = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wasPressed = _pressingRigidbodies.Count > 0;

        _pressingRigidbodies.Add(collision.attachedRigidbody);
        UpdateSprite();

        var cat = collision.attachedRigidbody.gameObject.GetComponent<CatController>();
        if (cat != null && !wasPressed)
        {
            switch (sizeChangerType)
            {
                case SizeChangerType.IncreaseAndDecrease:
                    cat.ToggleSize(transform.position);
                    break;
                case SizeChangerType.Increase:
                    cat.TryIncreasingSize(transform.position);
                    break;
                case SizeChangerType.Decrease:
                    cat.TryDecreasingSize(transform.position);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _pressingRigidbodies.Remove(collision.attachedRigidbody);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = _pressingRigidbodies.Count == 0 ? normalSprite : pressedSprite;
    }
}
