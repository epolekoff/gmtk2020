using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public GameObject BulletPrefab;
    public Transform BulletSpawnPosition;
    public GameObject FireFX;

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
    }
}
