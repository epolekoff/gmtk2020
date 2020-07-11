using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCharacter : MonoBehaviour
{
    //=================================================
    // Variables
    //=================================================
    public Rewired.Player PlayerInput { get { return ReInput.players.GetPlayer(0); } }
    public Rigidbody Rigidbody;
    public Ragdoll Ragdoll;
    public Animator Animator;
    public PlayerData PlayerData;
    public EzCamera PlayerCamera;
    public Transform RotatingRoot;


    private const float GroundCheckDistance = 0.1f;


    private bool m_onGround;
    private bool m_isJumpSquatting;
    private Vector3 m_jumpVelocity;
    private Vector3 m_movementVelocity;

    //=================================================
    // Unity Functions
    //=================================================

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        Ragdoll.SetEnabled(false);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        CheckGround();
        HandleInput();
    }

    //=================================================
    // Internal Functions
    //=================================================

    //-------------------------------------------------------------------
    /// <summary>
    /// Move the player
    /// </summary>
    private void HandleInput()
    {
        Vector2 moveVector = new Vector2(PlayerInput.GetAxis("Horizontal"), PlayerInput.GetAxis("Vertical"));

        // Add force based on the camera facing direction.
        Vector3 forwardVector = Vector3.ProjectOnPlane(PlayerCamera.transform.forward, Vector3.up);
        Vector3 rightVector = Vector3.ProjectOnPlane(PlayerCamera.transform.right, Vector3.up);
        //Rigidbody.AddForce(moveVector.x * rightVector * PlayerData.MoveSpeed);
        //Rigidbody.AddForce(moveVector.y * forwardVector * PlayerData.MoveSpeed);

        // If the player is not inputting a direction, slow the character down
        if (m_onGround)
        {
            m_movementVelocity.x = m_movementVelocity.x + (-1 * Mathf.Sign(m_movementVelocity.x) * PlayerData.Friction);
            if (Mathf.Abs(m_movementVelocity.x) <= PlayerData.Friction) { m_movementVelocity.x = 0; }
            m_movementVelocity.z = m_movementVelocity.z + (-1 * Mathf.Sign(m_movementVelocity.z) * PlayerData.Friction);
            if (Mathf.Abs(m_movementVelocity.z) <= PlayerData.Friction) { m_movementVelocity.z = 0; }
        }

        // Move the player.
        float moveSpeed = m_onGround ? PlayerData.MoveSpeed : PlayerData.AirMoveSpeed;
        m_movementVelocity += moveVector.x * rightVector * moveSpeed * Time.deltaTime;
        m_movementVelocity += moveVector.y * forwardVector * moveSpeed * Time.deltaTime;

        // Clamp the velocity at max.
        if(m_movementVelocity.magnitude > PlayerData.MaxVelocity)
        {
            m_movementVelocity.x *= (PlayerData.MaxVelocity / m_movementVelocity.magnitude);
            m_movementVelocity.z *= (PlayerData.MaxVelocity / m_movementVelocity.magnitude);
        }

        // Update the animation variables.
        if (m_movementVelocity.magnitude > 0.01f)
        {
            Animator.SetBool("Running", true);

            // Keep the player facing forward while running
            RotatingRoot.LookAt(RotatingRoot.transform.position + m_movementVelocity, Vector3.up);
        }
        else
        {
            Animator.SetBool("Running", false);
        }

        // Check for Jump.
        if (PlayerInput.GetButtonDown("Jump"))
        {
            if (!m_isJumpSquatting && m_onGround)
            {
                m_isJumpSquatting = true;
                StartCoroutine(JumpAfterDelay());
            }
        }

        // Apply Forces
        Rigidbody.velocity = new Vector3(
            m_movementVelocity.x,
            Rigidbody.velocity.y,
            m_movementVelocity.z);
    }

    //-------------------------------------------------------------------
    private IEnumerator JumpAfterDelay()
    {
        // Update the animator
        Animator.SetTrigger("Jump");
        Animator.SetBool("OnGround", false);

        yield return new WaitForSeconds(PlayerData.DelayBeforeJump);
        Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, PlayerData.JumpVelocity, Rigidbody.velocity.z);
        m_isJumpSquatting = false;
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Check if this fighter is on the ground or not.
    /// </summary>
    private void CheckGround()
    {
        // Don't check if you're on the ground when moving upwards.
        if (Rigidbody.velocity.y > 0.001f)
        {
            m_onGround = false;
            Animator.SetBool("OnGround", m_onGround);
            return;
        }

        bool wasOnGround = m_onGround;

        // Check if there is a stage or platform beneath the fighter.
        //Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, GroundCheckDistance, GetGroundLayerMask());
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position + (Vector3.up * 0.01f), Vector3.down, GroundCheckDistance, GetGroundLayerMask());

        // Check the colliders and make sure they are not triggers.
        // We are considered on the ground if we touched one that was not a trigger.
        m_onGround = false;
        foreach (RaycastHit hit in raycastHits)
        {
            if (!hit.collider.isTrigger)
            {
                m_onGround = true;
                break;
            }
        }
        Animator.SetBool("OnGround", m_onGround);

        // We just landed.
        if (!wasOnGround && m_onGround)
        {
            OnLand();
        }
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Reset some variables and set state related to landing.
    /// </summary>
    private void OnLand()
    {
        // Update the animator
        Animator.SetBool("OnGround", true);
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Get the ground layers.
    /// </summary>
    private int GetGroundLayerMask()
    {
        return LayerMask.GetMask(new[] { "Ground" });
    }
}
