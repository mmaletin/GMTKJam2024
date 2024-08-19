using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour
{
    public UnityEventBool onButtonStateChanged;

    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    public StudioEventEmitter press_sound;
    public StudioEventEmitter release_sound;

    public ObjectSize minimumSize;

    private HashSet<Rigidbody2D> _pressingRigidbodies = new();

    private bool _previousIsDown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wasPressed = _pressingRigidbodies.Count > 0;

        var objectWithSize = collision.gameObject.GetComponent<IObjectWithSize>();
        if (objectWithSize != null && objectWithSize.Size >= minimumSize)
        {
            _pressingRigidbodies.Add(collision.attachedRigidbody);
            UpdateSprite();

            if (!wasPressed)
            {
                press_sound.Play();
                onButtonStateChanged.Invoke(true);
                _previousIsDown = true;
            }
                         
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _pressingRigidbodies.Remove(collision.attachedRigidbody);
        UpdateSprite();
        if (_pressingRigidbodies.Count == 0)
        {          
            onButtonStateChanged.Invoke(false);

            if (_previousIsDown)
            {
                release_sound.Play();
                _previousIsDown = false;
            }
        }
            
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = _pressingRigidbodies.Count == 0 ? normalSprite : pressedSprite;
    }
}
