using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRIDGen : MonoBehaviour
{
    public enum GridType { Hexagonal, Square, Triangular, RandomShape }

    [Header("Elements")]
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject Hexagon;

    [Header("Settings")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private int gridsize = 5;
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private GridType gridType;

    [Header("Offset Controls")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private float xOffset = 1.0f; // X spacing
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private float zOffset = 1.0f; // Z spacing

    [Header("Randomization Settings")]
    [SerializeField] private bool useRandomShapes = true;
    [SerializeField] private float randomFillChance = 0.7f; // % of cells filled in random shape

    private void GenerateGrid()
    {
        transform.Clear();

        switch (gridType)
        {
            case GridType.Hexagonal:
                GenerateHexagonalGrid();
                break;
            case GridType.Triangular:
                GenerateTriangularGrid();
                break;
        }
    }

   private void GenerateHexagonalGrid()
{
    transform.Clear();
        for (int x = -gridsize; x <= gridsize; x++) 
        {
            for (int y = -gridsize; y <= gridsize; y++)
            { 
                Vector3 spawnpos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                if (spawnpos.magnitude > grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * gridsize) 
                {
                    continue;
                }
                Instantiate(Hexagon, spawnpos, Quaternion.identity,transform);
            }
        }
}


   

    private void GenerateTriangularGrid()
    {
        float triangleSizeX = grid.cellSize.x * xOffset;
        float triangleSizeZ = (Mathf.Sqrt(3) / 2 * grid.cellSize.x) * zOffset;
        float groundY = transform.position.y;

        for (int y = 0; y < gridsize; y++)
        {
            for (int x = 0; x <= y; x++)
            {
                float offsetX = x * triangleSizeX - (y * triangleSizeX * 0.5f);
                float offsetZ = y * triangleSizeZ;
                Vector3 spawnPos = new Vector3(offsetX, groundY, offsetZ);

                Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
            }
        }
    }

    private void GenerateRandomShape()
    {
        float hexWidth = grid.cellSize.x * xOffset;
        float hexHeight = Mathf.Sqrt(3) / 2 * hexWidth * zOffset;
        float groundY = transform.position.y;

        for (int x = -gridsize; x <= gridsize; x++)
        {
            for (int y = -gridsize; y <= gridsize; y++)
            {
                if (Random.value > randomFillChance) // Randomly decide if a hexagon should be placed
                    continue;

                float offsetX = x * hexWidth * 0.75f;
                float offsetZ = y * hexHeight + (x % 2 == 0 ? 0 : hexHeight / 2);
                Vector3 spawnPos = new Vector3(offsetX, groundY, offsetZ);

                Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
            }
        }
    }
}
