using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 ballStartingPos;
    public Vector3 goalPos;
    public int par;
    public Tilemap tilemap;
    public Vector3 cameraOffset;
    public float cameraSize;
    public static float cameraDistance = 1.2f;
    public bool hasWind;

    public void ResetLevel()
    {
        GolfBall.Instance.velocity = Vector2.zero;
        GolfBall.Instance.transform.position = ballStartingPos;
        Game.Instance.Reset();
    }
    public void FitCamera()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int min = bounds.min; 
        Vector3Int max = bounds.max; 

        // Oblicz liczbę kolumn i wierszy
        int numberOfColumns = max.x - min.x + 1;
        int numberOfRows = max.y - min.y + 1;

        Vector3 center = new Vector3(0, (numberOfRows * tilemap.cellSize.y) / 2f);
        center += new Vector3(0, min.y);

        Camera.main.transform.position = new Vector3(center.x, center.y, Camera.main.transform.position.z) + cameraOffset;

        Camera.main.orthographicSize = cameraSize;
    
    }
}
