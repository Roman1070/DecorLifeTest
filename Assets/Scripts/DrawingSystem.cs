using System;
using UnityEngine;

public struct SurfaceCollideData
{
    public Vector3 CornerPosition;
    public Vector3 OppositeCornerPosition;
    public Vector3 ResultPosition;
    public RangeFloat[] SurfaceBorders;

    private BaseSurfaceGrid surface;
    private Vector3 objectSize;

    public SurfaceCollideData(BaseSurfaceGrid surface, Vector3 objectSize)
    {
        this.objectSize = objectSize;
        this.surface = surface;
        CornerPosition = default;
        OppositeCornerPosition = default;
        ResultPosition = default;
        SurfaceBorders = default;
    }

    public void SetCornerPosition(Vector3 unrounded)
    {
        float x = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.x), 1));
        float y = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.y), 1));
        float z = Convert.ToSingle(Math.Round(Convert.ToDouble(unrounded.z), 1));
        CornerPosition = new Vector3(x, y, z);
    }

    public void SetOppositeCornerPosition(Vector3 objectSize) => OppositeCornerPosition = CornerPosition + objectSize;

    public void SetSurfaceBorders()
    {
        RangeFloat XRange = new RangeFloat { Min = surface.transform.position.x, Max = surface.transform.position.x + surface.GridSize.x };
        RangeFloat YRange = new RangeFloat { Min = surface.transform.position.y, Max = surface.transform.position.y + surface.GridSize.y };
        RangeFloat ZRange = new RangeFloat { Min = surface.transform.position.z, Max = surface.transform.position.z + surface.GridSize.z };

        SurfaceBorders = new RangeFloat[3] {XRange,YRange,ZRange };
    }

    public Vector3 GetResultPosition()
    {
        float x = Mathf.Clamp(CornerPosition.x, SurfaceBorders[0].Min, SurfaceBorders[0].Max - objectSize.x);
        float y = Mathf.Clamp(CornerPosition.y, SurfaceBorders[1].Min, SurfaceBorders[1].Max - objectSize.y);
        float z = Mathf.Clamp(CornerPosition.z, SurfaceBorders[2].Min, SurfaceBorders[2].Max - objectSize.z);

        return new Vector3(x, y, z);
    }
}

public class DrawingSystem : MonoBehaviour
{
    private DrawableObject DrawnObject;

    private Camera mainCamera;
    private Vector3 offset;
    private Vector3 lastSuitablePosition;
    private BaseSurfaceGrid currentSurface;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (DrawnObject == null)
        {
            CheckTakingObject();
        }
        else
        {
            DrawObject();
        }
        if (Input.GetMouseButtonUp(0) && DrawnObject != null)
        {
            PlaceObject();
        }
    }

    private void DrawObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        DrawnObject.CanBePlaced = false;
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.TryGetComponent<BaseSurfaceGrid>(out var surface) && surface.transform.parent != DrawnObject.transform)
            {
                if (surface.Orientation == Orientation.Horizontal)
                {
                    CalculateHorinztalPosition(surface, hit.point);
                    currentSurface = surface;
                }
                else
                {

                }
            }
            else if (currentSurface != null)
            {
                if (currentSurface.Orientation == Orientation.Horizontal)
                {
                    CalculateHorinztalPosition(currentSurface, hit.point);
                }
                else
                {

                }
            }
        }
    }

    private void CalculateVerticalPosition(BaseSurfaceGrid surface, Vector3 hitPoint)
    {
        Vector3 cornerPositionWorld = hitPoint - offset;

        float startX = Convert.ToSingle(Math.Round(Convert.ToDouble(cornerPositionWorld.x), 1));
        float startY = Convert.ToSingle(Math.Round(Convert.ToDouble(cornerPositionWorld.y), 1));
        float startZ = Convert.ToSingle(Math.Round(Convert.ToDouble(cornerPositionWorld.z), 1));

        RangeFloat XRange = new RangeFloat { Min = surface.transform.position.x, Max = surface.transform.position.x + surface.GridSize.x };
        RangeFloat YRange = new RangeFloat { Min = surface.transform.position.y, Max = surface.transform.position.y + surface.GridSize.y };
        RangeFloat ZRange = new RangeFloat { Min = surface.transform.position.z, Max = surface.transform.position.z + surface.GridSize.z };


        float endX = startX + DrawnObject.Size.x;
        float endY = startY + DrawnObject.Size.y;
        float endZ = startZ + DrawnObject.Size.z;

        float x = Mathf.Clamp(startX, XRange.Min, XRange.Max - DrawnObject.Size.x);
        float y = Mathf.Clamp(startY, YRange.Min, YRange.Max - DrawnObject.Size.y);
        float z = Mathf.Clamp(startZ, ZRange.Min, ZRange.Max - DrawnObject.Size.z);

        Axis secondAxis = surface.GridSize.x > 0 ? Axis.X : Axis.Z;
    }

    private void CalculateHorinztalPosition(BaseSurfaceGrid surface, Vector3 hitPoint)
    {
        Vector3 cornerPositionWorld = hitPoint - offset;

        float startX = Convert.ToSingle(Math.Round(Convert.ToDouble(cornerPositionWorld.x), 1));
        float startZ = Convert.ToSingle(Math.Round(Convert.ToDouble(cornerPositionWorld.z), 1));

        RangeFloat XRange = new RangeFloat { Min = surface.transform.position.x, Max = surface.transform.position.x + surface.GridSize.x };
        RangeFloat ZRange = new RangeFloat { Min = surface.transform.position.z, Max = surface.transform.position.z + surface.GridSize.z };

        float endX = startX + DrawnObject.Size.x;
        float endZ = startZ + DrawnObject.Size.z;

        float x = Mathf.Clamp(startX, XRange.Min, XRange.Max - DrawnObject.Size.x);
        float z = Mathf.Clamp(startZ, ZRange.Min, ZRange.Max - DrawnObject.Size.z);

        if (startX >= XRange.Min && startZ >= ZRange.Min && endX <= XRange.Max && endZ <= ZRange.Max)
        {
            DrawnObject.transform.position = new Vector3(x, surface.transform.position.y, z);
        }
        else
        {
            if (surface.PreferedAxis == Axis.X)
            {
                float fixedZ = cornerPositionWorld.z > surface.transform.position.z ? surface.transform.position.z+surface.GridSize.z-DrawnObject.Size.z : surface.transform.position.z;
                DrawnObject.transform.position = new Vector3(x, surface.transform.position.y, fixedZ);
            }
            else
            {
                float fixedX = cornerPositionWorld.x > surface.transform.position.x ? surface.transform.position.x + surface.GridSize.x - DrawnObject.Size.x : surface.transform.position.x;
                DrawnObject.transform.position = new Vector3(fixedX, surface.transform.position.y, z);
            }
        }
        DrawnObject.CanBePlaced = true;
        lastSuitablePosition = DrawnObject.transform.position;
        DrawnObject.LastCorrectSurface = surface;
    }

    private void CheckTakingObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, LayerMask.GetMask("Drawable")))
            {
                if (hit.collider.TryGetComponent<DrawableObject>(out var obj))
                {
                    DrawnObject = obj;

                    lastSuitablePosition = DrawnObject.transform.position;
                    offset = hit.point - DrawnObject.transform.position;
                    offset.y = 0;
                    DrawnObject.Collider.enabled = false;
                    DrawnObject.Surface.enabled = false;
                }
            }
        }
    }

    private void PlaceObject()
    {
        if (!DrawnObject.CanBePlaced) DrawnObject.transform.position = lastSuitablePosition;

        DrawnObject.Collider.enabled = true;
        DrawnObject.Surface.enabled = true;
        DrawnObject.transform.SetParent(DrawnObject.LastCorrectSurface.transform);
        DrawnObject = null;
    }
}