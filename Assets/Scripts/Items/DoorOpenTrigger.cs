using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    public Door Door;
    public SpeechBubble SpeechBubble;
    public string SpeechBubbleText;

    private bool m_done;

    /// <summary>
    /// Trigger
    /// </summary>
    void OnTriggerStay(Collider col)
    {
        if(m_done)
        {
            return;
        }

        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        Hand handInCollider = col.gameObject.GetComponentInParent<Hand>();
        if (pc != null &&
            handInCollider == null)
        {
            // Wait for the speech bubble to complete before opening the door.
            if (SpeechBubble.IsComplete())
            {
                // Check the hands for coins.
                foreach (var hand in pc.Hands)
                {
                    // Spend the coin and open the door.
                    if (hand.IsHoldingItem() && hand.HeldItem is Coin)
                    {
                        ((Coin)hand.HeldItem).SpendCoin();
                        Door.SetOpen(true);
                        SpeechBubble.Show(false, "");
                        Destroy(this.gameObject);
                        m_done = true;
                        return;
                    }
                }
            }

            // Show the speech bubble.
            SpeechBubble.Show(true, SpeechBubbleText);
        }
    }

    /// <summary>
    /// Hide the speech bubble.
    /// </summary>
    void OnTriggerExit(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        Hand handInCollider = col.gameObject.GetComponentInParent<Hand>();
        if (pc != null &&
            handInCollider == null)
        {
            // Show the speech bubble.
            SpeechBubble.Show(false, "");
        }
    }
}
