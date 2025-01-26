using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BubbleController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private UnityEvent<Vector2> onClicked;
    [SerializeField] private UnityEvent<Vector2> onReleased;
    [SerializeField] private GameObject bubbleGameObj;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AnimationCurve bubbleScaleCurve;
    
    private InputSystem_Actions _controls;
    private Action _lastClickAction = Action.Released;
    private bool _isPressed;
    private Vector3 _clickedPointInWorldSpace;
    private float _currentScaleTime;

    private void Start()
    {
        bubbleScaleCurve.postWrapMode = WrapMode.ClampForever;
        bubbleScaleCurve.preWrapMode = WrapMode.ClampForever;
    }

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new InputSystem_Actions();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPressed)
        {
            var currentScale = bubbleScaleCurve.Evaluate(_currentScaleTime);
            bubbleGameObj.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            _currentScaleTime += Time.deltaTime;
        }
    }

    #region IPlayerActions

    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
    }

    public void OnClickPoint(InputAction.CallbackContext context)
    {
        // note that this method is called multiple times (with correct isPressed)
        if (IsButtonDown(context))
        {
            if (_lastClickAction == Action.Clicked)
            {
                return;
            }
            
            _lastClickAction = Action.Clicked;
            onClicked.Invoke(context.action.ReadValue<Vector2>());
            return;
        }

        if (_lastClickAction == Action.Released)
        {
            return;
        }

        _lastClickAction = Action.Released;
        onReleased.Invoke(context.action.ReadValue<Vector2>());
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
    }

    public void OnNext(InputAction.CallbackContext context)
    {
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
    }

    #endregion

    private bool IsButtonDown(InputAction.CallbackContext context)
    {
        var action = context.action;
        // getting reverse value for isPressed...
        return !action.IsPressed();
    }
    
    private bool IsButtonUp(InputAction.CallbackContext context)
    {
        var action = context.action;
        return !action.IsPressed();
    }
    
    private enum Action
    {
        Clicked,
        Released
    }

    public void IsPressed(Vector2 value)
    {
        _isPressed = true;
        _currentScaleTime = 0;
        _clickedPointInWorldSpace = mainCamera.ScreenToWorldPoint(new Vector3(value.x, value.y, mainCamera.nearClipPlane));
        bubbleGameObj.transform.SetPositionAndRotation(_clickedPointInWorldSpace, quaternion.identity);
        bubbleGameObj.SetActive(true);
    }
    
    public void IsReleased(Vector2 value)
    {
        _isPressed = false;
        bubbleGameObj.SetActive(false);
    }
}
