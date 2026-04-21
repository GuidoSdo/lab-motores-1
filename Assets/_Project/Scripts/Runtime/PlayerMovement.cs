using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovement : MonoBehaviour
{
    // ================
    // CONFIG
    //=================

    [Header("Movement")]
    [SerializeField] private float _normalSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Rotate")]
    [SerializeField] private float _rotateSpeed = 100f;

    [Header("Jump Specs")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = -9.8f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("RayCastConfig")]
    [SerializeField] private float _rayLength;
    
    [Header("SphereCastConfig")]
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _groundLayer;


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
        HandleMovement();
        HandleRotation();
        HandleLook();
        
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
        float mouseX = _look.x * _rotateSpeed *Time.deltaTime;
        float mouseY =  _look.y * _rotateSpeed * Time.deltaTime;

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
           
           _verticalVelocity = Mathf.Sqrt(_jumpForce *2f* -_gravity);
           
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _rayLength)&& hit.collider.CompareTag("Interactivo"))
            {
                Debug.Log("puede interactuar");

            }
            
            
            
        }
      
    }

    

  
}
