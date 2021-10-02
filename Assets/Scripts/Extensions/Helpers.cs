using UnityEngine;

public class Helpers : MonoBehaviour {
    // Helper that draws rays casted in the editor to better debug.
    public static RaycastHit2D RaycastWithDebug(Vector2 origin, Vector2 direction, float distance, int layerMask) {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(origin, direction * distance, color);
        return hit;
    }

    // Helper that draws rays casted in the editor to better debug.
    public static RaycastHit2D[] RaycastAllWithDebug(Vector2 origin, Vector2 direction, float distance, int layerMask) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        Color color = hits.Length > 0 ? Color.red : Color.green;
        Debug.DrawRay(origin, direction * distance, color);
        return hits;
    }
}
