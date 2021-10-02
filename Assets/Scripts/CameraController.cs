using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform player;
    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        RaiseCameraWithPlayer();
        CheckIfPlayerFell();
    }

    private void RaiseCameraWithPlayer() {
        Vector3 camPos = transform.position;
        if (camPos.y < player.position.y) {
            camPos.y = player.position.y;
            transform.position = camPos;
        }
    }

    private void CheckIfPlayerFell() {
        Vector3 topOfPlayer = player.position + new Vector3(0, player.localScale.y / 2f, 0);
        if (cam.WorldToScreenPoint(topOfPlayer).y < 0) {
            EventManager.TriggerEvent(EventManager.Event.PlayerFell);
        }
    }

}
