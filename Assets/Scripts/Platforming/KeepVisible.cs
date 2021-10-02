using UnityEngine;

/**
 * Currently only works in the x axis. A more complete class would have the option to apply in either x and/or y axis,
 * but no need to over-engineer and build what isn't needed yet.
 */
public class KeepVisible : MonoBehaviour {
    private Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    public void LateUpdate() {
        Vector3 worldPos = transform.position;
        // The distance from the center of the object to one of its sides.
        float extentX = transform.localScale.x / 2f;
        
        // Handle the left.
        Vector2 viewPosLeft = cam.WorldToViewportPoint(worldPos - new Vector3(extentX, 0, 0));
        if (viewPosLeft.x < 0) {
            viewPosLeft.x = 0;
            worldPos.x = cam.ViewportToWorldPoint(viewPosLeft).x + extentX;
            transform.position = worldPos;
            return;
        }
        
        // Handle the right.
        Vector2 viewPosRight = cam.WorldToViewportPoint(worldPos + new Vector3(extentX, 0, 0));
        if (viewPosRight.x > 1) {
            viewPosRight.x = 1;
            worldPos.x = cam.ViewportToWorldPoint(viewPosRight).x - extentX;
            transform.position = worldPos;
            return;
        }
    }
}
