using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
[RequireComponent(typeof(SphereCollider), typeof(BombExplosion))]
public sealed class Bomb : MonoBehaviour, IPoolable<Bomb>
{
    private static readonly int SurfaceProperty =
        Shader.PropertyToID("_Surface");

    private static readonly int ModeProperty =
        Shader.PropertyToID("_Mode");

    private static readonly int SourceBlendProperty =
        Shader.PropertyToID("_SrcBlend");

    private static readonly int DestinationBlendProperty =
        Shader.PropertyToID("_DstBlend");

    private static readonly int WriteDepthProperty =
        Shader.PropertyToID("_ZWrite");

    [SerializeField, Min(0f)] private float _minExplosionDelay = 2f;
    [SerializeField, Min(0f)] private float _maxExplosionDelay = 5f;
    [SerializeField] private Color _bombColor = Color.black;

    private Material _material;
    private Rigidbody _rigidbody;
    private BombExplosion _bombExplosion;
    private Coroutine _explosionCoroutine;

    public event Action<Bomb> ReleaseRequested;

    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        _rigidbody = GetComponent<Rigidbody>();
        _bombExplosion = GetComponent<BombExplosion>();
        _material = meshRenderer.material;
    }

    private void OnDestroy()
    {
        Destroy(_material);
    }

    private void OnValidate()
    {
        _maxExplosionDelay = Mathf.Max(
            _minExplosionDelay,
            _maxExplosionDelay);
    }

    public void PrepareForSpawn()
    {
        StopExplosionCoroutine();

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        SetOpaqueRenderMode();
        SetAlpha(1f);

        float explosionDelay = Random.Range(
            _minExplosionDelay,
            _maxExplosionDelay);

        _explosionCoroutine = StartCoroutine(
            FadeAndExplode(explosionDelay));
    }

    public void PrepareForRelease()
    {
        StopExplosionCoroutine();
    }

    private IEnumerator FadeAndExplode(float duration)
    {
        SetTransparentRenderMode();

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(
                elapsedTime / duration);

            SetAlpha(alpha);

            yield return null;
        }

        _bombExplosion.Explode();
        _explosionCoroutine = null;

        ReleaseRequested?.Invoke(this);
    }

    private void SetAlpha(float alpha)
    {
        Color color = _bombColor;
        color.a = alpha;

        _material.color = color;
    }

    private void SetOpaqueRenderMode()
    {
        SetMaterialFloat(SurfaceProperty, 0f);
        SetMaterialFloat(ModeProperty, 0f);
        SetMaterialFloat(SourceBlendProperty, (float)BlendMode.One);
        SetMaterialFloat(DestinationBlendProperty, (float)BlendMode.Zero);
        SetMaterialFloat(WriteDepthProperty, 1f);

        _material.SetOverrideTag("RenderType", "Opaque");
        _material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _material.DisableKeyword("_ALPHATEST_ON");
        _material.DisableKeyword("_ALPHABLEND_ON");
        _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        _material.renderQueue = (int)RenderQueue.Geometry;
    }

    private void SetTransparentRenderMode()
    {
        SetMaterialFloat(SurfaceProperty, 1f);
        SetMaterialFloat(ModeProperty, 2f);
        SetMaterialFloat(
            SourceBlendProperty,
            (float)BlendMode.SrcAlpha);

        SetMaterialFloat(
            DestinationBlendProperty,
            (float)BlendMode.OneMinusSrcAlpha);

        SetMaterialFloat(WriteDepthProperty, 0f);

        _material.SetOverrideTag("RenderType", "Transparent");
        _material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _material.DisableKeyword("_ALPHATEST_ON");
        _material.EnableKeyword("_ALPHABLEND_ON");
        _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        _material.renderQueue = (int)RenderQueue.Transparent;
    }

    private void SetMaterialFloat(int propertyId, float value)
    {
        if (_material.HasProperty(propertyId))
            _material.SetFloat(propertyId, value);
    }

    private void StopExplosionCoroutine()
    {
        if (_explosionCoroutine == null)
            return;

        StopCoroutine(_explosionCoroutine);
        _explosionCoroutine = null;
    }
}