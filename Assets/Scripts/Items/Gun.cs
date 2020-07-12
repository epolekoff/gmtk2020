using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public GameObject BulletPrefab;
    public Transform BulletSpawnPosition;
    public GameObject FireFX;

    private const float HandLerpToPositionTime = 0.2f;
    private const float TimeBetweenShots = 0;
    private const float HeldGunHeight = 1f;
    private const float HeldGunDistance = 3f;

    private int m_numShotsRemaining = 15;

    void Start()
    {
        
    }

    /// <summary>
    /// Fire a bullet.
    /// </summary>
    /// <param name="direction"></param>
    public void Fire(Vector3 direction)
    {
        FireFX.SetActive(true);
        FireFX.GetComponent<ParticleSystem>().Play();

        // Spawn a bullet.
        GameObject newBullet = GameObject.Instantiate(BulletPrefab, BulletSpawnPosition.position, Quaternion.LookRotation(direction, Vector3.up), null);

        // Make the bullet move.
        newBullet.GetComponent<Bullet>().Fire(direction);

        AudioManager.Instance.PlayShootSound();
    }

    /// <summary>
    /// When the gun is grabbed, start using it.
    /// </summary>
    protected override void OnGrabbed()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GunPickupSound);
        StartCoroutine(PlayerUseGunCoroutine());
    }

    /// <summary>
    /// When dropped, stop firing.
    /// </summary>
    protected override void OnDropped()
    {
        base.OnDropped();

        StopAllCoroutines();
    }

    /// <summary>
    /// Shoot a bunch.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerUseGunCoroutine()
    {
        // Wait a bit before you start firing.
        yield return new WaitForSeconds(0.5f);

        // Fire until dropped or no bullets remaining.
        while(m_numShotsRemaining > 0)
        {
            // Pick a random direction.
            float angle = Random.Range(0, 360);
            float upAngle = Random.Range(-10, 45);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 fireDirection = (rotation * Vector3.forward);
            Vector3 gunLookDirection = (rotation * Vector3.right);
            Vector3 handPosition = (m_heldHand.Owner.transform.position + (Vector3.up * HeldGunHeight)) + (fireDirection * HeldGunDistance);

            // Point the gun in that direction.
            transform.rotation = Quaternion.LookRotation(gunLookDirection);

            // Move the hand into position.
            float timer = 0;
            Vector3 handStartPosition = m_heldHand.transform.position;
            while (timer < HandLerpToPositionTime)
            {
                timer += Time.deltaTime;
                float t = timer / HandLerpToPositionTime;

                m_heldHand.transform.position = Vector3.Lerp(handStartPosition, handPosition, t);

                yield return null;
            }

            // Fire
            Fire(fireDirection);

            // Reduce the shots.
            m_numShotsRemaining--;

            // Wait for the next shot.
            yield return new WaitForSeconds(TimeBetweenShots);
        }


        m_heldHand.transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
