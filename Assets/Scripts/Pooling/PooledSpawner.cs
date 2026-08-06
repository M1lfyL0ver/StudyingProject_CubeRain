using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledSpawner<T> : SpawnerStatisticsSource
    where T : MonoBehaviour, IPoolable<T>
{
    [SerializeField] private T _prefab;
    [SerializeField, Min(1)] private int _defaultCapacity = 10;
    [SerializeField, Min(1)] private int _maxPoolSize = 100;

    private ObjectPool<T> _objectPool;
    private int _spawnedObjectsCount;

    public override int SpawnedObjectsCount => _spawnedObjectsCount;

    public override int CreatedObjectsCount =>
        _objectPool == null ? 0 : _objectPool.CountAll;

    public override int ActiveObjectsCount =>
        _objectPool == null ? 0 : _objectPool.CountActive;

    protected virtual void Awake()
    {
        int maxPoolSize = Mathf.Max(_defaultCapacity, _maxPoolSize);

        _objectPool = new ObjectPool<T>(
            CreateObject,
            null,
            DisableObject,
            DestroyObject,
            true,
            _defaultCapacity,
            maxPoolSize);
    }

    protected T Spawn(Vector3 position, Quaternion rotation)
    {
        T pooledObject = _objectPool.Get();

        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.gameObject.SetActive(true);
        pooledObject.PrepareForSpawn();

        _spawnedObjectsCount++;
        NotifyStatisticsChanged();

        return pooledObject;
    }

    protected virtual void AfterObjectReleased(T pooledObject)
    {
    }

    private T CreateObject()
    {
        T pooledObject = Instantiate(_prefab);

        pooledObject.gameObject.SetActive(false);
        pooledObject.ReleaseRequested += ReleaseObject;

        return pooledObject;
    }

    private void ReleaseObject(T pooledObject)
    {
        pooledObject.PrepareForRelease();
        _objectPool.Release(pooledObject);

        AfterObjectReleased(pooledObject);
        NotifyStatisticsChanged();
    }

    private void DisableObject(T pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    private void DestroyObject(T pooledObject)
    {
        pooledObject.ReleaseRequested -= ReleaseObject;
        Destroy(pooledObject.gameObject);
    }
}