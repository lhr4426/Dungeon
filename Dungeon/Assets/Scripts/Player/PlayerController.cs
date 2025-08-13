using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    private float originalMoveSpeed;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public LayerMask platformLayerMask;
    private bool onWall;

    private Vector2 curMoveInput;

    [Header("Look Settings")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    public bool canLook = true;
    public float lookSensitivity;
    public Transform tpsCameraContainer;
    public bool isFPS;
    public GameObject fpsEquipCamera;
    public LayerMask fpsMask;
    public LayerMask tpsMask;


    private Vector2 mouseDelta;
    private float camCurXRot;
    private float colliderWidthHalf;
    private float colliderHeightHalf;


    // Private Settings
    public PlayerInput playerInput;
    [HideInInspector] public Rigidbody rb;
    public Action inventory;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // 여기 있는게 너무 맘에 안드는데 방법이 없나... 
        // 생성 순서 때문에 어쩔 수 없이 여기다 뒀는데 너무 거슬림
        playerInput.Player.Interact.started
            += PlayerManager.Player.interaction.OnInteraction;

        Cursor.lockState = CursorLockMode.Locked;
        isFPS = true;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        colliderHeightHalf = capsuleCollider.height / 2;
        colliderWidthHalf = capsuleCollider.radius / 2;

        originalMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerInput = new();

        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMoveStop;

        playerInput.Player.Jump.started += OnJump;

        playerInput.Player.Look.performed += OnLook;
        playerInput.Player.Look.canceled += OnLookStop;

        playerInput.Player.Inventory.started += OnInventory;

        playerInput.Player.ChangeView.started += OnChangeView;

        playerInput.Player.Enable();
    }
    
    

    private void LateUpdate()
    {
        if(canLook) Look();
    }

    private void FixedUpdate()
    {
        onWall = OnWall();
        rb.useGravity = !onWall;
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
        if (!onWall)
        {
            Vector3 dir = transform.forward * curMoveInput.y
            + transform.right * curMoveInput.x;
            dir *= moveSpeed;
            // y축 속도는 그대로 유지
            dir.y = rb.velocity.y;
            rb.velocity = dir;
        }
        else
        {
            Vector3 dir = transform.up * curMoveInput.y
                + transform.right * curMoveInput.x;
            dir *= moveSpeed;
            dir.z = rb.velocity.z;
            rb.velocity = dir;
        }
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

        if(isFPS) cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        else tpsCameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }


    void OnJump(InputAction.CallbackContext context)
    {
        if(IsGrounded() || onWall)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        if (IsPlatformShooter(out RaycastHit hit))
        {
            Debug.Log("플랫폼에 닿음");
            if(hit.collider.TryGetComponent(out PlatformShooter shooter))
            {
                shooter.Shoot();
            }
        }
    }

    bool IsPlatformShooter(out RaycastHit hit)
    {
        hit = new RaycastHit();

        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], out hit, colliderHeightHalf + 0.01f, platformLayerMask))
            {
                return true;
            }
        } 
        return false;
    }

    bool IsGrounded()
    {
        
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], colliderHeightHalf + 0.01f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    bool OnWall()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position, Vector3.forward),
            new Ray(transform.position, Vector3.back),
            new Ray(transform.position, Vector3.right),
            new Ray(transform.position, Vector3.left)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], colliderWidthHalf + 0.5f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    void LookToggle()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    void OnInventory(InputAction.CallbackContext context)
    {
        inventory?.Invoke();
        LookToggle();
    }

    void OnChangeView(InputAction.CallbackContext context)
    {
        ChangeView();
    }


    public void ChangeView()
    {
        if (isFPS)
        {
            Camera.main.transform.SetParent(tpsCameraContainer.transform, false);
            fpsEquipCamera.SetActive(false);
            Camera.main.cullingMask = tpsMask;
        }
        else
        {
            Camera.main.transform.SetParent(cameraContainer.transform, false);
            fpsEquipCamera.SetActive(true);
            Camera.main.cullingMask = fpsMask;
        }
            
        isFPS = !isFPS;
    }

    public void ChangeSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }
}
