using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static GRIDGen;

public class GridGenerator : MonoBehaviour
{
    public enum GridType { Hexagonal, Triangular, XPattern, StaggeredColumn,HoneyComb, DiamondHoneyComb,Shape,Rectangle,S,Heart }
    [Header("Elements")]
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject Hexagon;
    [Header("Settings")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private int gridsize;
    [SerializeField] private GridType gridType;
    [Header("Offset Controls")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private float xOffset = 1.0f; // X spacing
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private float zOffset = 1.0f; // Z spacing
    private void GenerateGrid()
    {
        transform.Clear();

        switch (gridType)
        {
            case GridType.Hexagonal:
                GenerateHexaGrid();
                break;
            case GridType.Triangular:
                GenerateTriangularGrid();
                break;
            case GridType.XPattern:
                GenerateXPattern();
                break;
            case GridType.StaggeredColumn:
                GenerateStaggeredColumn();
                break;
            case GridType.HoneyComb:
                GenerateHoneycombPattern();
                break;
            case GridType.DiamondHoneyComb:
                GenerateDiamondHoneycomb();
                break;
            case GridType.Shape:
                Shape();
                break;
            case GridType.Rectangle:
                GenerateRectangleHoneycomb();
                break;
            case GridType.S:
                GenerateSShapedHoneycomb();
                break;
            case GridType.Heart:
                GenerateHeartPattern();
                break;

        }
    }
    

    private void GenerateHexaGrid()
    {

        for (int x = -gridsize; x <= gridsize; x++)
        {
            for (int y = -gridsize; y <= gridsize; y++)
            {
                Vector3 spawnpos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                if (spawnpos.magnitude > grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * gridsize)
                {
                    continue;
                }
                Instantiate(Hexagon, spawnpos, Quaternion.identity, transform);
            }
        }
    }
    private void GenerateTriangularGrid()
    {
        float triangleSizeX = grid.cellSize.x;
        float triangleSizeZ = (Mathf.Sqrt(3) / 2 * grid.cellSize.x);
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
    private void GenerateXPattern()
    {
        float cellSizeX = xOffset;
        float cellSizeZ = zOffset;
        float groundY = transform.position.y;

        for (int x = -gridsize; x <= gridsize; x++)
        {
            for (int y = -gridsize; y <= gridsize; y++)
            {
                // X Pattern Condition: Points along diagonals
                if (Mathf.Abs(x) == Mathf.Abs(y))
                {
                    Vector3 spawnPos = new Vector3(x * cellSizeX, groundY, y * cellSizeZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
    private void GenerateStaggeredColumn()
    {
        float hexWidth = grid.cellSize.x * 1f;  // More compact horizontal spacing
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) / 2;  // More compact vertical spacing
        float groundY = transform.position.y;

        for (int y = 0; y < gridsize; y++)
        {
            float offsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;  // Offset alternate rows
            float offsetZ = y * hexHeight; // Adjust vertical placement

            Vector3 spawnPos = new Vector3(offsetX, groundY, offsetZ);
            Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
        }
    }
    private void GenerateHoneycombPattern()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Adjust for tighter horizontal spacing
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;
        float groundY = transform.position.y;

        // Define the honeycomb shape (approximately matching the image)
        bool[,] honeycombPattern = new bool[4, 4] {
        { false, true,  true,  false },
        { true,  true,  true,  true  },
        { true,  true,  true,  true  },
        { false, true,  true,  false }
    };

        // Center offset to position the pattern nicely
        float centerOffsetX = -1.5f * hexWidth;
        float centerOffsetZ = -1.5f * hexHeight;

        // Generate the honeycomb
        for (int y = 0; y < honeycombPattern.GetLength(0); y++)
        {
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;  // Offset every other row

            for (int x = 0; x < honeycombPattern.GetLength(1); x++)
            {
                if (honeycombPattern[y, x])
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }




        }
    }
    private void GenerateDiamondHoneycomb()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Horizontal spacing 
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;  // Vertical spacing
        float groundY = transform.position.y;

        // Define the diamond honeycomb pattern (matching the image)
        int[][] pattern = new int[][]
        {
        new int[] { 0, 0, 1, 0, 0 },
        new int[] { 0, 1, 1, 0, 0 },
        new int[] { 0, 1, 1, 1, 0 },
        new int[] { 0, 1, 1, 0, 0 },
        new int[] { 0, 0, 1, 0, 0 }
        };

        // Center offset
        float centerOffsetX = -2f * hexWidth;
        float centerOffsetZ = -2f * hexHeight;

        // Generate the diamond honeycomb
        for (int y = 0; y < pattern.Length; y++)
        {
            // Offset every other row horizontally
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;

            for (int x = 0; x < pattern[y].Length; x++)
            {
                if (pattern[y][x] == 1)
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
    private void Shape()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Horizontal spacing 
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;  // Vertical spacing
        float groundY = transform.position.y;

        // Define the diamond honeycomb pattern (matching the image)
        int[][] pattern = new int[][]
        {
        new int[] { 0, 0, 1, 0, 0 },
        new int[] { 0, 1, 1, 0, 0 },
        new int[] { 1, 1, 1, 1, 1 },
        new int[] { 1, 1, 1, 1, 0 },
        new int[] { 0, 0, 1, 0, 0 }
        };

        // Center offset
        float centerOffsetX = -2f * hexWidth;
        float centerOffsetZ = -2f * hexHeight;

        // Generate the diamond honeycomb
        for (int y = 0; y < pattern.Length; y++)
        {
            // Offset every other row horizontally
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;

            for (int x = 0; x < pattern[y].Length; x++)
            {
                if (pattern[y][x] == 1)
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
    private void GenerateRectangleHoneycomb()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Horizontal spacing 
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;  // Vertical spacing
        float groundY = transform.position.y;

        // Define the diamond honeycomb pattern (matching the image)
        int[][] pattern = new int[][]
        {
        new int[] { 0, 1, 1, 0, 0 },
        new int[] { 1, 1, 1, 0, 0 },
        new int[] { 0, 1, 1, 0, 0 },
        new int[] { 1, 1, 1, 0, 0 },
        new int[] { 0, 1, 1, 0, 0 }
        };

        // Center offset
        float centerOffsetX = -2f * hexWidth;
        float centerOffsetZ = -2f * hexHeight;

        // Generate the diamond honeycomb
        for (int y = 0; y < pattern.Length; y++)
        {
            // Offset every other row horizontally
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;

            for (int x = 0; x < pattern[y].Length; x++)
            {
                if (pattern[y][x] == 1)
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
    private void GenerateSShapedHoneycomb()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Horizontal spacing
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;  // Vertical spacing
        float groundY = transform.position.y;

        // Define the S-shaped pattern
        int[][] pattern = new int[][]
        {
        new int[] { 0, 1, 1, 0 },
        new int[] { 0, 1, 0, 0 },
        new int[] { 0, 1, 1, 0 },
        new int[] { 0, 1, 0, 0 },
        new int[] { 0, 1, 1, 0 },
        new int[] { 0, 1, 0, 0 }
        };

        // Center offset
        float centerOffsetX = -1.5f * hexWidth;
        float centerOffsetZ = -2f * hexHeight;

        // Generate the S-shaped honeycomb
        for (int y = 0; y < pattern.Length; y++)
        {
            // Offset every other row horizontally
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;

            for (int x = 0; x < pattern[y].Length; x++)
            {
                if (pattern[y][x] == 1)
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
    private void GenerateHeartPattern()
    {
        float hexWidth = grid.cellSize.x * 1f;  // Horizontal spacing
        float hexHeight = grid.cellSize.x * Mathf.Sqrt(3) * 0.5f;  // Vertical spacing
        float groundY = transform.position.y;

        // Define the heart pattern (1 = place hexagon, 0 = empty space)
        int[][] heartPattern = new int[][]
        {
        new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 1, 1, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
        new int[] { 0, 0, 1, 1, 1, 1, 0, 0, 0 },
        new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0 },
        new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 0 },
        new int[] { 0, 0, 1, 1, 0, 1, 1, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        // Center offset
        float centerOffsetX = -4 * hexWidth;
        float centerOffsetZ = -3.5f * hexHeight;

        // Generate only the heart-shaped hexagons
        for (int y = 0; y < heartPattern.Length; y++)
        {
            // Offset every other row horizontally
            float rowOffsetX = (y % 2 == 0) ? 0 : hexWidth * 0.5f;

            for (int x = 0; x < heartPattern[y].Length; x++)
            {
                if (heartPattern[y][x] == 1)
                {
                    float posX = centerOffsetX + rowOffsetX + (x * hexWidth);
                    float posZ = centerOffsetZ + (y * hexHeight);

                    Vector3 spawnPos = new Vector3(posX, groundY, posZ);
                    Instantiate(Hexagon, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
}

