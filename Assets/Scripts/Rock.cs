using DG.Tweening;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rock : MonoBehaviour, IObjectWithSize
{
    public ObjectSize minimumSize;
    public Rigidbody2D body;

    public LayerMask collision;

    private List<RaycastHit2D> _hits = new();

    private ContactFilter2D _contactFilter;

    // Technically not the same thing, but whatever
    public ObjectSize Size => minimumSize;

    public StudioEventEmitter move_sound;

    private void Awake()
    {
        _contactFilter = new ContactFilter2D()
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = collision
        };
    }

    public bool CanBeMoved(ObjectSize catSize, Vector2 direction)
    {
        if (catSize < minimumSize)
            return false;

        body.Cast(direction, _contactFilter, _hits, float.MaxValue);

        var closest = _hits.Count > 0 ? _hits.Select(hit => hit.distance).Min() : float.MaxValue;
        return closest > 1.01f;
    }

    public void Move(Vector2 direction, float moveTime)
    {
        transform.DOMove(transform.position + (Vector3)direction, moveTime).SetLink(gameObject);
        move_sound.Play();
    }
}
