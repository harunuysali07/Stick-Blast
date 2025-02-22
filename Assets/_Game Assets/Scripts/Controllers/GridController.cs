using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class GridController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize = new(6, 6);

    [SerializeField] private float cellPadding = 0.1f;
    [SerializeField] private Cell cellPrefab;

    [Header("Debug Options")]
    [ShowInInspector, ReadOnly] private Cell[,] _grid;

    [ShowInInspector, ReadOnly] private List<Cell> _cells;

    [ShowInInspector, ReadOnly] private Vector2 _cubeMoveRange;

    public void Initialize()
    {
        CreateGrid();

        StartCoroutine(CheckForMatchCoroutine());
    }

    [Button]
    private void CreateGrid()
    {
        _grid = new Cell[gridSize.x, gridSize.y];
        _cells = new List<Cell>();

        var xOffSet = cellPadding * (gridSize.x - 1) / 2f;
        var zOffSet = cellPadding * (gridSize.y - 1) / 2f;

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var cell = Instantiate(cellPrefab, transform);
                cell.name = $"Cell [{x},{y}]";
                cell.transform.localPosition = new Vector3(x * cellPadding - xOffSet, 0,
                    (gridSize.y - 1 - y) * cellPadding - zOffSet);

                _grid[x, y] = cell;
                _cells.Add(cell);
                cell.gridPosition = new Vector2Int(x, y);
                
            }
        }

        _cubeMoveRange = new Vector2(_grid[0, 0].transform.position.x,
            _grid[gridSize.x - 1, gridSize.y - 1].transform.position.x);
    }
    
    [ShowInInspector, ReadOnly] private readonly Queue<Cell> _matchQueue = new();

    private IEnumerator CheckForMatchCoroutine()
    {
        var checkInterval = new WaitForSeconds(.25f);

        while (enabled)
        {
            yield return checkInterval;
            
        }
    }
}