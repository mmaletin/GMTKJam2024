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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var cat = collision.attachedRigidbody.gameObject.GetComponent<CatController>();
        if (cat != null)
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
}
