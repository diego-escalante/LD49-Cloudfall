using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour {

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.PlayerFell, RestartScene);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.PlayerFell, RestartScene);
    }

    private void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}