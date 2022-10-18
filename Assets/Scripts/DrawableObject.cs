using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    Horizontal,
    Vertical
}

public class DrawableObject : MonoBehaviour
{
    [SerializeField] private BaseSurfaceGrid defaultSurface;

    public bool CanBePlaced;
    public Vector3 Size;
    public Transform Model;
    public Collider Collider;
    public Collider Surface;

    public BaseSurfaceGrid LastCorrectSurface;

    public Orientation TargetSurfaceType;

    private void Awake()
    {
        LastCorrectSurface = defaultSurface;
    }
}
