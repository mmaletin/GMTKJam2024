using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class CatController : MonoBehaviour, IObjectWithSize
{
    private const float TouchControlsRadius = 100f;

    public float scaleAnimationDuration = 0.5f;
    public float moveTime = 0.2f;
    public float kittenMeowDelay = 0.7f;
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    public Sprite smallRightSprite;
    public Sprite smallUpSprite;
    public Sprite smallDownSprite;

    public Sprite bigRightSprite;
    public Sprite bigUpSprite;
    public Sprite bigDownSprite;

    public LayerMask smallCatCollision;
    public LayerMask bigCatCollision;

    [Layer] public int puddlesLayer;

    public bool isBig;
    private bool _isMoving;

    private bool _sheduledScaleAnimation;
    private Vector3 _sizeTogglerPosition;

    private List<RaycastHit2D> _hits = new();

    private ContactFilter2D _contactFilter;

    private bool _hasIgnoredDirection;
    private Vector2 _ignoredDirection;

    private Vector2 _lastDirection;

    public ObjectSize Size => isBig ? ObjectSize.Big : ObjectSize.Small;

    public StudioEventEmitter shrink;
    public StudioEventEmitter stretch;

    public StudioEventEmitter cat_footstep_small;
    public StudioEventEmitter cat_footstep_big;

    public StudioEventEmitter small_cat_voice;
    private float _lastKittenMeowTime;

    private Vector2 _touchOrigin;

    private void Awake()
    {
        Input.multiTouchEnabled = false;

        UpdateContactFilter();

        var direction = GetDirection();
        if (direction != Vector2.zero)
        {
            _hasIgnoredDirection = true;
            _ignoredDirection = direction;
        }

        UpdateMusic();
    }

    private void UpdateContactFilter()
    {
        _contactFilter = new ContactFilter2D()
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = isBig ? bigCatCollision : smallCatCollision
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
            {
                MoveInDirection(direction);
            }                          
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        _lastDirection = direction;
        UpdateSprite();

        body.Cast(direction, _contactFilter, _hits, 1.01f);

        if (_hits.Count > 0)
        {
            if (_hits.Where(h => h.transform.gameObject.layer == puddlesLayer).Any() && !isBig && Time.time > _lastKittenMeowTime + kittenMeowDelay)
            {
                _lastKittenMeowTime = Time.time;
                small_cat_voice.Play();
            }

            if (_hits.Count == 1)
            {
                var door = _hits[0].transform.GetComponentInParent<BreakableDoor>();
                if (door != null && Size >= door.requiredCatSize)
                {
                    door.Smash();
                    MoveInDirection(direction);
                    return;
                }
            }

            using var rocksPoolObject = ListPool<Rock>.Get(out var rocks);

            foreach (var hit in _hits)
            {
                var rock = hit.transform.GetComponent<Rock>();
                if (rock == null || !rock.CanBeMoved(isBig ? ObjectSize.Big : ObjectSize.Small, direction))
                    return;

                rocks.Add(rock);
            }

            foreach (var rock in rocks)
            {
                rock.Move(direction, moveTime);
            }
        }

        _isMoving = true;
        (isBig ? cat_footstep_big : cat_footstep_small).Play();

        spriteRenderer.transform.DOScaleY(1.1f, moveTime * 0.5f).SetLink(spriteRenderer.gameObject).SetEase(Ease.InOutQuad);
        spriteRenderer.transform.DOScaleY(1, moveTime * 0.5f).SetLink(spriteRenderer.gameObject).SetEase(Ease.InOutQuad).SetDelay(moveTime * 0.5f);
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
        isBig = !isBig;
        UpdateSprite();
        var targetScale = isBig ? Vector3.one * 2 : Vector3.one;
        var targetPosition = isBig ? _sizeTogglerPosition : _sizeTogglerPosition - new Vector3(-0.5f, -0.5f, 0);

        UpdateContactFilter();

        if (isBig)
        {
            stretch.Play();
        }
        else
        {
            shrink.Play();
        }

        UpdateMusic();

        transform.DOScale(targetScale, scaleAnimationDuration).SetLink(gameObject);
        transform.DOMove(targetPosition, scaleAnimationDuration).SetLink(gameObject).OnComplete(() =>
        {
            _isMoving = false;
        });
    }

    private void UpdateMusic()
    {
        RuntimeManager.StudioSystem.setParameterByName("music_switch", isBig ? 0 : 1);
    }

    private Vector2 GetDirection()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _touchOrigin = touch.position;
            }
            else
            {
                var delta = touch.position - _touchOrigin;

                if (delta.sqrMagnitude > TouchControlsRadius * TouchControlsRadius)
                {
                    return Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? new Vector2(Mathf.Sign(delta.x), 0) : new Vector2(0, Mathf.Sign(delta.y));
                }
            }
        }

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
        if (!isBig)
            ToggleSize(position);
    }

    public void TryDecreasingSize(Vector3 position)
    {
        if (isBig)
            ToggleSize(position);
    }

    private void UpdateSprite()
    {
        if (!isBig && (_lastDirection == Vector2.zero || _lastDirection == Vector2.down)) spriteRenderer.sprite = smallDownSprite;
        if (!isBig && _lastDirection == Vector2.up) spriteRenderer.sprite = smallUpSprite;
        if (!isBig && _lastDirection == Vector2.left) spriteRenderer.sprite = smallRightSprite;
        if (!isBig && _lastDirection == Vector2.right) spriteRenderer.sprite = smallRightSprite;

        if (isBig && (_lastDirection == Vector2.zero || _lastDirection == Vector2.down)) spriteRenderer.sprite = bigDownSprite;
        if (isBig && _lastDirection == Vector2.up) spriteRenderer.sprite = bigUpSprite;
        if (isBig && _lastDirection == Vector2.left) spriteRenderer.sprite = bigRightSprite;
        if (isBig && _lastDirection == Vector2.right) spriteRenderer.sprite = bigRightSprite;

        spriteRenderer.flipX = _lastDirection == Vector2.left;
    }
}
