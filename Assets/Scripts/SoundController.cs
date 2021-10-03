using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioClip fallSound;
    public AudioClip jumpSound;
    public AudioClip cloudPoofSound;
    public AudioClip flapSound;
    public AudioClip startShiftSound;
    public AudioClip stopShiftSound;

    public AudioSource audioSource;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnEnable() {
        EventManager.StartListening(EventManager.Event.PlayerFell, playfallSound);
        EventManager.StartListening(EventManager.Event.PlayerJumped, playJumpSound);
        EventManager.StartListening(EventManager.Event.CloudDisintegrated, playcloudPoofSound);
        EventManager.StartListening(EventManager.Event.PlayerFlapped, playflapSound);
        EventManager.StartListening(EventManager.Event.StartTimeShift, playStartShiftSound);
        EventManager.StartListening(EventManager.Event.StopTimeShift, playStopShiftSound);
    }
    
    public void OnDisable() {
        EventManager.StopListening(EventManager.Event.PlayerFell, playfallSound);
        EventManager.StopListening(EventManager.Event.PlayerJumped, playJumpSound);
        EventManager.StopListening(EventManager.Event.CloudDisintegrated, playcloudPoofSound);
        EventManager.StopListening(EventManager.Event.PlayerFlapped, playflapSound);
        EventManager.StopListening(EventManager.Event.StartTimeShift, playStartShiftSound);
        EventManager.StopListening(EventManager.Event.StopTimeShift, playStopShiftSound);
    }

    public void playfallSound() {
        audioSource.PlayOneShot(fallSound, 1);
    }

    public void playJumpSound() {
        audioSource.PlayOneShot(jumpSound, 1);
    }

    public void playcloudPoofSound() {
        audioSource.PlayOneShot(cloudPoofSound, 1);
    }

    public void playflapSound() {
        audioSource.PlayOneShot(flapSound, 1);
    }
    
    public void playStopShiftSound() {
        audioSource.PlayOneShot(stopShiftSound, 1);
    }
    
    public void playStartShiftSound() {
        audioSource.PlayOneShot(startShiftSound, 1);
    }
}
