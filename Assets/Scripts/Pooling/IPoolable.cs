using System;

public interface IPoolable<T> where T : IPoolable<T>
{
    public event Action<T> ReleaseRequested;

    public void PrepareForSpawn();

    public void PrepareForRelease();
}