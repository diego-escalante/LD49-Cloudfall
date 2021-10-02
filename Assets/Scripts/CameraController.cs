using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform player;
    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        Vector3 topOfPlayer = player.position + new Vector3(0, player.localScale.y / 2f, 0);
        if (cam.WorldToScreenPoint(topOfPlayer).y < 0) {
            // Scene stuff should go into its own controller.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
