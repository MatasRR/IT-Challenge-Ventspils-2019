using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    public AudioSource MusicSource;
    public AudioSource SFXSource;

    public AudioClip Discovery;

    public float PitchRandomness;
    private float OriginalSFXPitch;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        OriginalSFXPitch = SFXSource.pitch;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySFX(Discovery);
        }
    }

    public void PlaySFX(AudioClip SFX)
    {
        SFXSource.pitch = OriginalSFXPitch + Random.Range(-PitchRandomness, PitchRandomness);
        SFXSource.PlayOneShot(SFX);
    }
}
