using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class CatController : MonoBehaviour, IObjectWithSize
{
    public float scaleAnimationDuration = 0.5f;
    public float moveTime = 0.2f;
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    public Sprite[] sprites;

    public LayerMask smallCatCollision;
    public LayerMask bigCatCollision;

    private bool _isBig = true;
    private bool _isMoving;

    private bool _sheduledScaleAnimation;
    private Vector3 _sizeTogglerPosition;

    private List<RaycastHit2D> _hits = new();

    private ContactFilter2D _contactFilter;

    private bool _hasIgnoredDirection;
    private Vector2 _ignoredDirection;

    public ObjectSize Size => _isBig ? ObjectSize.Big : ObjectSize.Small;

    private void Awake()
    {
        UpdateContactFilter();

        var direction = GetDirection();
        if (direction != Vector2.zero)
        {
            _hasIgnoredDirection = true;
            _ignoredDirection = direction;
        }
    }

    private void UpdateContactFilter()
    {
        _contactFilter = new ContactFilter2D()
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = _isBig ? bigCatCollision : smallCatCollision
        };
    }

    void Update()
    {
        if (_hasIgnoredDirection)
        {
            var direction = GetDirection();
            if (direction == _ignoredDirection)
            {
                return;
            }
            else
            {
                _hasIgnoredDirection = false;
            }
        }

        if (!_isMoving)
        {
            var direction = GetDirection();
            if (direction != Vector2.zero)
                MoveInDirection(direction);
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        body.Cast(direction, _contactFilter, _hits, float.MaxValue);

        //if (_hits.Count > 0)
        //{
        //    int i = 0;
        //    foreach (var h in _hits)
        //    {
        //        Debug.Log($"Hit {i++} distance = {h.distance}");
        //    }
        //}

        var closest = _hits.Count > 0 ? _hits.Select(hit => hit.distance).Min() : float.MaxValue;

        if (closest < 1.01f)
        {
            using var rocksPoolObject = ListPool<Rock>.Get(out var rocks);

            foreach (var hit in _hits)
            {
                if (hit.distance > 1.01f)
                    continue;

                var rock = hit.transform.GetComponent<Rock>();
                if (rock == null || !rock.CanBeMoved(_isBig ? ObjectSize.Big : ObjectSize.Small, direction))
                    return;

                rocks.Add(rock);
            }

            foreach (var rock in rocks)
            {
                rock.Move(direction, moveTime);
            }
        }

        _isMoving = true;      
        transform.DOMove(transform.position + (Vector3)direction, moveTime).SetLink(gameObject).OnComplete(() =>
        {
            if (_sheduledScaleAnimation)
            {
                ToggleScaleAnimation();
            }
            else
            {
                _isMoving = false;
            }
        });
    }

    private void ToggleScaleAnimation()
    {
        _sheduledScaleAnimation = false;

        // TODO Use cat size
        _isBig = !_isBig;
        spriteRenderer.sprite = _isBig ? sprites[1] : sprites[0];
        var targetScale = _isBig ? Vector3.one * 2 : Vector3.one;
        var targetPosition = _isBig ? _sizeTogglerPosition : _sizeTogglerPosition - new Vector3(-0.5f, -0.5f, 0);

        UpdateContactFilter();

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("music_switch", _isBig ? 0 : 1);

        transform.DOScale(targetScale, scaleAnimationDuration).SetLink(gameObject);
        transform.DOMove(targetPosition, scaleAnimationDuration).SetLink(gameObject).OnComplete(() =>
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

    public void TryIncreasingSize(Vector3 position)
    {
        if (!_isBig)
            ToggleSize(position);
    }

    public void TryDecreasingSize(Vector3 position)
    {
        if (_isBig)
            ToggleSize(position);
    }
}
