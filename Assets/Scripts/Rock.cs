using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public CatSize minimumSize;
    public Rigidbody2D body;

    private List<RaycastHit2D> _hits = new();

    private ContactFilter2D _contactFilter;

    private void Awake()
    {
        _contactFilter = new ContactFilter2D()
        {
            useTriggers = false
        };
    }

    public bool CanBeMoved(CatSize catSize, Vector2 direction)
    {
        if (catSize < minimumSize)
            return false;

        body.Cast(direction, _contactFilter, _hits, float.MaxValue);

        var closest = _hits.Select(hit => hit.distance).Min();
        return closest > 1.01f;
    }

    public void Move(Vector2 direction, float moveTime)
    {
        transform.DOMove(transform.position + (Vector3)direction, moveTime).SetLink(gameObject);
    }
}
