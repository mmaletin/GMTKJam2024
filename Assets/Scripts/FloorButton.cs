using UnityEngine;

public class FloorButton : MonoBehaviour
{
    public bool isOn;
    public UnityEventBool onButtonStateChanged;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //var cat = collision.attachedRigidbody.gameObject.GetComponent<CatController>();
        //if (cat != null)
        //{
        //    isOn = !isOn;
        //    onButtonStateChanged?.Invoke(isOn);
        //}

        isOn = !isOn;
        onButtonStateChanged?.Invoke(isOn);
    }
}
