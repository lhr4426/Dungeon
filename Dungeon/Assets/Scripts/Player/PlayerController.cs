using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float jumpPower;
    public LayerMask groundLayerMask;

    private Vector2 curMoveInput;

    [Header("Look Settings")]
    public Transform CameraContainer;
    public float minXLook;
    public float maxXLook;
    public bool canLook = true;
    public float lookSensitivity;

    private Vector2 mouseDelta;
    private float camCurXRot;

    // Private Settings
    PlayerInput playerInput;
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnEnable()
    {
        playerInput = new();

        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMoveStop;

        playerInput.Player.Jump.started += OnJump;

        playerInput.Player.Look.performed += OnLook;
        playerInput.Player.Look.canceled += OnLookStop;

        playerInput.Player.Enable();
        playerInput.Change.Enable();

    }

    private void LateUpdate()
    {
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        curMoveInput = context.ReadValue<Vector2>();
    }

    void OnMoveStop(InputAction.CallbackContext context)
    {
        curMoveInput = Vector2.zero;
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMoveInput.y
            + transform.right * curMoveInput.x;
        dir *= moveSpeed;
        // y축 속도는 그대로 유지
        dir.y = rb.velocity.y;
        rb.velocity = dir;
    }

    void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    void OnLookStop(InputAction.CallbackContext context)
    {
        mouseDelta = Vector2.zero;
    }

    void Look()
    {         
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        CameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }


    void OnJump(InputAction.CallbackContext context)
    {
        if(IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + transform.up * 0.01f, Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    void OnInventory(InputAction.CallbackContext context)
    {

    }
}
