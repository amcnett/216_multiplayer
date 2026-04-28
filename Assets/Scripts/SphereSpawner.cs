using UnityEngine;
using Unity.Netcode;
using System;

public class SphereSpawner : NetworkBehaviour
{
    public GameObject spherePrefab;
    public float spawnInterval = 0.5f;
    public float spawnAreaSize = 10f;
    public float spawnHeight = 10f;

    private float timer;

    void Update()
    {
        if (!IsServer)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnSphere();
        }
    }

    private void SpawnSphere()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize), spawnHeight, UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize));

        GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity);
        sphere.GetComponent<NetworkObject>().Spawn();
    }
}