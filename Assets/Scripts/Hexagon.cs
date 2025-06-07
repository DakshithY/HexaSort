using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Collider collider;
    public Hexstack HexStack { get; private set; }
    public Color Color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
    }

    public void DisableColliders() => collider.enabled = false;

    public void Configure(Hexstack hexastack)
    {
        HexStack = hexastack;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.zero, .5f).setEase(LeanTweenType.easeInBack).setDelay(delay).setOnComplete(() => DestroyImmediate(gameObject));
    }

    public void MoveToLocal(Vector3 TargetLocalPos)
    {
        float delay = transform.GetSiblingIndex() * .01f;
        LeanTween.moveLocal(gameObject, TargetLocalPos, 0.2f).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
        Vector3 y = new Vector3(transform.rotation.x, 0, transform.rotation.z);
        Vector3 Direction = (TargetLocalPos - transform.localPosition).With(y: 0).normalized;
        Vector3 rotationaxis = Vector3.Cross(Vector3.up, Direction);
        LeanTween.rotateAround(gameObject, rotationaxis, 180, .2f).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
    }
}
