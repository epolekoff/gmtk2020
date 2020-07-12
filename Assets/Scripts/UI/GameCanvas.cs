using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public GameObject GameOverSection;
    public GameObject VictorySection;

    public void ShowGameOver()
    {
        GameOverSection.SetActive(true);
        VictorySection.SetActive(false);

        // Cursor stuff.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowGameplay()
    {
        GameOverSection.SetActive(false);
        VictorySection.SetActive(false);

        // Cursor stuff.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowVictory()
    {
        GameOverSection.SetActive(false);
        VictorySection.SetActive(true);

        // Cursor stuff.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
