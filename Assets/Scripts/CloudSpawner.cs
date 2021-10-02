using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This class instantiates a cloud every single time. To increase performance, switch this to use an object pool.
// But only if necessary. An object pool is not needed right now.
public class CloudSpawner : MonoBehaviour {

    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private float spawnRate = 1;
    [SerializeField] private float spawnRateDelta = 0.5f;

    private float _spawnXDistance;
    private Coroutine co;
    private Camera cam;

    private void Awake() {
        cam = Camera.main;
        // Calculation assumes the camera is always at x = 0.
        _spawnXDistance = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + cloudPrefab.transform.localScale.x / 2;
    }

    private void OnEnable() {
        co = StartCoroutine(Spawn());
    }

    private void OnDisable() {
        if (co != null) {
            StopCoroutine(co);
        }
    }

    private IEnumerator Spawn() {
        // Determine spawn height range based on current viewport.
        float minY = cam.ViewportToWorldPoint(Vector3.zero).y;
        float maxY = cam.ViewportToWorldPoint(Vector3.one * 1.33f).y;
        float height = Random.Range(minY, maxY);
        // Make sure no clouds spawn at by the ground.
        height = Mathf.Max(2, height);
        
        //TODO: Set speed according to height.
        SpawnCloud(height, Mathf.Sign(Random.Range(-1,1)), calculateSpeed(height));

        yield return new WaitForSeconds(spawnRate + Random.Range(-spawnRateDelta, spawnRateDelta));
        co = StartCoroutine(Spawn());
    }

    private void SpawnCloud(float height, float side, float speed) {
        Vector3 spawnPos = new Vector3(_spawnXDistance * side, height, 0);

        GameObject newCloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
        newCloud.GetComponent<CloudMovement>().SetSpeed(speed * side * -1);
    }

    // linear
    private float calculateSpeed(float height) {
        return (height + 5) / 10f;
    }
}
