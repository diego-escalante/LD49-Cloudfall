using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

    public RawImage fade;

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.PlayerFell, FadeOut);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.PlayerFell, FadeOut);
    }
    
    private void Start() {
        EventManager.TriggerEvent(EventManager.Event.SceneStart);
        FadeIn();
    }

    private void FadeIn() {
        StartCoroutine(Fade(Color.black, Color.clear, 0.5f, false));
    }

    private void FadeOut() {
        StartCoroutine(Fade(Color.clear, Color.black, 0.5f, true));
    }

    private IEnumerator Fade(Color start, Color end, float duration, bool restart) {
        if (restart) {
            float timer = 0.5f;
            while (timer > 0) {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        float total = duration;
        while (duration >= 0) {
            duration -= Time.deltaTime;
            fade.color = Color.Lerp(start, end, (total - duration) / total);
            yield return null;
        }

        if (restart) {
            RestartScene();
        }
    }

    private void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}