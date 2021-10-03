using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleController : MonoBehaviour {

    private TMP_Text title;
    private Transform cam;
    private bool active = false;

    private void Start() {
        title = GetComponent<TMP_Text>();
        cam = Camera.main.transform;
    }

    private void Update() {
        if (cam.position.y > 8f && !active) {
            active = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut() {
        float duration = 1f;
        float total = duration;
        while (duration > 0) {
            duration -= Time.deltaTime;
            title.color = Color.Lerp(Color.black, Color.clear, (total-duration) / total);
            yield return null;
        }
        Destroy(this);
    }
}
