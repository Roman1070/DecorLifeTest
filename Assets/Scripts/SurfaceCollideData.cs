using System;
using UnityEngine;

public struct SurfaceCollideData
{
    public bool NoProbmlemsWithPlacing()
    {
        switch (plane)
        {
            case Plane.XZ:
                {
                    return CornerPosition.x >= SurfaceBorders[0].Min && CornerPosition.z >= SurfaceBorders[2].Min
                        && OppositeCornerPosition.x <= SurfaceBorders[0].Max && CornerPosition.z <= SurfaceBorders[2].Max;
                }
            case Plane.XY:
                {
                    return CornerPosition.x >= SurfaceBorders[0].Min && CornerPosition.y >= SurfaceBorders[1].Min
                        && OppositeCornerPosition.x <= SurfaceBorders[0].Max && CornerPosition.y <= SurfaceBorders[1].Max;
                }
            case Plane.YZ:
                {
                    return CornerPosition.y >= SurfaceBorders[1].Min && CornerPosition.z >= SurfaceBorders[2].Min
                        && OppositeCornerPosition.y <= SurfaceBorders[1].Max && CornerPosition.z <= SurfaceBorders[2].Max;
                }
        }
        return false;
    }

    private Vector3 CornerPosition;
    private Vector3 OppositeCornerPosition;
    private RangeFloat[] SurfaceBorders;

    private BaseSurfaceGrid surface;
    private DrawableObject _object;
    private Plane plane;

    public SurfaceCollideData(BaseSurfaceGrid surface, DrawableObject obj, Plane plane)
    {
        _object = obj;
        this.surface = surface;
        this.plane = plane;

        CornerPosition = default;
        OppositeCornerPosition = default;
        SurfaceBorders = default;
    }

    private void CalculateCornerPosition(Vector3 unrounded)
    {
        float x = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.x), 1));
        float y = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.y), 1));
        float z = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.z), 1));

        CornerPosition = new Vector3(x, y, z);
    }

    private void CalculateOppositeCornerPosition()
    {
        OppositeCornerPosition = CornerPosition + _object.OppositeCorner.position- _object.transform.position;
    }

    private void CalculateSurfaceBorders()
    {
        RangeFloat XRange = new RangeFloat { Min = surface.transform.position.x, Max = surface.transform.position.x + surface.GridSize.x };
        RangeFloat YRange = new RangeFloat { Min = surface.transform.position.y, Max = surface.transform.position.y + surface.GridSize.y };
        RangeFloat ZRange = new RangeFloat { Min = surface.transform.position.z, Max = surface.transform.position.z + surface.GridSize.z };

        SurfaceBorders = new RangeFloat[3] { XRange, YRange, ZRange };
    }

    public Vector3 GetResultPosition(Vector3 unroundedCornerPos)
    {
        CalculateCornerPosition(unroundedCornerPos);
        CalculateOppositeCornerPosition();
        CalculateSurfaceBorders();

        Vector3 actualObjectSize = OppositeCornerPosition - CornerPosition;

        float x = Mathf.Clamp(CornerPosition.x, SurfaceBorders[0].Min, SurfaceBorders[0].Max - actualObjectSize.x);
        float y = Mathf.Clamp(CornerPosition.y, SurfaceBorders[1].Min - actualObjectSize.y, SurfaceBorders[1].Max);
        float z = Mathf.Clamp(CornerPosition.z, SurfaceBorders[2].Min - actualObjectSize.z, SurfaceBorders[2].Max-actualObjectSize.z);

        return new Vector3(x, y, z);
    }
}
