using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private int _timeDelayCubeSpawn = 1;
    [SerializeField] private Cube _cubePrefab;

    private ObjectPool<Cube> _cubePool;

    private void Awake()
    {
        _cubePool = new ObjectPool<Cube>(
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: (cube) => cube.gameObject.SetActive(true),
            actionOnRelease: (cube) => cube.gameObject.SetActive(false),
            actionOnDestroy: (cube) => Destroy(cube.gameObject),
            defaultCapacity: 1);
    }

    private void Start()
    {
        StartCoroutine(SpawnEveryDelayTime(_timeDelayCubeSpawn));
    }

    private IEnumerator SpawnEveryDelayTime(int delay)
    {
        WaitForSeconds waitDelay = new WaitForSeconds(delay);

        while (enabled)
        {
            yield return waitDelay;
            Cube cube = _cubePool.Get();
            cube.transform.position = transform.position;
            cube.LifeTimeEnded += DeleteCube;
        }
    }

    private void DeleteCube(Cube cube)
    {
        cube.LifeTimeEnded -= DeleteCube;
        _cubePool.Release(cube);
    }
}