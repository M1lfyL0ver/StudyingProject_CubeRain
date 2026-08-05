using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CubeSpawner : PooledSpawner<Cube>
{
    [SerializeField, Min(0.01f)] private float _spawnInterval = 1f;

    private Coroutine _spawnCoroutine;

    public event Action<Vector3> CubeReleased;

    private void OnEnable()
    {
        _spawnCoroutine = StartCoroutine(SpawnCubes());
    }

    private void OnDisable()
    {
        if (_spawnCoroutine == null)
            return;

        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    protected override void AfterObjectReleased(Cube cube)
    {
        CubeReleased?.Invoke(cube.transform.position);
    }

    private IEnumerator SpawnCubes()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(_spawnInterval);

        while (true)
        {
            yield return spawnDelay;
            Spawn(transform.position, Random.rotation);
        }
    }
}