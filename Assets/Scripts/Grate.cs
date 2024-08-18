﻿using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class Grate : MonoBehaviour
{
    public bool inverted;

    public SpriteRenderer spriteRenderer;
    public Sprite upSprite;
    public Sprite downSprite;

    public Rigidbody2D blockingRigidbody;

    private HashSet<Rigidbody2D> _pressingRigidbodies = new();
    private bool _buttonState;

    public StudioEventEmitter grate_sound;

    private bool _previousIsDown;

    private void Start()
    {
        _previousIsDown = inverted;
        UpdateState(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == blockingRigidbody)
            return;

        _pressingRigidbodies.Add(collision.attachedRigidbody);        
        UpdateState();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == blockingRigidbody)
            return;

        _pressingRigidbodies.Remove(collision.attachedRigidbody);
        UpdateState();
    }

    private void UpdateState(bool skipSound = false)
    {
        var isDown = _pressingRigidbodies.Count > 0 || (inverted ? !_buttonState : _buttonState);

        spriteRenderer.sprite = isDown ? downSprite : upSprite;
        blockingRigidbody.gameObject.SetActive(!isDown);
        
        if(_previousIsDown != isDown)
        {
            _previousIsDown = isDown;
            if (!skipSound)
                grate_sound.Play();
        }
    }

    public void SetButtonState(bool value)
    {
        _buttonState = value;
        UpdateState();
    }
}
