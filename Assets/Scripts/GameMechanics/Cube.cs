using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    [SerializeField] private int _minLifeTimeAfterCollision = 2;
    [SerializeField] private int _maxLifeTimeAfterCollision = 5;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _collisionColor;

    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;
    private int _lifeTimeAfterCollision;
    private bool _isCollided;

    public event Action<Cube> LifeTimeEnded;

    private void OnEnable()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _lifeTimeAfterCollision = Random.Range(_minLifeTimeAfterCollision, _maxLifeTimeAfterCollision);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        _renderer.material.color = _activeColor;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _isCollided = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Platform>(out _) && _isCollided == false)
        {
            _renderer.material.color = _collisionColor;
            StartCoroutine(DestroyAfterDelay(_lifeTimeAfterCollision));
            _isCollided = true;
        }
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        LifeTimeEnded?.Invoke(this);
    }
}