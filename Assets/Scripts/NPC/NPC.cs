using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, ISlashable
{
    public string SpeechText;
    public string SpeechText_NPCDied;
    public SpeechBubble SpeechBubble;
    public Ragdoll Ragdoll;
    public GameObject DeathFX;
    public Gun Gun;
    public Collider CapsuleHitbox;

    public bool WaitForTextBeforeSlash = false;
    public bool CanEverBeSlashed = true;

    private const float SlashForce = 500f;
    private const float BulletAimHeight = 1f;
    private const float FireCooldownMax = 4f;
    private const float FireCooldownMin = 1f;

    private bool m_alive = true;
    private PlayerCharacter m_aggroPlayerCharacter;
    private float m_fireTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Ragdoll.SetEnabled(false);
        m_fireTimer = UnityEngine.Random.Range(0, FireCooldownMin);
    }

    // Update is called once per frame
    void Update()
    {
        // When dead, move the object to always line up with the ragdoll.
        if (!m_alive)
        {
            transform.position = Ragdoll.Rigidbodies[0].transform.position;
        }

        if (m_aggroPlayerCharacter != null && m_alive)
        {
            // Keep rotating to look at the player.
            Vector3 npcToPlayer = m_aggroPlayerCharacter.transform.position - transform.position;
            Vector3 forwardVector = Vector3.ProjectOnPlane(npcToPlayer, Vector3.up);
            transform.rotation = Quaternion.LookRotation(forwardVector, Vector3.up);

            // Check the timer.
            m_fireTimer -= Time.deltaTime;
            if(m_fireTimer <= 0)
            {
                m_fireTimer = UnityEngine.Random.Range(FireCooldownMin, FireCooldownMax);

                // Fire the gun.
                Vector3 bulletAimPoint = m_aggroPlayerCharacter.transform.position + (BulletAimHeight * Vector3.up);
                Vector3 gunToPlayer = bulletAimPoint - Gun.BulletSpawnPosition.position;
                Gun.Fire(gunToPlayer);
            }

            
        }
    }

    /// <summary>
    /// Check if a player gets close.
    /// </summary>
    void OnTriggerEnter(Collider col)
    {
        if(!m_alive)
        {
            return;
        }

        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null && SpeechText != String.Empty && m_aggroPlayerCharacter == null)
        {
            SpeechBubble.Show(true, SpeechText);
        }
    }

    /// <summary>
    /// When a player leaves.
    /// </summary>
    void OnTriggerExit(Collider col)
    {
        if (!m_alive)
        {
            return;
        }

        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            SpeechBubble.Show(false, "");
        }
    }

    /// <summary>
    /// Getter for the game object.
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// Check if this NPC can be slashed.
    /// </summary>
    /// <returns></returns>
    public bool CanBeSlashed()
    {
        if (SpeechBubble != null && WaitForTextBeforeSlash && !SpeechBubble.IsComplete())
        {
            return false;
        }

        return CanEverBeSlashed;
    }

    /// <summary>
    /// Get Slashed.
    /// </summary>
    public void OnSlashed(Vector3 direction)
    {
        Die();
        Ragdoll.AddForce(direction.normalized * SlashForce);
        DeathFX.SetActive(true);
        DeathFX.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Kill the NPC.
    /// </summary>
    public void Die()
    {
        if(!m_alive)
        {
            return;
        }
        m_alive = false;
        GameManager.Instance.WasNPCKilled = true;

        Ragdoll.SetEnabled(true);
        Ragdoll.gameObject.transform.SetParent(null);
        CapsuleHitbox.enabled = false;
        Destroy(SpeechBubble.gameObject);

        // Drop the gun.
        Gun.transform.SetParent(null);
        Gun.Rigidbody.isKinematic = false;
        Gun.PickUpRange.enabled = true;
    }

    /// <summary>
    /// Pull out the gun.
    /// </summary>
    public void DrawGun(PlayerCharacter playerCharacter)
    {
        m_aggroPlayerCharacter = playerCharacter;
        Gun.gameObject.SetActive(true);
        Gun.Rigidbody.isKinematic = true;
        Gun.PickUpRange.enabled = false;
        Ragdoll.Animator.Play("Gun", 0);

        if(SpeechBubble != null && SpeechText_NPCDied != string.Empty)
        {
            SpeechBubble.ShowTimedPopup(UnityEngine.Random.Range(0f, 8f), SpeechText_NPCDied);
        }
    }
}
