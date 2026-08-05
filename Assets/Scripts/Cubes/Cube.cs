using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public sealed class Cube : PooledObject
{
    [SerializeField, Min(0f)] private float _minLifeTimeAfterCollision = 2f;
    [SerializeField, Min(0f)] private float _maxLifeTimeAfterCollision = 5f;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _collisionColor = Color.red;

    private MeshRenderer _meshRenderer;
    private Material _material;
    private Rigidbody _rigidbody;
    private Coroutine _releaseCoroutine;
    private bool _hasCollidedWithPlatform;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _material = _meshRenderer.material;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCollidedWithPlatform)
            return;

        if (collision.collider.GetComponentInParent<Platform>() == null)
            return;

        _hasCollidedWithPlatform = true;
        _material.color = _collisionColor;

        float lifeTime = Random.Range(_minLifeTimeAfterCollision, _maxLifeTimeAfterCollision);
        _releaseCoroutine = StartCoroutine(RequestReleaseAfterDelay(lifeTime));
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

    public override void PrepareForSpawn()
    {
        StopReleaseCoroutine();

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _material.color = _activeColor;
        _hasCollidedWithPlatform = false;
    }

    public override void PrepareForRelease()
    {
        StopReleaseCoroutine();
    }

    private IEnumerator RequestReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _releaseCoroutine = null;
        RequestRelease();
    }

    private void StopReleaseCoroutine()
    {
        if (_releaseCoroutine == null)
            return;

        StopCoroutine(_releaseCoroutine);
        _releaseCoroutine = null;
    }
}