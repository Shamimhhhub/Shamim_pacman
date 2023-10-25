using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private PrefabList prefabs;
    private int[,] levelMap =
    {
        { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 4 },
        { 2, 6, 4, 0, 0, 4, 5, 4, 0, 0, 0, 4, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 3, 5, 3, 4, 4, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 4, 4, 5, 3, 4, 4, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 4, 4, 5, 5, 5, 5, 4 },
        { 1, 2, 2, 2, 2, 1, 5, 4, 3, 4, 4, 3, 0, 4 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 3, 4, 4, 3, 0, 3 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 0 },
        { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 },
    };
    
    private readonly List<int> walls = new(){ 1, 2, 3, 4, 7};
    private readonly Vector3 positionOffset = new Vector3(-13.5f, 14f);
    private void Start()
    {
        GenerateFullMap();
        GenerateMapTiles();
    }

    private void GenerateFullMap()
    {
        var initRowCount = levelMap.GetLength(0);
        var initColCount = levelMap.GetLength(1);
        var result = new int[initRowCount * 2 - 1, initColCount * 2];
        for (var row = 0; row < result.GetLength(0); row++)
        {
            var rowToReadFrom = row < initRowCount ? row : initRowCount * 2 - 2 - row;
            for (var col = 0; col < result.GetLength(1); col++)
            {
                var colToReadFrom = col < initColCount ? col : initColCount * 2 - 1 - col;
                result[row, col] = levelMap[rowToReadFrom, colToReadFrom];
            }
        }
        levelMap = result;
    }
    
    private void GenerateMapTiles()
    {
        
        var parent = transform;
        for (var row = 0; row < levelMap.GetLength(0); row++)
        {
            for (var col = 0; col < levelMap.GetLength(1); col++)
            {
                var id = levelMap[row, col];
                var rotation = GetRotation(row, col);
                var tile = Instantiate(prefabs.GetPrefab(id),
                    GetPosition(col, row),
                    Quaternion.Euler(0, 0, rotation),
                    parent
                );
                if (id != 7) continue;

                if (IsPath(row + 1, col + 1) || IsPath(row - 1, col - 1))
                {
                    tile.GetComponent<SpriteRenderer>().flipY = true;
                }
            }
        }
    }

    private Vector3 GetPosition(int col, int row)
    {
        return new Vector3(col, -row) + positionOffset;
    }
    
    private float GetRotation(int row, int col)
    {
        if (!IsWall(row, col))
            return 0;
        var id = levelMap[row, col];
        switch (id)
        {
            case 1:
            case 3:
                if (!IsWall(row, col + 1))
                {
                    if (!IsWall(row - 1, col))
                        return 270;
                    if (!IsWall(row + 1, col))
                        return 180;
                }
                if (!IsWall(row, col - 1))
                {
                    if (!IsWall(row - 1, col))
                        return 0;
                    if (!IsWall(row + 1, col))
                        return 90;
                }
                if (IsPath(row - 1, col - 1)) return 180;
                if (IsPath(row + 1, col - 1)) return 270;
                if (IsPath(row + 1, col + 1)) return 0;
                if (IsPath(row - 1, col + 1)) return 90;
                return 0;
            case 2:
            case 4:
                if (IsWall(row, col - 1) && IsWall(row, col + 1))
                    return 0;
                if (IsWall(row, col - 1) && IsOut(row, col + 1))
                    return 0;
                if (IsOut(row, col - 1) && IsWall(row, col + 1))
                    return 0;
                if (!IsWall(row - 1, col) && !IsWall(row + 1, col))
                    return 0;
                return 90;
            case 7:
                if(IsOut(row - 1, col)) return 270;
                if (IsOut(row, col - 1)) return 180;
                if (IsOut(row, col + 1)) return 0;
                return 90;
        }
        return 0;
    }

    private bool IsPath(int row, int col)
    {
        return !IsOut(row, col) && !(walls.Contains(levelMap[row, col]));
    }
    private bool IsWall(int row, int col)
    {
        return !IsOut(row, col) && walls.Contains(levelMap[row, col]);
    }
    private bool IsOut(int row, int col)
    {
        return row < 0 || col < 0 || row >= levelMap.GetLength(0) || col >= levelMap.GetLength(1);
    }
}