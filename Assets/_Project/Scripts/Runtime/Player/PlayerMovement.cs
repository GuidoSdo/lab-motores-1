using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Resuelve el movimiento, el salto y la rotacion del jugador a partir del input.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    // ================
    // CONFIG
    //=================

    [Header("Movement")]
    [Tooltip("Velocidad base de desplazamiento del jugador.")]
    [SerializeField] private float _normalSpeed = 5f;
    [Tooltip("Velocidad de desplazamiento mientras el sprint esta activo.")]
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Rotate")]
    [Tooltip("Velocidad utilizada para rotar al jugador y la camara con el input.")]
    [SerializeField] private float _rotateSpeed = 100f;

    [Header("Jump Specs")]
    [Tooltip("Altura maxima que alcanza el salto del jugador.")]
    [SerializeField] private float _jumpHeight = 2f;
    [Tooltip("Fuerza de gravedad aplicada al movimiento vertical del jugador.")]
    [SerializeField] private float _gravity = -12f;

    [Header("Camera")]
    [Tooltip("Transform de la camara del jugador que recibe la rotacion vertical.")]
    [SerializeField] private Transform _cameraTransform;

    // ================
    // Variable Internas
    //=================

    private CharacterController _controller;

    private float _speed;
    private Vector2 _move;
    private float _rotate;
    private Vector2 _look;

    private float _verticalVelocity;
    private float _pitch;
    
    public bool _canControl = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _speed = _normalSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (_canControl)
        {
            HandleMovement();
            HandleRotation();
            HandleLook();
        }
    }

    // ================
    // Movimiento
    //=================

    private void HandleMovement()
    {
        Vector3 move = transform.forward * _move.y + transform.right * _move.x;

        move = move.normalized * _speed;

        if (IsGrounded() && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += _gravity * Time.deltaTime;

        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float rotation = _rotate * _rotateSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void HandleLook()
    {
        float mouseX = _look.x * _rotateSpeed * Time.deltaTime;
        float mouseY = _look.y * _rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -80f, 80f);

        _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    // ================
    // Ground BOOL
    //=================
    private bool IsGrounded() => _controller.isGrounded;

    // ================
    // Unity Events
    //=================

    public void OnMovement(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        _rotate = context.ReadValue<float>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            _speed = _sprintSpeed;
        }

        if (context.canceled)
        {
            _speed = _normalSpeed;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * 2f * -_gravity);
        }
    }

    public void OnCameraMonster(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }

        if (context.canceled)
        {
        }
    }
}
