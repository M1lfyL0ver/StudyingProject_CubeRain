using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public sealed class Cube : MonoBehaviour, IPoolable<Cube>
{
    [SerializeField, Min(0f)] private float _minLifeTimeAfterCollision = 2f;
    [SerializeField, Min(0f)] private float _maxLifeTimeAfterCollision = 5f;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _collisionColor = Color.red;

    private Material _material;
    private Rigidbody _rigidbody;
    private Coroutine _releaseCoroutine;
    private bool _hasCollidedWithPlatform;

    public event Action<Cube> ReleaseRequested;

    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        _rigidbody = GetComponent<Rigidbody>();
        _material = meshRenderer.material;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCollidedWithPlatform)
            return;

        if (collision.collider.TryGetComponent(out Platform _) == false)
            return;

        _hasCollidedWithPlatform = true;
        _material.color = _collisionColor;

        float lifeTime = Random.Range(
            _minLifeTimeAfterCollision,
            _maxLifeTimeAfterCollision);

        _releaseCoroutine = StartCoroutine(
            RequestReleaseAfterDelay(lifeTime));
    }

    private void OnDestroy()
    {
        Destroy(_material);
    }

    private void OnValidate()
    {
        _maxLifeTimeAfterCollision = Mathf.Max(
            _minLifeTimeAfterCollision,
            _maxLifeTimeAfterCollision);
    }

    public void PrepareForSpawn()
    {
        StopReleaseCoroutine();

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _material.color = _activeColor;
        _hasCollidedWithPlatform = false;
    }

    public void PrepareForRelease()
    {
        StopReleaseCoroutine();
    }

    private IEnumerator RequestReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _releaseCoroutine = null;
        ReleaseRequested?.Invoke(this);
    }

    private void StopReleaseCoroutine()
    {
        if (_releaseCoroutine == null)
            return;

        StopCoroutine(_releaseCoroutine);
        _releaseCoroutine = null;
    }
}