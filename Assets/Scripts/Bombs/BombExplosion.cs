using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class BombExplosion : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _radius = 5f;
    [SerializeField, Min(0f)] private float _force = 500f;
    [SerializeField, Min(0f)] private float _upwardsModifier = 1f;
    [SerializeField] private LayerMask _affectedLayers = ~0;

    private readonly HashSet<Rigidbody> _affectedRigidbodies = new HashSet<Rigidbody>();
    private Rigidbody _bombRigidbody;

    private void Awake()
    {
        _bombRigidbody = GetComponent<Rigidbody>();
    }

    public void Explode()
    {
        _affectedRigidbodies.Clear();

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            _radius,
            _affectedLayers,
            QueryTriggerInteraction.Ignore);

        foreach (Collider foundCollider in colliders)
        {
            Rigidbody affectedRigidbody = foundCollider.attachedRigidbody;

            if (affectedRigidbody == null)
                continue;

            if (affectedRigidbody == _bombRigidbody)
                continue;

            if (affectedRigidbody.isKinematic)
                continue;

            if (_affectedRigidbodies.Add(affectedRigidbody) == false)
                continue;

            affectedRigidbody.AddExplosionForce(
                _force,
                transform.position,
                _radius,
                _upwardsModifier,
                ForceMode.Impulse);
        }
    }
}