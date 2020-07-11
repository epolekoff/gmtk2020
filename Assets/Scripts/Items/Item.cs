using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Transform GrabPoint;
    public Collider PickUpRange;
    public Rigidbody Rigidbody;

    private const float TimeBeforePickupAfterDrop = 5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Grab item.
    /// </summary>
    public void GrabItem(Hand heldHand)
    {
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
    }

    protected virtual void OnGrabbed()
    {

    }

    /// <summary>
    /// Drop item.
    /// </summary>
    public void DropItem()
    {
        // Drop it.
        transform.SetParent(null);

        // Turn on the rigidbody.
        Rigidbody.isKinematic = false;

        StartCoroutine(CountdownUntilCanBePickedUpAfterDropping());
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
