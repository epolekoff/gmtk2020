using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
    public List<NPC> aggroNPCs;
    bool aggro = false;

    void OnTriggerStay(Collider col)
    {
        // Check if anyone died.
        if (!GameManager.Instance.WasNPCKilled)
        {
            return;
        }

        if(aggro)
        {
            return;
        }
        Debug.Log("Aggro!");

        // Get the player and aggro the npcs.
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            foreach(var npc in aggroNPCs)
            {
                npc.DrawGun(pc);
            }

            Destroy(this.gameObject);
            aggro = true;
        }

    }
}
