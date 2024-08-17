using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var cat = collision.attachedRigidbody.gameObject.GetComponent<CatController>();
        if (cat != null)
        {
            cat.ToggleSize(transform.position);
        }
    }
}
