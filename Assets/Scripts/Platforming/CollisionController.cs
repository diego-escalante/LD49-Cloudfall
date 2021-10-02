using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class CollisionController : MonoBehaviour {

    // TODO: (Refactor) Everything that can be moved should have a collision controller and use it for all movement.
    //       Can you extend transform to have a Move method that automatically uses the collision controller and moves?
    //       Maybe if there's no CollisionChecker, transform.Move can be a passthrough to transform.Translate.
    // TODO: (Refactor) Maybe don't define a default collisionMask here if we are now going to allow things to pass in their own mask.
    // TODO: (Refactor) If other things are using Check to see what is touching them, maybe just add a type of SkinCheck that checks for
    //       colliders at a SKIN distance in a particular direction.
    // TODO: (Refactor) (MAYBE) Do we really need CollisionInfo.HorizontalCollider AND CollisionInfo.VerticalCollider????
    // TODO: (Refactor) (MAYBE) Maybe use a Vector instead of 4 bools for what direction collisions happened? E.G. Vector(1,-1) = collision happened to right and bottom.

    /* This class is used to check for collisions in moving objects.
     * Note that this does not define how the object moves, but it merely 
     * takes a translation vector and returns a new translation vector
     * that has been collision checked, along with all sorts of info about
     * said collisions.
     */

    // Defines what the object can collide with.
    public LayerMask collisionMask;
    public LayerMask onewayMask;

    private BoxCollider2D boxCollider;
    private const float SKIN = 0.015f;
    private const int RAYCAST_COUNT = 5;

    private CollisionInfo collisionInfo;
    private RaycastOrigins raycastOrigins;

    private float droppingDuration = 0.25f;
    private float droppingDurationLeft;

    // The dimensions of the collider, divided by half. Only needs to be manually updated if the collider changes in any way.
    private Vector2 colliderHalfDims;
    // Indicates if the collider dimensions should be recalculated each frame. Not necessary unless it's changing each frame.
    public bool updateColliderDimension = false;

    public struct CollisionInfo {
        // Indicates where collisions were detected.
        public bool collisionAbove, collisionBelow, collisionLeft, collisionRight;
        // Stores the colliders of collisions detected.
        public Collider2D colliderVertical, colliderHorizontal;
        // The new movement vector after collision checking is done.
        public Vector2 moveVector;

        public RaycastHit2D hit;

        public void Reset() {
            Reset(Vector2.zero);
        }

        public void Reset(Vector2 moveVector) {
            collisionAbove = collisionBelow = collisionLeft = collisionRight = false;
            colliderVertical = colliderHorizontal = null;
            this.moveVector = moveVector;
        }
    }

    public void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateColliderDimensions();
        if (collisionMask.value == 0) {
            Debug.LogWarning("There is no layers selected for the Collision Mask in the CollisionChecker.");
            this.enabled = false;
        }
        if (collisionMask.Contains(gameObject.layer)) {
            Debug.LogWarning("GameObject's layer is included in its collisionMask!");
        }
        collisionInfo = new CollisionInfo();
    }

    public void Update() {
        if (droppingDurationLeft > 0) {
            droppingDurationLeft -= Time.deltaTime;
        }
    }

    /*
     * Takes in a moveVector that indicates how much the object wants to move,
     * and returns a vector indicating where the object will actually move to
     * given any collisions. If no collisionMask is provided, CollisionController's default will be used.
     */
    public ref CollisionInfo Check(Vector2 moveVector, bool drop) {
        if (drop) {
            droppingDurationLeft = droppingDuration;
        }
        return ref Check(moveVector);
    }

    public ref CollisionInfo Check(Vector2 moveVector) {
        return ref Check(moveVector, collisionMask);
    } 
    public ref CollisionInfo Check(Vector2 moveVector, LayerMask collisionMask) {
        UpdateRaycastOrigins();
        // Reuse the same collisionInfo within this class, as this function is called constantly each frame.
        collisionInfo.Reset(moveVector);

        // Horizontal collision check.
        if (moveVector.x != 0) {

            // Set up rays.
            float rayDirection = Mathf.Sign(moveVector.x);
            float rayDistance = Mathf.Abs(moveVector.x) + SKIN;
            Vector2 startOrigin, endOrigin;
            if (rayDirection == -1) {
                startOrigin = raycastOrigins.bottomLeft;
                endOrigin = raycastOrigins.topLeft;
            } else {
                startOrigin = raycastOrigins.bottomRight;
                endOrigin = raycastOrigins.topRight;
            }
            
            // Cast rays.
            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D hit = Helpers.RaycastWithDebug(raycastOrigin, rayDirection * Vector2.right, rayDistance, collisionMask);

                // If hit, track everything about the collision.
                if (hit.collider != null) {
                    collisionInfo.colliderHorizontal = hit.collider;
                    collisionInfo.moveVector.x = (hit.distance - SKIN) * rayDirection;
                    if (rayDirection == -1) {
                        collisionInfo.collisionLeft = true;
                    } else {
                        collisionInfo.collisionRight = true;
                    }
                    break;
                }
            }
        }

        // Vertical collision check.
        if (moveVector.y != 0) {
            
            // Set up rays.
            float rayDirection = Mathf.Sign(moveVector.y);
            float rayDistance = Mathf.Abs(moveVector.y) + SKIN;
            Vector2 startOrigin, endOrigin;
            if (rayDirection == -1) {
                startOrigin = raycastOrigins.bottomRight;
                endOrigin = raycastOrigins.bottomLeft;
            } else {
                startOrigin = raycastOrigins.topRight;
                endOrigin = raycastOrigins.topLeft;
            }
            // Vertical collisions are checked after horizontal collisions, so the rays should be casted from where the object will be horizontally.
            startOrigin.x += collisionInfo.moveVector.x;
            endOrigin.x += collisionInfo.moveVector.x;

            // Cast rays.
            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D hit = Helpers.RaycastWithDebug(raycastOrigin, rayDirection * Vector2.up, rayDistance, collisionMask | onewayMask);

                // If hit, track everything about the collision.
                if (hit.collider != null) {

                    // Special case: if the object is dropping through oneway platforms.
                    if (onewayMask.Contains(hit.collider.gameObject.layer) && ((rayDirection == 1 || hit.distance == 0) || droppingDurationLeft > 0)) {
                        continue;
                    }
                    
                    collisionInfo.colliderVertical = hit.collider;
                    collisionInfo.moveVector.y = (hit.distance - SKIN) * rayDirection;
                    collisionInfo.hit = hit;
                    if (rayDirection == -1) {
                        collisionInfo.collisionBelow = true;
                    } else {
                        collisionInfo.collisionAbove = true;
                    }
                    break;
                }
            }
        }

        return ref collisionInfo;
    }

    // CheckAll is similar to Check, except that it registers ALL unique collisions detected from all raycasts.
    public List<CollisionInfo> CheckAll(Vector2 moveVector) {
        return CheckAll(moveVector, collisionMask);
    } 
    public List<CollisionInfo> CheckAll(Vector2 moveVector, LayerMask collisionMask) {
        List<CollisionInfo> collisionInfos = new List<CollisionInfo>();

        UpdateRaycastOrigins();
        // Reuse the same collisionInfo within this class, as this function is called constantly each frame.
        collisionInfo.Reset(moveVector);

        HashSet<Collider2D> colliders;

        // Horizontal collision check.
        if (moveVector.x != 0) {
            // Create a HashSet to keep track of colliders hit.
            colliders = new HashSet<Collider2D>();

            // Set up rays.
            float rayDirection = Mathf.Sign(moveVector.x);
            float rayDistance = Mathf.Abs(moveVector.x) + SKIN;
            Vector2 startOrigin, endOrigin;
            if (rayDirection == -1) {
                startOrigin = raycastOrigins.bottomLeft;
                endOrigin = raycastOrigins.topLeft;
            } else {
                startOrigin = raycastOrigins.bottomRight;
                endOrigin = raycastOrigins.topRight;
            }
            
            // Cast rays.
            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D[] hits = Helpers.RaycastAllWithDebug(raycastOrigin, rayDirection * Vector2.right, rayDistance, collisionMask);

                foreach (RaycastHit2D hit in hits) {
                    // Skip if we already processed this collider (in another raycast).
                    if (colliders.Contains(hit.collider)) {
                        continue;
                    }
                    colliders.Add(hit.collider);
                    
                    // Track everything about the collision.
                    CollisionInfo collInfo = new CollisionInfo();
                    collInfo.colliderHorizontal = hit.collider;
                    collInfo.moveVector.x = (hit.distance - SKIN) * rayDirection;
                    if (rayDirection == -1) {
                        collInfo.collisionLeft = true;
                    } else {
                        collInfo.collisionRight = true;
                    }
                    collisionInfos.Add(collInfo);
                }
            }
        }

        // Vertical collision check.
        if (moveVector.y != 0) {
            // Create a HashSet to keep track of colliders hit.
            colliders = new HashSet<Collider2D>();
            
            // Set up rays.
            float rayDirection = Mathf.Sign(moveVector.y);
            float rayDistance = Mathf.Abs(moveVector.y) + SKIN;
            Vector2 startOrigin, endOrigin;
            if (rayDirection == -1) {
                startOrigin = raycastOrigins.bottomRight;
                endOrigin = raycastOrigins.bottomLeft;
            } else {
                startOrigin = raycastOrigins.topRight;
                endOrigin = raycastOrigins.topLeft;
            }
            // Vertical collisions are checked after horizontal collisions, so the rays should be casted from where the object will be horizontally.
            startOrigin.x += collisionInfo.moveVector.x;
            endOrigin.x += collisionInfo.moveVector.x;

            // Cast rays.
            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D[] hits = Helpers.RaycastAllWithDebug(raycastOrigin, rayDirection * Vector2.up, rayDistance, collisionMask);

                foreach (RaycastHit2D hit in hits) {
                    if (colliders.Contains(hit.collider)) {
                        continue;
                    }
                    colliders.Add(hit.collider);
                    
                    // Track everything about the collision.
                    CollisionInfo collInfo = new CollisionInfo();
                    collInfo.colliderVertical = hit.collider;
                    collInfo.moveVector.y = (hit.distance - SKIN) * rayDirection;
                    if (rayDirection == -1) {
                        collInfo.collisionBelow = true;
                    } else {
                        collInfo.collisionAbove = true;
                    }
                    collisionInfos.Add(collInfo);
                }
            }
        }

        return collisionInfos;
    }

    // Used to keep the set of raycast origins nice and organized.
    private struct RaycastOrigins {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    // Used to recalculate the dimensions of the collider.
    public void UpdateColliderDimensions() {
        colliderHalfDims = new Vector2(boxCollider.size.x / 2 * transform.localScale.x - SKIN, boxCollider.size.y / 2 * transform.localScale.y - SKIN);
    }

    private void UpdateRaycastOrigins() {
        Vector2 center = (Vector2)transform.position + boxCollider.offset;
        // Half the amount of both width and length of the box, minus skinWidth.
        if (updateColliderDimension) {
            UpdateColliderDimensions();
        }
        raycastOrigins.bottomLeft = center + new Vector2(-colliderHalfDims.x, -colliderHalfDims.y);
		raycastOrigins.bottomRight = center + new Vector2(colliderHalfDims.x, -colliderHalfDims.y);
		raycastOrigins.topLeft = center + new Vector2(-colliderHalfDims.x, colliderHalfDims.y);
		raycastOrigins.topRight = center + new Vector2(colliderHalfDims.x, colliderHalfDims.y);
    }
}
