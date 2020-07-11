using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandState
{
    Free,
    MovingToItem,
    MovingToPosition,
    Resting,
    UsingItem
}

public class Hand : MonoBehaviour
{
    public PlayerCharacter Owner;
    public Rigidbody Rigidbody;
    public Transform ItemBone;
    public Item HeldItem { get; set; }
    public HandState CurrentState { get; set; }
    public int Index { get; set; }

    private const float TimeBeforeNewItemGrab = 5f;

    private float m_timeSinceLastItemGrab = 1000f;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        CurrentState = HandState.Free;
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        m_timeSinceLastItemGrab += Time.deltaTime;
    }

    /// <summary>
    /// When can this hand grab a new item.
    /// </summary>
    public bool CanGrabNewItem()
    {
        return m_timeSinceLastItemGrab >= TimeBeforeNewItemGrab;
    }

    /// <summary>
    /// Check if holding an item.
    /// </summary>
    /// <returns></returns>
    public bool IsHoldingItem()
    {
        return HeldItem != null;
    }

    /// <summary>
    /// Drop the item.
    /// </summary>
    public void DropItem()
    {
        if(HeldItem == null)
        {
            return;
        }

        HeldItem.DropItem();
        HeldItem = null;

        // Set state
        SetPhysicsEnabled(true);
        CurrentState = HandState.Free;
    }

    /// <summary>
    /// Called when an item is grabbed.
    /// </summary>
    public void GrabItem(Item item)
    {
        HeldItem = item;
    }

    /// <summary>
    /// Turn on/off the physics.
    /// </summary>
    public void SetPhysicsEnabled(bool enabled)
    {
        if(!enabled)
        {
            Rigidbody.velocity = Vector3.zero;
        }

        Rigidbody.useGravity = enabled;
        Rigidbody.isKinematic = !enabled;
    }
}
