using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : Singleton<GameManager>
{
    public GameCanvas GameCanvas;
    public PostProcessVolume PostProcessing;

    public bool WasNPCKilled { get; set; }

    private bool m_isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        GameCanvas.ShowGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        // Quit when hitting escape.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // If framerate dips too much, turn off post processing.
        // This should speed it up.
        if(FPSCounter.Instance.IsLegitimate() && FPSCounter.Instance.CurrentFps < 20)
        {
            //SetPostProcessingActive(false);
        }
    }

    /// <summary>
    /// Player died.
    /// </summary>
    public void PlayerDied()
    {
        if(m_isGameOver)
        {
            return;
        }
        m_isGameOver = true;

        GameCanvas.ShowGameOver();
    }

    /// <summary>
    /// Reload the game.
    /// </summary>
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Enable or disable post processing.
    /// </summary>
    public void SetPostProcessingActive(bool active)
    {
        PostProcessing.enabled = active;
    }
}
