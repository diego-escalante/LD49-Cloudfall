using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {
    private TMP_Text score, best;
    private int scoreVal, bestVal;
    private Transform player;
    private static ScoreTracker instance;
    private bool isUpdating = false;

    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.SceneStart, Initialize);
        EventManager.StartListening(EventManager.Event.PlayerFell, Stop);
    }
    
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.SceneStart, Initialize);
        EventManager.StopListening(EventManager.Event.PlayerFell, Stop);
    }

    private void Update() {
        if (!isUpdating) {
            return;
        }
        
        int height = (int)player.position.y;
        
        if (height > scoreVal) {
            scoreVal = height;
            score.text = $"{scoreVal}m";
        }

        if (height > bestVal) {
            bestVal = height;
            best.text = $"{bestVal}m";
        }
    }

    private void Stop() {
        isUpdating = false;
    }

    private void Initialize() {
        Transform parent = GameObject.FindGameObjectWithTag("UI").transform.Find("ScoreTop");
        score = parent.Find("ScoreBundle/Score").GetComponent<TMP_Text>();
        scoreVal = 0;
        score.text = $"{scoreVal}m";
        best = parent.Find("BestBundle/Best").GetComponent<TMP_Text>();
        best.text = $"{bestVal}m";
        player = GameObject.FindGameObjectWithTag("Player").transform;
        isUpdating = true;
    }
}
