using UnityEngine;

public sealed class BombSpawner : PooledSpawner<Bomb>
{
    [SerializeField] private CubeSpawner _cubeSpawner;

    private void OnEnable()
    {
        if (_cubeSpawner != null)
            _cubeSpawner.CubeReleased += SpawnBomb;
    }

    private void OnDisable()
    {
        if (_cubeSpawner != null)
            _cubeSpawner.CubeReleased -= SpawnBomb;
    }

    private void SpawnBomb(Vector3 position)
    {
        Spawn(position, Quaternion.identity);
    }
}