using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    public GameObject SpendFX;

    /// <summary>
    /// Spend the coin.
    /// </summary>
    public void SpendCoin()
    {
        // Show the spend FX.
        SpendFX.SetActive(true);
        SpendFX.transform.SetParent(null);
        Destroy(SpendFX, 5f);

        // Make the hand drop the coin.
        m_heldHand.DropItem();

        AudioManager.Instance.PlaySound(AudioManager.Instance.CoinSpendSound);

        // Destroy the coin.
        Destroy(gameObject);
    }

    /// <summary>
    /// Grabbed
    /// </summary>
    protected override void OnGrabbed()
    {
        base.OnGrabbed();

        AudioManager.Instance.PlaySound(AudioManager.Instance.CoinPickupSound);
    }
}
