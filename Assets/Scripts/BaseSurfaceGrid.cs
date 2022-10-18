using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z
}

public class BaseSurfaceGrid : MonoBehaviour
{
    public Axis PreferedAxis;
    public DrawableObject ParentDrawabaleObject;
    public Vector3 TargetObjectsRotation;
    public bool HelpPlacingObjects = true;

    public Vector3Int GridSize;

    private bool[,] CellIsFree;

    public Orientation Orientation;

    /* private void Awake()
     {
         CellIsFree = new bool[GridSize.x * CellsInOneUnit, GridSize.y * CellsInOneUnit] ;
         for (int i = 0; i < GridSize.x * CellsInOneUnit; i++)
         {
             for(int j = 0; j < GridSize.y * CellsInOneUnit; j++)
             {
                 CellIsFree[i,j] = true;
             }
         }
     }

     public void OnDrawStarted(DrawableObject obj)
     {
         for (int i = (int)(obj.transform.position.x* CellsInOneUnit); i < (int)((obj.transform.position.x + obj.Size.x) * CellsInOneUnit); i++)
         {
             for (int j = (int)(obj.transform.position.z * CellsInOneUnit); j < (int)((obj.transform.position.z + obj.Size.z) * CellsInOneUnit); j++)
             {
                 CellIsFree[i, j] = true;
             }
         }
     }

     public void OnDrawFinished(DrawableObject obj)
     {
         for (int i = (int)(obj.transform.position.x * CellsInOneUnit); i < (int)((obj.transform.position.x + obj.Size.x) * CellsInOneUnit); i++)
         {
             for (int j = (int)(obj.transform.position.z * CellsInOneUnit); j < (int)((obj.transform.position.z + obj.Size.z) * CellsInOneUnit); j++)
             {
                 CellIsFree[i, j] = false;
             }
         }
     }

     public bool PlaceIsFree(DrawableObject drawnObject)
     {
         for (int i = (int)(drawnObject.transform.position.x * CellsInOneUnit); i < (int)((drawnObject.transform.position.x * CellsInOneUnit)+ drawnObject.Size.x); i++)
         {
             for (int j = (int)(drawnObject.transform.position.z * CellsInOneUnit); j < (int)((drawnObject.transform.position.z * CellsInOneUnit) + drawnObject.Size.z); j++)
             {
                 if (!CellIsFree[i + (int)(transform.position.x*CellsInOneUnit), j + (int)(transform.position.z * CellsInOneUnit)]) return false;
             }
         }
         return true;
     }*/
}
