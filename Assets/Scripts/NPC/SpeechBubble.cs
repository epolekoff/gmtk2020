using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    public Animator Animator;

    private bool m_isShowing = false;

    private const float CharacterSpeed = 0.05f;

    /// <summary>
    /// Show or hide,
    /// </summary>
    public void Show(bool shown, string newText)
    {
        if(m_isShowing == shown)
        {
            return;
        }

        m_isShowing = shown;
        StopAllCoroutines();

        // Set the text.
        if(shown)
        {
            StartCoroutine(SetTextOverTime(newText));
        }

        // Play animation to show or hide.
        Animator.SetTrigger(shown ? "Show" : "Hide");
    }

    /// <summary>
    /// Make the characters appear over time.
    /// </summary>
    private IEnumerator SetTextOverTime(string text)
    {
        Text.text = text;
        yield return null;

        int shownCharacters = 0;
        int maxCharacters = Text.textInfo.characterCount;

        while (shownCharacters <= maxCharacters)
        {
            Text.maxVisibleCharacters = shownCharacters;
            shownCharacters++;
            yield return new WaitForSeconds(CharacterSpeed);
        }

        Text.maxVisibleCharacters = maxCharacters;
    }
}
