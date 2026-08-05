using System;
using UnityEngine;

public abstract class PooledObject : MonoBehaviour
{
    public event Action<PooledObject> ReleaseRequested;

    public abstract void PrepareForSpawn();

    public abstract void PrepareForRelease();

    protected void RequestRelease()
    {
        ReleaseRequested?.Invoke(this);
    }
}