using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject Visual;
    public Collider Collider;
    public GameObject HitFX;

    private const float MoveSpeed = 5f;

    private bool m_destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_destroyed)
        {
            return;
        }

        // Keep the bullet moving forward.
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Set the parameters for firing.
    /// </summary>
    public void Fire(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Ensure bullets always get cleaned up.
        Destroy(gameObject, 20f);
    }

    /// <summary>
    /// Destroy on collision.
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        Visual.SetActive(false);

        HitFX.SetActive(true);
        HitFX.GetComponent<ParticleSystem>().Play();

        Destroy(gameObject, 3f);
        m_destroyed = true;
        Collider.enabled = false;

        // Hit an NPC, kill them.
        NPC hitNPC = col.gameObject.GetComponentInParent<NPC>();
        if (hitNPC != null)
        {
            hitNPC.OnSlashed(transform.forward);
        }

        // Hit a Player, deal damage.
        PlayerCharacter hitPlayer = col.gameObject.GetComponentInParent<PlayerCharacter>();
        if (hitPlayer != null)
        {
            hitPlayer.OnHitByBullet(transform.forward);
        }
    }
}
