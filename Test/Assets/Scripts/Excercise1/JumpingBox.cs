using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class JumpingBox : MonoBehaviour
{
    [SerializeField]
    public float m_jumpForce;
    [SerializeField]
    private bool groundDetectionRelativeToWorld = false;
    [SerializeField]
    private float floorDetectionOffset = 0.1f;

    private float m_scaledJumpForce;

    private bool m_isGrounded;
    
    private InputAction jumpAction;
    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody == null)
            Debug.LogError("JumpingBox.Awake() : Rigidbody not found. Please add one.");

    }

    private void Start()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        
        PauseScreenManager pauseManager = FindFirstObjectByType<PauseScreenManager>(FindObjectsInactive.Include);
        m_scaledJumpForce = m_jumpForce * pauseManager.JumpSlider.value;
        pauseManager.JumpSlider.onValueChanged.AddListener(OnJumpSliderChange);
    }

    public void Update()
    {
        CheckIfGrounded();

        if (m_isGrounded && jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    private void CheckIfGrounded()
    {
        Vector3 down;

        if (groundDetectionRelativeToWorld)
        {
            down = -Vector3.up;
            down.Scale(transform.localScale * 0.5f);
            down = down + down.normalized * floorDetectionOffset;
        }
        else
        {
            down = -transform.up;
            down.Scale(transform.localScale * 0.5f);
            down = down + down.normalized * floorDetectionOffset;
        }
        Ray ray = new Ray(transform.position, down);
        //RaycastHit hit;

        Debug.DrawRay(transform.position, down.normalized, Color.cyan, down.magnitude);
        m_isGrounded = Physics.Raycast(ray, down.magnitude, LayerMask.GetMask("Ground"));
    }

    public void Jump()
    {
        m_rigidBody.AddForce(transform.up * m_scaledJumpForce, ForceMode.Impulse);
        m_isGrounded = false;
    }

    public void OnJumpSliderChange(float value)
    {
        m_scaledJumpForce = m_jumpForce * value;
    }
}
