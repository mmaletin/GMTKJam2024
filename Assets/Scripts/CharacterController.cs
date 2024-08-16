using DG.Tweening;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveTime = 0.2f;

    private bool _isMoving;
    private float _characterWidth = 1f;

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
        var hit = Physics2D.Raycast(transform.position, direction, 1000, 1 << 7);

        if (hit.distance < _characterWidth / 2 + 0.01f)
            return; // Can't move

        _isMoving = true;
        transform.DOMove(transform.position + (Vector3)direction, moveTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            _isMoving = false;
            if (IsHoldingButton(direction))
                MoveInDirection(direction);
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
