using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public float moveTime = 0.2f;
    public Rigidbody2D body;

    private bool _isMoving;

    private List<RaycastHit2D> _hits = new();

    void Update()
    {
        if (!_isMoving)
        {
            var direction = GetDirection();
            if (direction != Vector2.zero)
                MoveInDirection(direction);
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        //var hit = Physics2D.Raycast(transform.position, direction, 1000, 1 << 7);

        body.Cast(direction, _hits);

        if (_hits.Count > 0)
        {
            int i = 0;
            foreach (var h in _hits)
            {
                Debug.Log($"Hit {i++} distance = {h.distance}");
            }
        }

        var closest = _hits.Select(hit => hit.distance).Min();

        Debug.Log($"Closest = {closest}");

        if (closest < 1.01f)
            return; // Can't move

        //var finalPosition = body.position + direction;
        //finalPosition = new Vector2(Mathf.Round(finalPosition.x), Mathf.Round(finalPosition.y));

        //Debug.Log($"Body position at the start: {body.position}");
        _isMoving = true;
        //body.DOMove(new Vector2(transform.position.x, transform.position.y) + direction, moveTime).SetEase(Ease.Linear).OnComplete(() =>        
        //body.DOMove(body.position + direction, moveTime).SetEase(Ease.Linear).OnComplete(() =>        
        transform.DOMove(transform.position + (Vector3)direction, moveTime).OnComplete(() =>
        {
            //body.MovePosition(finalPosition);
            _isMoving = false;
            //if (IsHoldingButton(direction))
            //    MoveInDirection(direction);
        });
    }

    private bool IsHoldingButton(Vector2 direction)
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if (direction.x > 0 && h > 0) return true;
        if (direction.x < 0 && h < 0) return true;
        if (direction.y > 0 && v > 0) return true;
        if (direction.y < 0 && v < 0) return true;

        return false;
    }

    private Vector2 GetDirection()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if (h != 0) return new Vector2(Mathf.Sign(h), 0);
        if (v != 0) return new Vector2(0, Mathf.Sign(v));

        return Vector2.zero;
    }
}
