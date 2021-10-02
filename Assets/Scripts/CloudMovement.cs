using System.Collections;
using UnityEngine;

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
    private float edgeX;
    private Coroutine disintegrateCo;
    private float yMin, yMax;
    private bool isDisintegrating;
    private int direction;

    private void Awake() {
        direction = (int)Mathf.Sign(Random.Range(-1, 1));
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
        LoopIfOutsideScreen();
        
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
        if (!isDisintegrating) {
            StartCoroutine(Disintegrate());
            isDisintegrating = true;
        }
    }

    private void LoopIfOutsideScreen() {
        Vector3 pos = transform.position;
        if ((speed > 0 && pos.x > edgeX) || (speed < 0 && pos.x < -edgeX)) {
            // If the cloud was disintegrating, just delete it.
            if (isDisintegrating) {
                Destroy(gameObject);
            }
            
            ResetCloud();
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

    public void InitializeCloud(float yMin, float yMax) {
        // Calculation assumes the camera is always at x = 0.
        edgeX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + transform.localScale.x / 2;
        this.yMin = yMin;
        this.yMax = yMax;
        ResetCloud();
        Vector3 pos = transform.position;
        pos.x = Random.Range(-edgeX, edgeX); // Start the cloud for the first time at a random horizontal position.
        transform.position = pos;
    }

    private void ResetCloud() {
        // Move the cloud back to the starting point horizontally.
        Vector3 pos = transform.position;
        pos.x = speed > 0 ? -edgeX : edgeX;
        
        // Wiggle the height of the cloud in its row. Remove this line if it's too unpredictable.
        pos.y = Random.Range(yMin, yMax);
        speed = CalculateSpeed(pos.y) * direction;    // Update the speed based on the new height.
        transform.position = pos;
    }
    
    // linear relationship between height and speed.
    private float CalculateSpeed(float height) {
        return (height + 30) / 60f;
    }
}
