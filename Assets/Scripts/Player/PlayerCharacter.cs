﻿using System.Collections;
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
    public List<Hand> Hands;
    public List<Transform> HandRestingBones;
    public GameObject BloodFX;
    public bool IsMenuCharacter;

    private const float GroundCheckDistance = 0.1f;


    private bool m_onGround;
    private bool m_isJumpSquatting;
    private Vector3 m_jumpVelocity;
    private Vector3 m_movementVelocity;
    private int m_handItemIndex = 0;
    private int m_health = 5;
    private bool m_isDead = false;

    //=================================================
    // Unity Functions
    //=================================================

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        Ragdoll.SetEnabled(false);

        for(int i = 0; i < Hands.Count; i++)
        {
            Hands[i].Index = i;
        }

    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if(m_isDead)
        {
            return;
        }

        CheckGround();
        HandleInput();
    }

    /// <summary>
    /// Trigger checks.
    /// </summary>
    void OnTriggerEnter(Collider col)
    {
        if(m_isDead)
        {
            return;
        }

        Item item = col.GetComponentInParent<Item>();
        if (item != null && !item.IsHeld() && !item.Consumed)
        {
            OnApproachItem(item);
        }
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
        if (IsMenuCharacter)
        {
            return;
        }

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
            Vector3 newForward = Vector3.RotateTowards(RotatingRoot.transform.forward, m_movementVelocity, 0.1f, 0.1f);
            RotatingRoot.LookAt(RotatingRoot.transform.position + newForward, Vector3.up);
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.JumpSound);
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Check if this fighter is on the ground or not.
    /// </summary>
    private void CheckGround()
    {
        if(IsMenuCharacter)
        {
            return;
        }

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
        if(IsMenuCharacter)
        {
            return;
        }

        // Update the animator
        Animator.SetBool("OnGround", true);
        AudioManager.Instance.PlayLandSound();
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Get the ground layers.
    /// </summary>
    private int GetGroundLayerMask()
    {
        return LayerMask.GetMask(new[] { "Ground" });
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// Get the next hand.
    /// </summary>
    /// <returns></returns>
    private Hand GetNextAvailableHand()
    {
        Hand handToGrabItem = Hands[m_handItemIndex];

        // Check if the hand can grab this item.
        if (!handToGrabItem.CanGrabNewItem())
        {
            // Try the other hand.
            AdvanceHandIndex();
            handToGrabItem = Hands[m_handItemIndex];
            if (!handToGrabItem.CanGrabNewItem())
            {
                return null;
            }
        }

        // Increment the index again for next time.
        AdvanceHandIndex();
        return handToGrabItem;
    }

    //-------------------------------------------------------------------
    private void RollBackHandIndex()
    {
        m_handItemIndex--;
        if (m_handItemIndex < 0)
        {
            m_handItemIndex = Hands.Count-1;
        }
    }

    //-------------------------------------------------------------------
    private void AdvanceHandIndex()
    {
        m_handItemIndex++;
        if(m_handItemIndex == Hands.Count)
        {
            m_handItemIndex = 0;
        }
    }

    //-------------------------------------------------------------------
    /// <summary>
    /// When approaching an item, do something.
    /// </summary>
    private void OnApproachItem(Item item)
    {
        Hand handToGrabItem = GetNextAvailableHand();
        if (handToGrabItem == null)
        {
            return;
        }

        // Check if the hands are holding anything.
        if (handToGrabItem.IsHoldingItem())
        {
            handToGrabItem.DropItem();
        }

        // Start trying to grab the item.
        StartCoroutine(GrabItem(handToGrabItem, item));
    }

    /// <summary>
    /// Move the hand to grab the item.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrabItem(Hand hand, Item item)
    {
        hand.CurrentState = HandState.MovingToItem;

        // Kill all momentum.
        hand.SetPhysicsEnabled(false);

        // Move the hand towards the item.
        Vector3 handStartPosition = hand.transform.position;
        float grabTimer = 0;
        while (grabTimer < PlayerData.ItemGrabLerpTime)
        {
            // If the player leaves range, break out entirely.
            if(Vector3.Distance(transform.position, item.GrabPoint.position) > PlayerData.ItemGrabRange)
            {
                hand.CurrentState = HandState.Free;
                hand.SetPhysicsEnabled(true);
                yield break;
            }

            grabTimer += Time.deltaTime;
            float t = grabTimer / PlayerData.ItemGrabLerpTime;

            // Move the hand using physics, to knock things out of the way.
            hand.transform.position = Vector3.Lerp(handStartPosition, item.GrabPoint.position, t);

            yield return null;
        }

        // If the hand is not close to the item, give up.
        if(Vector3.Distance(hand.transform.position, item.GrabPoint.position) > 0.5f)
        {
            hand.CurrentState = HandState.Free;
            hand.SetPhysicsEnabled(true);
            yield break;
        }

        // Grab the item.
        bool itemGrabbed = item.GrabItem(hand);

        // If a button was pushed or the grab failed, free up the hand.
        if(!itemGrabbed)
        {
            hand.CurrentState = HandState.Free;
            hand.SetPhysicsEnabled(true);
            RollBackHandIndex();
            yield break;
        }

        // Move the hand back, with the item.
        hand.CurrentState = HandState.MovingToPosition;
        handStartPosition = hand.transform.position;
        grabTimer = 0;
        while (grabTimer < PlayerData.ItemGrabLerpTime)
        {
            grabTimer += Time.deltaTime;
            float t = grabTimer / PlayerData.ItemGrabLerpTime;

            // Move the hand using physics, to knock things out of the way.
            hand.transform.position = Vector3.Lerp(handStartPosition, HandRestingBones[hand.Index].position, t);

            yield return null;
        }
        
        // Move the hand into the bone.
        hand.transform.SetParent(HandRestingBones[hand.Index]);
        hand.transform.localPosition = Vector3.zero;
        hand.transform.localRotation = Quaternion.identity;
        hand.CurrentState = HandState.Resting;
    }

    /// <summary>
    /// Hit by bullet.
    /// </summary>
    public void OnHitByBullet(Vector3 direction)
    {
        // Play some blood.
        BloodFX.SetActive(true);
        BloodFX.GetComponent<ParticleSystem>().Play();

        // Check if dead.
        m_health--;
        if (m_health <= 0)
        {
            Die();
            Ragdoll.AddForce(direction.normalized * 500f);
        }
    }

    /// <summary>
    /// Die
    /// </summary>
    public void Die()
    {
        m_isDead = true;
        Ragdoll.SetEnabled(true);
        GameManager.Instance.PlayerDied();
        PlayerCamera.SetOrbitEnabled(false);
        PlayerCamera.enabled = false;

        foreach (Hand hand in Hands)
        {
            hand.DropItem();
            hand.SetPhysicsEnabled(true);
            hand.CurrentState = HandState.Free;
            hand.transform.SetParent(null);
        }
    }
}
