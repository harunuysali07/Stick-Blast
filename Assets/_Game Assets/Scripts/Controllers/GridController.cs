using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int GridSize => new(5, 5);

    [Header("Grid Settings")]
    [SerializeField] private float cellPadding = 0.1f;

    [SerializeField] private Cell cellPrefab;

    [Header("Shape Variables")]
    [SerializeField] private Shape shapePrefab;

    [SerializeField] private Transform shapeSpawnPoint;
    [SerializeField] private List<Transform> shapePositions;
    [SerializeField] private Vector3 fingerOffset;

    [Header("Debug Options")]
    [ShowInInspector, ReadOnly] private Cell[,] _grid;

    [ShowInInspector, ReadOnly] private List<Cell> _cells;

    private Shape _selectedShape;
    private List<Shape> _activeShapes = new List<Shape>();

    public void Initialize()
    {
        CreateGrid();
        SpawnShapes();

        StartCoroutine(CheckForMatchCoroutine());
    }

    [Button]
    private void CreateGrid()
    {
        _grid = new Cell[GridSize.x + 1, GridSize.y + 1];
        _cells = new List<Cell>();

        var xOffSet = cellPadding * (GridSize.x - 1) / 2f;
        var zOffSet = cellPadding * (GridSize.y - 1) / 2f;

        for (var x = 0; x < GridSize.x; x++)
        {
            for (var y = 0; y < GridSize.y; y++)
            {
                var cell = Instantiate(cellPrefab, transform);
                cell.name = $"Cell [{x},{y}]";
                cell.transform.localPosition = new Vector3(x * cellPadding - xOffSet, 0,
                    (GridSize.y - 1 - y) * cellPadding - zOffSet);

                _grid[x, y] = cell;
                _cells.Add(cell);
                cell.gridPosition = new Vector2Int(x, y);
            }
        }

        //Create Hidden Cells
        for (var y = 0; y < GridSize.y + 1; y++)
        {
            var x = GridSize.x;
            var cell = Instantiate(cellPrefab, transform);
            cell.name = $"Cell [{x},{y}]";
            cell.transform.localPosition = new Vector3(x * cellPadding - xOffSet, 0,
                (GridSize.y - 1 - y) * cellPadding - zOffSet);

            _grid[x, y] = cell;
            _cells.Add(cell);
            cell.gridPosition = new Vector2Int(x, y);
        }


        for (var x = 0; x < GridSize.x; x++)
        {
            var y = GridSize.y;
            var cell = Instantiate(cellPrefab, transform);
            cell.name = $"Cell [{x},{y}]";
            cell.transform.localPosition = new Vector3(x * cellPadding - xOffSet, 0,
                (GridSize.y - 1 - y) * cellPadding - zOffSet);

            _grid[x, y] = cell;
            _cells.Add(cell);
            cell.gridPosition = new Vector2Int(x, y);
        }

        _cells.ForEach(x => x.Initialize(GetNeighbors(x.gridPosition), this,
            x.gridPosition.x == GridSize.x || x.gridPosition.y == GridSize.y));
    }

    [ShowInInspector, ReadOnly] private readonly Queue<Cell> _matchQueue = new();

    private IEnumerator CheckForMatchCoroutine()
    {
        var checkInterval = new WaitForSeconds(.25f);

        while (enabled)
        {
            yield return checkInterval;

            var updateCells = false;

            while (_matchQueue.Count > 0)
            {
                updateCells = true;

                yield return new WaitForSeconds(.05f);

                var cell = _matchQueue.Dequeue();

                cell.ResetShapeInfo();
            }

            if (updateCells)
                _cells.ForEach(x => x.UpdatePieces(false, null));
        }
    }

    private void SpawnShapes()
    {
        if (_activeShapes.Count > 0)
        {
            Debug.LogError("There is already an active shape!");
            return;
        }

        foreach (var shapePosition in shapePositions)
        {
            var shapeData = DataManager.AllShapes.RandomItem();
            var shape = Instantiate(shapePrefab, shapeSpawnPoint.position, Quaternion.identity);
            shape.transform.SetParent(shapePosition);
            shape.ApplyShapeData(shapeData);

            shape.transform.DOLocalMove(Vector3.zero, 20f).SetSpeedBased(true).SetDelay(_activeShapes.Count * .25f)
                .SetEase(Ease.OutCubic);

            _activeShapes.Add(shape);
        }

        if (_activeShapes.All(x => _cells.All(c => !c.CheckForShapeData(x.ShapeData))))
        {
            this.Invoke(() => GameManager.Instance.LevelFinish(false), 1f);
        }
    }

    public void SelectShape(Shape shape)
    {
        if (_matchQueue.Count > 0)
            return;

        if (_selectedShape != null)
            _selectedShape.SetSelected(false);

        _selectedShape = shape;
        _selectedShape.SetSelected(true);
    }

    public void MoveShape(Vector3 position)
    {
        if (_selectedShape is null)
            return;

        _selectedShape.transform.position = position + fingerOffset;

        var closestCell = GetClosestCell(_selectedShape.transform.position);

        if (closestCell == null)
        {
            UpdateHighlightedCells(new List<Cell>());

            return;
        }

        closestCell.TryToHighlight(_selectedShape.ShapeData);
    }

    public void DeSelectShape()
    {
        if (_selectedShape == null)
            return;

        var closestCell = GetClosestCell(_selectedShape.transform.position);

        if (closestCell == null || !closestCell.CheckForShapeData(_selectedShape.ShapeData))
        {
            UpdateHighlightedCells(new List<Cell>());

            _selectedShape.SetSelected(false);
            _selectedShape.transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.OutCubic);

            _selectedShape = null;

            return;
        }

        closestCell.ApplyShapeData(_selectedShape.ShapeData);

        UpdateHighlightedCells(new List<Cell>());

        //TODO: Move to Pool
        Destroy(_selectedShape.gameObject);

        _activeShapes.Remove(_selectedShape);
        _selectedShape = null;

        if (_activeShapes.All(x => x == null) || _activeShapes.Count == 0)
        {
            SpawnShapes();
        }
        else if (_activeShapes.All(x => _cells.All(c => !c.CheckForShapeData(x.ShapeData))))
        {
            this.Invoke(() => GameManager.Instance.LevelFinish(false), 1f);
        }
        else
        {
            CheckForLineComplate();
        }
    }

    private Cell GetClosestCell(Vector3 position)
    {
        var closestCell = _cells[0];
        var closestDistance = Vector3.Distance(position, closestCell.transform.position);

        foreach (var cell in _cells)
        {
            var distance = Vector3.Distance(position, cell.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = cell;
            }
        }

        return closestDistance > cellPadding ? null : closestCell;
    }

    private List<Cell> GetNeighbors(Vector2Int gridPosition)
    {
        var neighbors = new List<Cell>();

        for (var y = -1; y <= 1; y++)
        {
            for (var x = -1; x <= 1; x++)
            {
                if (gridPosition.x + x >= 0 && gridPosition.x + x < GridSize.x + 1 && gridPosition.y + y >= 0 &&
                    gridPosition.y + y < GridSize.y + 1)
                {
                    neighbors.Add(_grid[gridPosition.x + x, gridPosition.y + y]);
                }
                else
                {
                    neighbors.Add(null);
                }
            }
        }

        return neighbors;
    }

    public void UpdateHighlightedCells(List<Cell> neighbors)
    {
        foreach (var cell in _cells.Except(neighbors))
        {
            cell.UpdatePieces(false, null);
        }
    }

    private void CheckForLineComplate()
    {
        //Vertical Check
        for (var x = 0; x < GridSize.x; x++)
        {
            var lineComplete = true;

            for (var y = 0; y < GridSize.y; y++)
            {
                if (!_grid[x, y].ShapeInfo.IsFull)
                {
                    lineComplete = false;
                    break;
                }
            }

            if (lineComplete)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    _matchQueue.Enqueue(_grid[x, y]);
                }
            }
        }

        //Horizontal Check
        for (var y = 0; y < GridSize.y; y++)
        {
            var lineComplete = true;

            for (var x = 0; x < GridSize.x; x++)
            {
                if (!_grid[x, y].ShapeInfo.IsFull)
                {
                    lineComplete = false;
                    break;
                }
            }

            if (lineComplete)
            {
                for (var x = 0; x < GridSize.x; x++)
                {
                    _matchQueue.Enqueue(_grid[x, y]);
                }
            }
        }
    }
}