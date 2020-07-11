using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
    public List<NPC> aggroNPCs;

    void OnTriggerEnter(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            foreach(var npc in aggroNPCs)
            {
                npc.DrawGun(pc);
            }
        }

        Destroy(this.gameObject);
    }
}
