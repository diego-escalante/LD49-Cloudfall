using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeShiftController : MonoBehaviour {

    [SerializeField] private Image[] bars;
    [SerializeField] private float totalDuration = 5f;
    [SerializeField] private float rechargeRate = 0.5f;
    [SerializeField] private float cooldownDuration = 1f;

    private float _currentDuration;
    private PlayerMovement _playerMovement;
    private bool _isShifting;

    private bool onCooldown;

    
    private void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
        _currentDuration = totalDuration;
    }
    private void Update() {
        // Start shifting if the player presses the spacebar while in the air and has duration left.
        if (Input.GetKeyDown(KeyCode.Space) && _playerMovement.GetJumpsLeft() == 0 && _currentDuration > 0) {
            StartShifting();
        // Stop shifting if the player lets go of the spacebar, hits the ground, or runs out of juice.
        } else if (Input.GetKeyUp(KeyCode.Space) || _playerMovement.IsGrounded() || _currentDuration <= 0) {
            StopShifting();
        }

        // If the player is shifting, reduce duration until it hits zero.
        if (_isShifting) {
            _currentDuration = Mathf.Max(0, _currentDuration - Time.deltaTime);
            UpdateShifterBar();
            // We hit 0, start the cooldown penalty.
            if (_currentDuration == 0) {
                StartCoroutine(Cooldown());
            }
        }
        // If the player is not shifting, not on cooldown and isn't fully recharged, recharge until full.
        else if (_currentDuration < totalDuration && !onCooldown) {
            _currentDuration = Mathf.Min(_currentDuration + Time.deltaTime * rechargeRate, totalDuration);
            UpdateShifterBar();
        }
    }

    private IEnumerator Cooldown() {
        onCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    private void StartShifting() {
        _isShifting = true;
        EventManager.TriggerEvent(EventManager.Event.StartTimeShift);
    }
    private void StopShifting() {
        _isShifting = false;
        EventManager.TriggerEvent(EventManager.Event.StopTimeShift);
    }

    public void UpdateShifterBar() {
        float currentNormalized = _currentDuration / totalDuration;
        for (int i = 0; i < bars.Length; i++) {
            bars[i].fillAmount = currentNormalized;
        }
    }
    
}
