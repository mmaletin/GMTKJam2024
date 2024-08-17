using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public float scaleAnimationDuration = 0.5f;
    public float moveTime = 0.2f;
    public Rigidbody2D body;

    private bool _isBig = true;
    private bool _isMoving;

    private bool _sheduledScaleAnimation;
    private Vector3 _sizeTogglerPosition;

    private List<RaycastHit2D> _hits = new();

    private ContactFilter2D _contactFilter;

    private void Awake()
    {
        _contactFilter = new ContactFilter2D()
        {
            useTriggers = false
        };
    }

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

        body.Cast(direction, _contactFilter, _hits, float.MaxValue);

        //if (_hits.Count > 0)
        //{
        //    int i = 0;
        //    foreach (var h in _hits)
        //    {
        //        Debug.Log($"Hit {i++} distance = {h.distance}");
        //    }
        //}

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
            if (_sheduledScaleAnimation)
            {
                ToggleScaleAnimation();
            }
            else
            {
                _isMoving = false;
            }

            //body.MovePosition(finalPosition);
            //if (IsHoldingButton(direction))
            //    MoveInDirection(direction);
        });
    }

    private void ToggleScaleAnimation()
    {
        _sheduledScaleAnimation = false;

        _isBig = !_isBig;
        var targetScale = _isBig ? Vector3.one * 2 : Vector3.one;
        var targetPosition = _isBig ? _sizeTogglerPosition : _sizeTogglerPosition - new Vector3(-0.5f, -0.5f, 0);

        transform.DOScale(targetScale, scaleAnimationDuration);
        transform.DOMove(targetPosition, scaleAnimationDuration).OnComplete(() =>
        {
            _isMoving = false;
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

    public void ToggleSize(Vector3 position)
    {
        _sheduledScaleAnimation = true;
        _sizeTogglerPosition = position;
    }
}
