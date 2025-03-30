using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    public event Action<Vector2> OnMoveEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.action.performed += OnMotion;
        moveAction.action.canceled += OnStop;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMotion;
        moveAction.action.canceled -= OnStop;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMotion(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(moveInput);
    }

    public void OnStop(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        OnMoveEvent?.Invoke(moveInput);
    }

    private void Move()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
