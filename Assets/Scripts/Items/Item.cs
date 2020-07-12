using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Transform GrabPoint;
    public Collider PickUpRange;
    public Rigidbody Rigidbody;

    public bool IsButton;

    public bool Consumed { get; set; }

    private const float TimeBeforePickupAfterDrop = 5f;

    protected Hand m_heldHand;
    protected float m_itemCooldownTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_itemCooldownTimer > 0)
        {
            m_itemCooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Check if already held.
    /// </summary>
    public bool IsHeld()
    {
        return m_heldHand != null;
    }

    /// <summary>
    /// Grab item.
    /// </summary>
    public bool GrabItem(Hand heldHand)
    {
        if(IsButton)
        {
            PushButton(heldHand);
            return false;
        }

        m_heldHand = heldHand;

        // Set this item to be held by the hand.
        transform.SetParent(heldHand.ItemBone.transform);

        // Move the item such that the Grab Point is at the center of the hand.
        transform.localPosition = Vector3.zero - GrabPoint.localPosition;
        transform.localRotation = Quaternion.identity;

        // Disable the pickup range collider.
        PickUpRange.enabled = false;

        // Turn off the rigidbody.
        Rigidbody.isKinematic = true;

        // Set the grabbed item.
        heldHand.GrabItem(this);

        // Fire the event.
        OnGrabbed();

        return true;
    }

    protected virtual void OnGrabbed()
    {

    }

    /// <summary>
    /// Drop item.
    /// </summary>
    public void DropItem()
    {
        m_heldHand = null;

        // Drop it.
        transform.SetParent(null);

        // Turn on the rigidbody.
        Rigidbody.isKinematic = false;

        // Fire the event.
        OnDropped();

        StartCoroutine(CountdownUntilCanBePickedUpAfterDropping());
    }

    protected virtual void OnDropped()
    {

    }

    /// <summary>
    /// Push a button instead of grabbing an item.
    /// </summary>
    protected virtual void PushButton(Hand hand)
    {

    }

    /// <summary>
    /// Wait a bit, then re-enable.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountdownUntilCanBePickedUpAfterDropping()
    {
        yield return new WaitForSeconds(TimeBeforePickupAfterDrop);

        // Re-enable the collider to pickup the item.
        PickUpRange.enabled = true;
    }
}
