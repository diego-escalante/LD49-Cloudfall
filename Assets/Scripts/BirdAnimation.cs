using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimation : MonoBehaviour {

    private SpriteRenderer _rend;
    private PlayerMovement _playerMovement;
    private Animator _anim;
    private TimeShifter _timeShifter;
    

    private void Awake() {
        _playerMovement = GetComponent<PlayerMovement>();
        _rend = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _timeShifter = GetComponent<TimeShifter>();
    }

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.PlayerFell, Fell);
    }
    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.PlayerFell, Fell);
    }

    private void Update() {
        _anim.speed = _timeShifter.GetFactor();
        
        Vector2 vel = _playerMovement.GetVelocity();
        _anim.SetFloat("vertical", vel.y);
        _anim.SetFloat("horizontal", vel.x);
        
        // Set the orientation of the bird.
        if (vel.x > 0) {
            _rend.flipX = true;
        } else if (vel.x < 0) {
            _rend.flipX = false;
        }
    }

    public void Flap() {
        EventManager.TriggerEvent(EventManager.Event.PlayerFlapped);
    }

    public void Fell() {
        gameObject.SetActive(false);
    }
    

}
