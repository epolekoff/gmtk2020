using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string SpeechText;
    public SpeechBubble SpeechBubble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Check if a player gets close.
    /// </summary>
    void OnTriggerEnter(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            SpeechBubble.Show(true, SpeechText);
        }
    }

    /// <summary>
    /// When a player leaves.
    /// </summary>
    void OnTriggerExit(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            SpeechBubble.Show(false, SpeechText);
        }
    }
}
