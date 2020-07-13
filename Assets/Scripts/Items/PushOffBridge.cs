using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushOffBridge : Item
{
    public NPC NpcToPushOffBridge;

    private const float PushForce = 1000f;

    /// <summary>
    /// Push the guy off the bridge.
    /// </summary>
    protected override void PushButton(Hand hand)
    {
        NpcToPushOffBridge.Die(otherNpcsCare: false);

        // Get the push vector.
        Vector3 handToNpc = transform.position - hand.transform.position;
        NpcToPushOffBridge.Ragdoll.AddForce(handToNpc.normalized * PushForce);

        AudioManager.Instance.PlaySound(AudioManager.Instance.PushOffBridgeSound);

        Destroy(gameObject, 3f);
    }
}
