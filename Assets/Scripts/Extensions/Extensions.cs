using UnityEngine;
using UnityEngine.SceneManagement;

public static class Extensions{
    // Contains checks if a layer is included in a LayerMask.
    public static bool Contains(this LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }

    // Overlaps checks if this LayerMask contains at least another layer on the other LayerMask.
    public static bool Overlaps(this LayerMask mask, LayerMask other) {
        return 0 != (mask & other);
    }

    // Overlaps checks if a box collider overlaps with another.
    public static bool Overlaps(this BoxCollider2D coll, BoxCollider2D other) {
        Vector2 collMin = (Vector2)coll.transform.position + (coll.offset - coll.size/2) * coll.transform.localScale;
        Vector2 collMax = (Vector2)coll.transform.position + (coll.offset + coll.size/2) * coll.transform.localScale;

        Vector2 otherMin = (Vector2)other.transform.position + (other.offset - other.size/2) * other.transform.localScale;
        Vector2 otherMax = (Vector2)other.transform.position + (other.offset + other.size/2) * other.transform.localScale;

        return collMin.x < otherMax.x && otherMin.x < collMax.x && collMin.y < otherMax.y && otherMin.y < collMax.y;
    }

    public static void LoadNextScene(this SceneManager sceneManager) {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
