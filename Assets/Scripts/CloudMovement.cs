using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CloudMovement : MonoBehaviour {

    [SerializeField] private bool isTransportingPlayer = false;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float timeToDisintegrate = 5f;

    private EdgeCollider2D _coll;
    private const float BoxCastHeight = 0.05f;
    private float _originYOffset;
    private Vector2 _boxSize;

    private static PlayerMovement _playerMovement;
    private float destroyX;
    private Coroutine disintegrateCo;

    private void Awake() {
        destroyX = transform.position.x * -1;
    }

    private void Start() {
        if (!isTransportingPlayer) {
            return;
        }
        
        if (_playerMovement == null) {
            _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        _coll = GetComponent<EdgeCollider2D>();
        _originYOffset = _coll.offset.y + BoxCastHeight / 2;
        _boxSize = new Vector2(transform.localScale.x, BoxCastHeight);
    }

    private void Update() {
        DestroyIfOutsideScreen();
        
        float moveStep = speed * Time.deltaTime;
        if (isTransportingPlayer) {
            TransportPlayer(moveStep);
        }
        transform.Translate(moveStep, 0, 0);
    }

    /**
     * This function has assumptions on how the collider is set up and assumes there's only one player that exists and
     * that it is the only thing on the playerMask. Generalization is not needed at this time.
     */
    private void TransportPlayer(float moveStep) {
        // Detect if the player is riding the cloud by casting a box on the surface of the cloud.
        Vector2 origin = transform.position;
        origin.y += _originYOffset;
        RaycastHit2D hit = Physics2D.BoxCast(origin, _boxSize, 0, Vector2.up, 0, playerMask);
        if (hit.collider == null || !_playerMovement.IsGrounded()) {
            return;
        }
        
        // The player was detected and is standing on the cloud.
        hit.transform.Translate(moveStep, 0, 0);
            
        // If this is the first time the player is standing on the cloud, start to disintegrate.
        // (Yo, I just learned about this null-coalescing assignment operator and my mind is blown.)
        disintegrateCo ??= StartCoroutine(Disintegrate());
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    private void DestroyIfOutsideScreen() {
        if ((speed > 0 && transform.position.x > destroyX) || (speed < 0 && transform.position.x < destroyX)) {
            Destroy(gameObject);
        }
    }

    private IEnumerator Disintegrate() {
        float totalTime = timeToDisintegrate;
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        Color startingColor = rend.color;
        while (timeToDisintegrate > 0) {
            timeToDisintegrate -= Time.deltaTime;
            Color c = startingColor;
            c.a = Mathf.Lerp(1, 0, (totalTime-timeToDisintegrate) / totalTime);
            rend.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
