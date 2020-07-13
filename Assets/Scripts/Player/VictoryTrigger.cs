using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    public Transform ConfettiRoot;

    /// <summary>
    /// Trigger victory.
    /// </summary>
    void OnTriggerEnter(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        Hand hand = col.gameObject.GetComponentInParent<Hand>();
        if (pc != null &&
            hand == null)
        {
            Victory();
        }
    }

    /// <summary>
    /// Do some victory things.
    /// </summary>
    private void Victory()
    {
        foreach(Transform confetti in ConfettiRoot)
        {
            confetti.gameObject.SetActive(true);
        }

        GameManager.Instance.GameCanvas.ShowVictory();
        AudioManager.Instance.PlaySound(AudioManager.Instance.VictorySound);
    }
}
