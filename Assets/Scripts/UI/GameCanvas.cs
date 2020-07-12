using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public GameObject GameOverSection;

    public void ShowGameOver()
    {
        GameOverSection.SetActive(true);

        // Cursor stuff.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowGameplay()
    {
        GameOverSection.SetActive(false);

        // Cursor stuff.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
