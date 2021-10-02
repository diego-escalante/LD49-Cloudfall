using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeShifter : MonoBehaviour {
    [SerializeField] private AnimationCurve shiftCurve;
    private float x;
    private Coroutine shiftCo;

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.StartTimeShift, StartShift);
        EventManager.StartListening(EventManager.Event.StopTimeShift, StopShift);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.StartTimeShift, StartShift);
        EventManager.StopListening(EventManager.Event.StopTimeShift, StopShift);
        x = 0;
    }

    public float GetFactor() {
        return shiftCurve.Evaluate(x);
    }

    private void StartShift() {
        x = 1;
        // if (shiftCo != null) {
        //     StopCoroutine(shiftCo);
        // }
        //
        // shiftCo = StartCoroutine(ShiftX(1));
    }

    private void StopShift() {
        x = 0;
        // if (shiftCo != null) {
        //     StopCoroutine(shiftCo);
        // }
        //
        // shiftCo = StartCoroutine(ShiftX(0));
    }

    private IEnumerator ShiftX(float targetX) {
        while (true) {
            float delta = Time.deltaTime;
            if (x < targetX) {
                x += delta;
                if (x >= targetX) {
                    x = targetX;
                    break;
                }
            }
            else {
                x -= delta;
                if (x <= targetX) {
                    x = targetX;
                    break;
                }
            }
            yield return null;
        }
    } 
    
}
