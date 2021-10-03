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
    
    private float playerJumpHeightHalf;
    private float cloudHeightHalf;

    private void Start() {
        playerJumpHeightHalf = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().GetJumpHeight() / 2;
        cloudHeightHalf = cloudPrefab.transform.localScale.y / 2;
        
        // Generate a bunch of clouds, this can be optimized later if needed by only spawning those in view.
        for (int i = 2; i <= 200; i++) {
            SpawnCloud(CalculateCloudRow(i));
        }
    }
    
    private CloudRow CalculateCloudRow(int row) {
        return new CloudRow(
            playerJumpHeightHalf * (row - 1) + cloudHeightHalf,
            playerJumpHeightHalf * row - cloudHeightHalf);
    }

    private void SpawnCloud(CloudRow cloudRow) {
        GameObject newCloud = Instantiate(cloudPrefab);
        newCloud.GetComponent<CloudMovement>().InitializeCloud(cloudRow.yMin, cloudRow.yMax);
    }

    private struct CloudRow {
        public float yMin;
        public float yMax;

        public CloudRow(float yMin, float yMax) {
            this.yMin = yMin;
            this.yMax = yMax;
        }
    }
}
