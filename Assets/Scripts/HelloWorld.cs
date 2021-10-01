using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour {
    private Camera cam;
    
    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            cam.backgroundColor = new Color(Random.value, Random.value, Random.value);
        }
    }
}
