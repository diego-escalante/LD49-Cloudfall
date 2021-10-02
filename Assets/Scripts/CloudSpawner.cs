using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The CloudSpawner needs to be more elegant than just randomly spawning clouds everywhere in order to make the game
 * fair by making it possible to always jump on the next cloud. In order to do this, the spawner needs to be aware
 * of how high the player can jump, and make sure that there's always a cloud reachable from the previous cloud.
 * At the same time, the spawner must not spawn two clouds overlapping each other, to keep the play area less noisy and
 * more readable to the player at a glance.
 *
 * To do this, the CloudSpawner divides the sky into "rows" of a particular width that a cloud can spawn within.
 * Each row gets it's own cloud that loops over the screen horizontally. Between each row is a spawn "deadzone" that
 * prevents a cloud spawning at the top of row n to overlap with a cloud spawning at the bottom of row n+1.
 *
 * Using the player's jump height to calculate these rows, we can guarantee that the max height difference between two
 * clouds (one spawning at the bottom of row n and one spawning at the top of row n+1) is always traversable by the
 * player.
 */
public class CloudSpawner : MonoBehaviour {

    [SerializeField] private GameObject cloudPrefab;

    private float _spawnXDistance; // Precise horizontal distance to spawn right outside of the screen.
    private Camera cam;
    private float playerJumpHeightHalf;
    private float cloudHeightHalf;

    private void Awake() {
        cam = Camera.main;
        // Calculation assumes the camera is always at x = 0.
        _spawnXDistance = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + cloudPrefab.transform.localScale.x / 2;
    }

    private void Start() {
        playerJumpHeightHalf = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().GetJumpHeight() / 2;
        cloudHeightHalf = cloudPrefab.transform.localScale.y / 2;
        
        // Generate a bunch of clouds, this can be optimized later if needed by only spawning those in view.
        CloudRow cloudRow;
        for (int i = 1; i <= 100; i++) {
            cloudRow = calculateCloudRow(i);
            float height = Random.Range(cloudRow.yMin, cloudRow.yMax);
            SpawnCloud(height, Mathf.Sign(Random.Range(-1, 1)), calculateSpeed(height));
        }
    }
    
    private CloudRow calculateCloudRow(int row) {
        var cloudRow = new CloudRow();
        cloudRow.yMin = playerJumpHeightHalf * (row - 1) + cloudHeightHalf;
        cloudRow.yMax = playerJumpHeightHalf * row - cloudHeightHalf;
        return cloudRow;
    }

    private void SpawnCloud(float height, float side, float speed) {
        Vector3 spawnPos = new Vector3(_spawnXDistance * side, height, 0);

        GameObject newCloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
        newCloud.GetComponent<CloudMovement>().SetSpeed(speed * side * -1);
    }

    // linear relationship between height and speed.
    private float calculateSpeed(float height) {
        return (height + 5) / 10f;
    }

    private struct CloudRow {
        public float yMin;
        public float yMax;
    }
}
