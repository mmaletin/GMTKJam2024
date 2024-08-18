using UnityEngine;

public class EndLevelButton : MonoBehaviour
{
    public Level level;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var cat = collision.attachedRigidbody.gameObject.GetComponent<CatController>();
        if (cat != null)
        {
            level.CompleteLevel();
        }
    }
}