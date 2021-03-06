﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip ButtonPressSound;
    public AudioClip SwordPickupSound;
    public AudioClip SwordSwingSound;
    public AudioClip GunPickupSound;
    public AudioClip JumpSound;
    public AudioClip CoinPickupSound;
    public AudioClip CoinSpendSound;
    public AudioClip SpeechBubbleAppearSound;
    public AudioClip PushOffBridgeSound;
    public AudioClip HitWithBulletSound;
    public AudioClip VictorySound;

    public List<AudioClip> ShootSounds;
    public List<AudioClip> TextAppearSounds;
    public List<AudioClip> MaleDeathSounds;
    public List<AudioClip> LandSounds;

    private List<AudioSource> Sources = new List<AudioSource>();
    private const int MaxSources = 10;
    private int m_currentSourceIndex = 0;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < MaxSources; i++)
        {
            Sources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    /// <summary>
    /// Play a shoot sound.
    /// </summary>
    public void PlayShootSound()
    {
        int index = Random.Range(0, ShootSounds.Count);
        PlaySound(ShootSounds[index]);
    }

    /// <summary>
    /// Play a shoot sound.
    /// </summary>
    public void PlayTextAppearSound()
    {
        int index = Random.Range(0, TextAppearSounds.Count);
        PlaySound(TextAppearSounds[index]);
    }

    /// <summary>
    /// Play a shoot sound.
    /// </summary>
    public void PlayLandSound()
    {
        int index = Random.Range(0, LandSounds.Count);
        PlaySound(LandSounds[index]);
    }

    /// <summary>
    /// Play a shoot sound.
    /// </summary>
    public void PlayMaleDeathSound()
    {
        int index = Random.Range(0, MaleDeathSounds.Count);
        PlaySound(MaleDeathSounds[index]);
    }

    /// <summary>
    /// Play an audio clip on the pool of sources.
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        // Get the next source.
        m_currentSourceIndex++;
        if (m_currentSourceIndex >= MaxSources)
        {
            m_currentSourceIndex = 0;
        }

        Sources[m_currentSourceIndex].Stop();
        Sources[m_currentSourceIndex].clip = clip;
        Sources[m_currentSourceIndex].Play();
    }
}