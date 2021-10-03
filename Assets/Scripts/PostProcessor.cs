using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessor : MonoBehaviour {

    public GameObject volume;
    
    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.StartTimeShift, On);
        EventManager.StartListening(EventManager.Event.StopTimeShift, Off);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.StartTimeShift, On);
        EventManager.StopListening(EventManager.Event.StopTimeShift, Off);
    }

    private void On() {
        volume.SetActive(true);
    }
    
    private void Off() {
        volume.SetActive(false);
    }
}
