using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    public Animator Animator;

    private bool m_isShowing = false;

    private const float CharacterSpeed = 0.05f;
    private const float DelayAfterShowingTimedPopup = 2f;

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
            AudioManager.Instance.PlaySound(AudioManager.Instance.SpeechBubbleAppearSound);
            StartCoroutine(SetTextOverTime(newText));
        }

        // Play animation to show or hide.
        Animator.SetTrigger(shown ? "Show" : "Hide");
    }

    /// <summary>
    /// Show with some delays. Delete after delay.
    /// </summary>
    public void ShowTimedPopup(float delayBeforeShowing, string text)
    {
        StartCoroutine(ShowTimedPopupCoroutine(delayBeforeShowing, text));
    }

    /// <summary>
    /// Show the popup
    /// </summary>
    /// <param name="delayBeforeShowing"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator ShowTimedPopupCoroutine(float delayBeforeShowing, string text)
    {
        yield return new WaitForSeconds(delayBeforeShowing);

        m_isShowing = true;
        Animator.SetTrigger("Show");
        AudioManager.Instance.PlaySound(AudioManager.Instance.SpeechBubbleAppearSound);
        yield return SetTextOverTime(text);

        yield return new WaitForSeconds(DelayAfterShowingTimedPopup);

        Show(false, "");
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
            AudioManager.Instance.PlayTextAppearSound();
            Text.maxVisibleCharacters = shownCharacters;
            shownCharacters++;
            yield return new WaitForSeconds(CharacterSpeed);
        }

        Text.maxVisibleCharacters = maxCharacters;
    }

    /// <summary>
    /// Is all the text shown.
    /// </summary>
    public bool IsComplete()
    {
        return Text.maxVisibleCharacters == Text.textInfo.characterCount;
    }
}
