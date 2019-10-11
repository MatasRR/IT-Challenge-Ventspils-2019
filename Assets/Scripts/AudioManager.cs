using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    public AudioSource MusicSource;
    public AudioSource SFXSource;

    public Slider MusicSlider;
    public Slider SFXSlider;

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

        if (GameObject.FindGameObjectWithTag("Music Slider") != null)
        {
            MusicSlider = GameObject.FindGameObjectWithTag("Music Slider").GetComponent<Slider>();
            SFXSlider = GameObject.FindGameObjectWithTag("SFX Slider").GetComponent<Slider>();
        }
    }

    private void Update()
    {
        if (MusicSlider != null)
        {
            MusicSource.volume = MusicSlider.value;
            SFXSource.volume = SFXSlider.value;
        }
    }

    public void PlaySFX(AudioClip SFX)
    {
        SFXSource.pitch = OriginalSFXPitch + Random.Range(-PitchRandomness, PitchRandomness);
        SFXSource.PlayOneShot(SFX);
    }
}
