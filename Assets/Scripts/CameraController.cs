using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Gradient backgroundColor;
    [SerializeField] private float maxGradientHeight = 50f;
    private Transform player;
    private Camera cam;
    public SpriteRenderer starField;
    private bool fell = false;

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
            UpdateBackgroundColor();
        }
    }

    private void CheckIfPlayerFell() {
        // Vector3 topOfPlayer = player.position + new Vector3(0, player.localScale.y / 2f, 0);
        if (cam.WorldToScreenPoint(player.position).y < 0 && !fell) {
            fell = true;
            EventManager.TriggerEvent(EventManager.Event.PlayerFell);
        }
    }

    private void UpdateBackgroundColor() {
        float normalized = transform.position.y / maxGradientHeight;
        cam.backgroundColor = backgroundColor.Evaluate(normalized);
        if (normalized >= 0.8f) {
            starField.color = Color.Lerp(Color.clear, Color.white, Map(normalized, 0.8f, 1, 0, 1));
        }
    }
    
    private float Map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

}
