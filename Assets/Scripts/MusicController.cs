using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public AudioSource music1, music2, music3, music4, music5;

    private float volume = 1.25f;

    private int musicStage = 0;

    private void Start() {
        StartMusic();
    }

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.StartTimeShift, PitchDown);
        EventManager.StartListening(EventManager.Event.StopTimeShift, PitchUp);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.StartTimeShift, PitchDown);
        EventManager.StopListening(EventManager.Event.StopTimeShift, PitchUp);
    }

    public void StartMusic() {
        if (music1.isActiveAndEnabled) {
            music1.Play();
        }
        if (music2.isActiveAndEnabled) {
            music2.Play();
        }
        if (music3.isActiveAndEnabled) {
            music3.Play();
        }
        if (music4.isActiveAndEnabled) {
            music4.Play();
        }
        if (music5.isActiveAndEnabled) {
            music5.Play();
        }
    }

    public void Update() {
        float y = Camera.main.transform.position.y;
        if (musicStage != 1 && y < 10f) {
            Music1();
        } if (musicStage == 1 && y > 60f) {
            Music2();
        } else if (musicStage == 2 && y > 120f) {
            Music3();
        } else if (musicStage == 3 && y > 180f) {
            Music4();
        } else if (musicStage == 4 && y > 240f) {
            Music5();
        }
    }

    private IEnumerator fadeIn(AudioSource audio, float toVolume, float duration=1f) {
        float timeLeft = duration;
        float startingVolume = audio.volume;

        while (timeLeft > 0) {
            timeLeft -= Time.unscaledDeltaTime;
            audio.volume = Mathf.Lerp(startingVolume, toVolume, 1f - (timeLeft/duration));
            yield return null;
        }
        audio.volume = toVolume;
    }

    // Running out of time, don't do bad code like this, kids.
    public void Music1() {
        StartCoroutine(fadeIn(music1, volume));
        StartCoroutine(fadeIn(music2, 0));
        StartCoroutine(fadeIn(music3, 0));
        StartCoroutine(fadeIn(music4, 0));
        StartCoroutine(fadeIn(music5, 0));
        musicStage = 1;
    }

    public void Music2() {
        StartCoroutine(fadeIn(music1, 0));
        StartCoroutine(fadeIn(music2, volume));
        StartCoroutine(fadeIn(music3, 0));
        StartCoroutine(fadeIn(music4, 0));
        StartCoroutine(fadeIn(music5, 0));
        musicStage = 2;
    }
    
    public void Music3() {
        StartCoroutine(fadeIn(music1, 0));
        StartCoroutine(fadeIn(music2, 0));
        StartCoroutine(fadeIn(music3, volume));
        StartCoroutine(fadeIn(music4, 0));
        StartCoroutine(fadeIn(music5, 0));
        musicStage = 3;
    }
    
    public void Music4() {
        StartCoroutine(fadeIn(music1, 0));
        StartCoroutine(fadeIn(music2, 0));
        StartCoroutine(fadeIn(music3, 0));
        StartCoroutine(fadeIn(music4, volume));
        StartCoroutine(fadeIn(music5, 0));
        musicStage = 4;
    }
    
    public void Music5() {
        StartCoroutine(fadeIn(music1, 0));
        StartCoroutine(fadeIn(music2, 0));
        StartCoroutine(fadeIn(music3, 0));
        StartCoroutine(fadeIn(music4, 0));
        StartCoroutine(fadeIn(music5, volume));
        musicStage = 5;
    }

    public void PitchDown() {
        music1.pitch = 0.5f;
        music2.pitch = 0.5f;
        music3.pitch = 0.5f;
        music4.pitch = 0.5f;
        music5.pitch = 0.5f;
    }
    
    public void PitchUp() {
        music1.pitch = 1f;
        music2.pitch = 1f;
        music3.pitch = 1f;
        music4.pitch = 1f;
        music5.pitch = 1f;
    }
}
