using System;
using UnityEngine;

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
                if (surface.Orientation == Orientation.Horizontal && DrawnObject.TargetSurfaceType==Orientation.Horizontal)
                {
                    CalculateHorinztalPosition(surface, hit.point);
                }
                else if (surface.Orientation == Orientation.Vertical && DrawnObject.TargetSurfaceType == Orientation.Vertical)
                {
                    CalculateVerticalPosition(surface, hit.point);
                }
                currentSurface = surface;
            }
            else if (currentSurface != null)
            {
                if (currentSurface.Orientation == Orientation.Horizontal && DrawnObject.TargetSurfaceType == Orientation.Horizontal)
                {
                    CalculateHorinztalPosition(currentSurface, hit.point);
                }
                else if(currentSurface.Orientation == Orientation.Vertical && DrawnObject.TargetSurfaceType == Orientation.Vertical)
                {
                    CalculateVerticalPosition(currentSurface, hit.point);
                }
            }
        }
    }

    private void CalculateVerticalPosition(BaseSurfaceGrid surface, Vector3 hitPoint)
    {
        Axis secondAxis = surface.GridSize.x > 0 ? Axis.X : Axis.Z;

        Vector3 cornerPositionWorld = hitPoint - offset;

        SurfaceCollideData collideData = new SurfaceCollideData(surface, DrawnObject, secondAxis == Axis.X ? Plane.XY : Plane.YZ);
        Vector3 resultPos = collideData.GetResultPosition(cornerPositionWorld);

        if (collideData.NoProbmlemsWithPlacing())
        {
            if (secondAxis == Axis.X)
            {
                DrawnObject.transform.position = new Vector3(resultPos.x, resultPos.y, surface.transform.position.z);
            }
            else
            {
                DrawnObject.transform.position = new Vector3(surface.transform.position.x, resultPos.y, resultPos.z);
            }
            DrawnObject.transform.eulerAngles = surface.TargetObjectsRotation;
        }
        DrawnObject.CanBePlaced = surface.Orientation == DrawnObject.TargetSurfaceType;
        lastSuitablePosition = DrawnObject.transform.position;
        DrawnObject.LastCorrectSurface = surface;
    }

    private void CalculateHorinztalPosition(BaseSurfaceGrid surface, Vector3 hitPoint)
    {
        Vector3 cornerPositionWorld = hitPoint - offset;

        SurfaceCollideData collideData = new SurfaceCollideData(surface,DrawnObject, Plane.XZ);
        Vector3 resultPos = collideData.GetResultPosition(cornerPositionWorld);

        if (collideData.NoProbmlemsWithPlacing())
        {
            DrawnObject.transform.position = new Vector3(resultPos.x, surface.transform.position.y, resultPos.z);
            DrawnObject.transform.eulerAngles = surface.TargetObjectsRotation;
        }
        else if (surface.HelpPlacingObjects)
        {
            if (surface.PreferedAxis == Axis.X)
            {
                float fixedZ = cornerPositionWorld.z > surface.transform.position.z ? surface.transform.position.z+surface.GridSize.z-DrawnObject.Size.z : surface.transform.position.z;
                DrawnObject.transform.position = new Vector3(resultPos.x, surface.transform.position.y, fixedZ);
            }
            else
            {
                float fixedX = cornerPositionWorld.x > surface.transform.position.x + surface.GridSize.x/2f ? surface.transform.position.x + surface.GridSize.x - DrawnObject.Size.x : surface.transform.position.x;
                DrawnObject.transform.position = new Vector3(fixedX, surface.transform.position.y, resultPos.z);
            }
            DrawnObject.transform.eulerAngles = surface.TargetObjectsRotation;
        }
        DrawnObject.CanBePlaced = surface.Orientation == DrawnObject.TargetSurfaceType;
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
                   // offset = hit.point - DrawnObject.transform.position;
                    //offset.y = 0;
                    DrawnObject.Collider.enabled = false;
                    DrawnObject.transform.parent = null;
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