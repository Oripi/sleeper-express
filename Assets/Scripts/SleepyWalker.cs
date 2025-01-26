using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class SleepyWalker : MonoBehaviour
{
    private static readonly int WakeUpTrigger = Animator.StringToHash("WakeUp");

    [Header("Settings")]
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float minTimeToUpdateNextVelocity;
    [SerializeField] private float maxTimeToUpdateNextVelocity;
    [SerializeField] private int numHitsToWakeUp = 3;
    [SerializeField] private LayerMask sleepWalkerLayerMask;
    [SerializeField] private bool isAsleepOnStart = false;
    [SerializeField] private float timeToWakeUp = 5;
    [SerializeField] private LayerMask bubbleLayer;
    [SerializeField] private float awakeTimeToLose = 5;
    [SerializeField] private AnimationCurve awakeAngryCurve;
    [SerializeField] private Color angryColor;
    
    [Header("References")]
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onWakeUp;
    [SerializeField] private UnityEvent onBubbled;
    [SerializeField] private UnityEvent onSpawn;

    private Rigidbody2D _rb;
    private Vector2 _currentVelocity;
    private float _accTimeSinceLastVelocityUpdate;
    private readonly ContactPoint2D[] _contacts = new ContactPoint2D[1];
    private SleepyWalkerPool _pool;
    private bool _isSleeping;
    private int _currentHits;
    private float _currentSleepTimer;
    private float _currentAwakeTime;
    private bool _shouldStartWalking = true;
    private Tween _angryColorTween;
    private Color _initialColor;

    private bool IsReady { get; set; }
    public float TimeToUpdateVelocity { get; set; }
    public Vector2 CurrentVelocity
    {
        get => _currentVelocity; 
        set => _currentVelocity = value;
    }

    public float TimeToWakeUp
    {
        get => timeToWakeUp;
        set => timeToWakeUp = value;
    }

    public int NumHitsToWakeUp
    {
        get => numHitsToWakeUp;
        set => numHitsToWakeUp = value;
    }

    public bool ShouldGetAngry { get; set; } = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _isSleeping = isAsleepOnStart;
        animator.PlayInFixedTime("Sleep");
        _initialColor = spriteRenderer.color;
    }

    private void Update()
    {
        if(!IsReady) return;
        
        UpdateNextVelocity();
        UpdateSleepTimer();
        UpdateAwakeTimer();
        UpdateSleepState();
    }

    public void FixedUpdate()
    {
        if(!IsReady) return;
        
        UpdateVelocity();
        UpdateLookDirection();
    }

    private void UpdateLookDirection()
    {
        spriteRenderer.flipX = _currentVelocity.x < 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (LayerUtils.IsInLayerMask(other.gameObject, sleepWalkerLayerMask))
        {
            OnHitByOtherSleepWalker();
        }

        if (_isSleeping) return;
        
        var numContacts = other.GetContacts(_contacts);
        if (numContacts <= -1) return;

        var firstContactNormal = _contacts[0].normal;
        if (Mathf.Abs(firstContactNormal.x) > Mathf.Epsilon)
        {
            _currentVelocity.x = -_currentVelocity.x;
        }

        if (Mathf.Abs(firstContactNormal.y) > Mathf.Epsilon)
        {
            _currentVelocity.y = -_currentVelocity.y;
        }

        UpdateVelocity();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerUtils.IsInLayerMask(other.gameObject, bubbleLayer))
        {
            if (_isSleeping) return;
            
            animator.PlayInFixedTime("Bubbled");
            onBubbled.Invoke();
            GoToSleep();
        }
    }
    
    private void UpdateAwakeTimer()
    {
        if(_isSleeping || !ShouldGetAngry) return;
        
        _currentAwakeTime += Time.deltaTime;

        if (_currentAwakeTime >= awakeTimeToLose)
        {
            GameManager.Instance.LoseGame();
        }
    }
    
    private void UpdateSleepTimer()
    {
        if (!_isSleeping) return;

        _currentSleepTimer += Time.deltaTime;
    }
    
    private void UpdateSleepState()
    {
        if (!_isSleeping) return;

        if (_currentSleepTimer >= timeToWakeUp)
        {
            WakeUp();
        }
    }

    private void OnHitByOtherSleepWalker()
    {
        if(!_isSleeping) return;
        
        _currentHits++;
        if (_currentHits >= numHitsToWakeUp)
        {
            WakeUp();
        }
    }

    private void WakeUp()
    {
        _isSleeping = false;
        _shouldStartWalking = false;
        animator.SetTrigger(WakeUpTrigger);
        onWakeUp.Invoke();
    }
    
    private void UpdateNextVelocity()
    {
        _accTimeSinceLastVelocityUpdate += Time.deltaTime;
        if (_accTimeSinceLastVelocityUpdate >= TimeToUpdateVelocity)
        {
            _currentVelocity = new Vector2(Random.Range(-maxXVelocity, maxXVelocity),
                Random.Range(-maxYVelocity, maxYVelocity));
            _accTimeSinceLastVelocityUpdate = TimeToUpdateVelocity % _accTimeSinceLastVelocityUpdate;
            TimeToUpdateVelocity = Random.Range(minTimeToUpdateNextVelocity, maxTimeToUpdateNextVelocity);
        }
    }

    private void UpdateVelocity()
    {
        _rb.linearVelocity = _isSleeping || !_shouldStartWalking ? Vector2.zero : _currentVelocity;
    }

    public void SetPool(SleepyWalkerPool pool)
    {
        _pool = pool;
    }

    public void Release()
    {
        _pool.ReleaseSleepWalker(this);
        _pool = null;
    }

    public void ActivateWalker()
    {
        IsReady = true;
        _shouldStartWalking = false;
        gameObject.SetActive(true);
        onSpawn.Invoke();
    }

    public void PlaySpawnAnimation()
    {
        animator.PlayInFixedTime("Spawn");
    }

    public void OnWakeUpFinished()
    {
        _shouldStartWalking = true;
        _currentAwakeTime = 0;
        animator.PlayInFixedTime("Walk");
        GetAngry();
    }

    public void GetAngry()
    {
        ShouldGetAngry = true;
        _angryColorTween = Tween.Color(spriteRenderer, angryColor, awakeTimeToLose, awakeAngryCurve);
    }

    private void GoToSleep()
    {
        _isSleeping = true;
        _currentHits = 0;
        _currentSleepTimer = 0;

        if (_angryColorTween.isAlive)
        {
            _angryColorTween.Stop();
        }
        
        
        Tween.Color(spriteRenderer, _initialColor, 0.3f);
    }
}