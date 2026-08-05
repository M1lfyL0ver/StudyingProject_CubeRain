using System;
using UnityEngine;

public abstract class SpawnerStatisticsSource : MonoBehaviour
{
    public event Action StatisticsChanged;

    public abstract int SpawnedObjectsCount { get; }

    public abstract int CreatedObjectsCount { get; }

    public abstract int ActiveObjectsCount { get; }

    protected void NotifyStatisticsChanged()
    {
        StatisticsChanged?.Invoke();
    }
}