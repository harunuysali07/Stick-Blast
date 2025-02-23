using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [ReadOnly] public Vector2Int gridPosition;
    public ShapeInfo ShapeInfo => _shapeInfo;

    [Header("Shape Images")]
    [SerializeField] private List<Image> cornerImages; // 0: TopLeft, 1: TopRight, 2: BottomRight, 3: BottomLeft

    [SerializeField] private List<Image> sideImages; // 0: Top, 1: Right, 2: Bottom, 3: Left
    [SerializeField] private List<Image> pieceImages; // 0:Top, 1:Right, 2:Bottom, 3:Left
    [SerializeField] private Image centerImage;

    [SerializeField] private Color32 emptyColor;
    [SerializeField] private Color32 ghostColor;

    [SerializeField] private Material filledMaterial;

    [ShowInInspector, ReadOnly] private List<Cell> _neighbors = new();
    [ShowInInspector, ReadOnly] private ShapeInfo _shapeInfo;

    private GridController _gridController;

    private Color32 _themeColor;
    private bool _isHidden;

    public void Initialize(List<Cell> neighbors, GridController gridController, bool isHidden)
    {
        _gridController = gridController;
        _isHidden = isHidden;

        _neighbors = neighbors;
        _shapeInfo = new ShapeInfo();

        _themeColor = GameManager.Instance.levelManager.currentLevel.ThemeColor;

        cornerImages.ForEach(x => x.color = emptyColor);
        sideImages.ForEach(x => x.color = emptyColor);
        pieceImages.ForEach(x => x.color = Color.clear);

        if (isHidden)
        {
            if (gridPosition.x == _gridController.GridSize.x)
            {
                _shapeInfo.top = true;
                _shapeInfo.right = true;
                _shapeInfo.bottom = true;

                cornerImages[1].gameObject.SetActive(false);
                cornerImages[2].gameObject.SetActive(false);

                sideImages[0].gameObject.SetActive(false);
                sideImages[1].gameObject.SetActive(false);
                sideImages[2].gameObject.SetActive(false);

                pieceImages[0].gameObject.SetActive(false);
                pieceImages[1].gameObject.SetActive(false);
                pieceImages[2].gameObject.SetActive(false);

                centerImage.gameObject.SetActive(false);
            }

            if (gridPosition.y == _gridController.GridSize.y)
            {
                _shapeInfo.right = true;
                _shapeInfo.bottom = true;
                _shapeInfo.left = true;

                cornerImages[2].gameObject.SetActive(false);
                cornerImages[3].gameObject.SetActive(false);

                sideImages[1].gameObject.SetActive(false);
                sideImages[2].gameObject.SetActive(false);
                sideImages[3].gameObject.SetActive(false);

                pieceImages[1].gameObject.SetActive(false);
                pieceImages[2].gameObject.SetActive(false);
                pieceImages[3].gameObject.SetActive(false);

                centerImage.gameObject.SetActive(false);
            }
        }

        centerImage.color = Color.clear;
    }

    public void TryToHighlight(ShapeData selectedShapeShapeData)
    {
        var isAvailable = CheckForShapeData(selectedShapeShapeData);

        _gridController.UpdateHighlightedCells(_neighbors);

        if (isAvailable)
        {
            for (var i = 0; i < 9; i++)
            {
                _neighbors[i]?.UpdatePieces(true, selectedShapeShapeData.shapeInfos[i]);
            }
        }
        else
        {
            for (var i = 0; i < 9; i++)
            {
                _neighbors[i]?.UpdatePieces(false, null);
            }
        }
    }

    public bool CheckForShapeData(ShapeData selectedShapeShapeData)
    {
        var isAvailable = true;

        for (var i = 0; i < 9; i++)
        {
            if (_neighbors[i] == null)
            {
                if (!selectedShapeShapeData.shapeInfos[i].IsEmpty)
                {
                    isAvailable = false;
                    break;
                }

                continue;
            }

            if (_neighbors[i]._shapeInfo.Compare(selectedShapeShapeData.shapeInfos[i]))
                continue;

            isAvailable = false;
            break;
        }

        return isAvailable;
    }

    public void UpdatePieces(bool isHighlighted, ShapeInfo shapeInfo)
    {
        if (!isHighlighted)
        {
            cornerImages[0].color = (_shapeInfo.top || _shapeInfo.left) && !_isHidden ? _themeColor : emptyColor;
            cornerImages[1].color = (_shapeInfo.top || _shapeInfo.right) && !_isHidden ? _themeColor : emptyColor;
            cornerImages[2].color = (_shapeInfo.bottom || _shapeInfo.right) && !_isHidden ? _themeColor : emptyColor;
            cornerImages[3].color = (_shapeInfo.bottom || _shapeInfo.left) && !_isHidden ? _themeColor : emptyColor;

            pieceImages[0].color = _shapeInfo.top ? _themeColor : Color.clear;
            pieceImages[1].color = _shapeInfo.right ? _themeColor : Color.clear;
            pieceImages[2].color = _shapeInfo.bottom ? _themeColor : Color.clear;
            pieceImages[3].color = _shapeInfo.left ? _themeColor : Color.clear;

            pieceImages[0].material = null;
            pieceImages[1].material = null;
            pieceImages[2].material = null;
            pieceImages[3].material = null;
        }
        else if (shapeInfo != null && !_shapeInfo.IsFull)
        {
            cornerImages[0].color =
                ((_shapeInfo.top || _shapeInfo.left) && !_isHidden) || shapeInfo.top || shapeInfo.left
                    ? _themeColor
                    : emptyColor;
            cornerImages[1].color =
                ((_shapeInfo.top || _shapeInfo.right) && !_isHidden) || shapeInfo.top || shapeInfo.right
                    ? _themeColor
                    : emptyColor;
            cornerImages[2].color = ((_shapeInfo.bottom || _shapeInfo.right) && !_isHidden) || shapeInfo.bottom ||
                                    shapeInfo.right
                ? _themeColor
                : emptyColor;
            cornerImages[3].color = ((_shapeInfo.bottom || _shapeInfo.left) && !_isHidden) || shapeInfo.bottom ||
                                    shapeInfo.left
                ? _themeColor
                : emptyColor;

            pieceImages[0].color = _shapeInfo.top ? _themeColor : (shapeInfo.top ? ghostColor : Color.clear);
            pieceImages[1].color = _shapeInfo.right ? _themeColor : (shapeInfo.right ? ghostColor : Color.clear);
            pieceImages[2].color = _shapeInfo.bottom ? _themeColor : (shapeInfo.bottom ? ghostColor : Color.clear);
            pieceImages[3].color = _shapeInfo.left ? _themeColor : (shapeInfo.left ? ghostColor : Color.clear);

            pieceImages[0].material = !_shapeInfo.top && shapeInfo.top ? filledMaterial : null;
            pieceImages[1].material = !_shapeInfo.right && shapeInfo.right ? filledMaterial : null;
            pieceImages[2].material = !_shapeInfo.bottom && shapeInfo.bottom ? filledMaterial : null;
            pieceImages[3].material = !_shapeInfo.left && shapeInfo.left ? filledMaterial : null;
        }

        cornerImages[0].material = cornerImages[0].color == _themeColor ? filledMaterial : null;
        cornerImages[1].material = cornerImages[1].color == _themeColor ? filledMaterial : null;
        cornerImages[2].material = cornerImages[2].color == _themeColor ? filledMaterial : null;
        cornerImages[3].material = cornerImages[3].color == _themeColor ? filledMaterial : null;

        centerImage.color = _shapeInfo.IsFull ? _themeColor : Color.clear;
    }

    public void ApplyShapeData(ShapeData shapeData)
    {
        var isAvailable = CheckForShapeData(shapeData);

        if (!isAvailable)
            return;

        for (var i = 0; i < 9; i++)
            _neighbors[i]?.ApplyShapeInfo(shapeData.shapeInfos[i]);
    }

    private void ApplyShapeInfo(ShapeInfo shapeInfo)
    {
        _shapeInfo.top = _shapeInfo.top || shapeInfo.top;
        _shapeInfo.right = _shapeInfo.right || shapeInfo.right;
        _shapeInfo.bottom = _shapeInfo.bottom || shapeInfo.bottom;
        _shapeInfo.left = _shapeInfo.left || shapeInfo.left;

        if (_shapeInfo.top && _neighbors[1] != null)
        {
            _neighbors[1]._shapeInfo.bottom = true;
        }
        
        if (_shapeInfo.left && _neighbors[3] != null)
        {
            _neighbors[3]._shapeInfo.right = true;
        }

        if (_shapeInfo.right && _neighbors[5] != null)
        {
            _neighbors[5]._shapeInfo.left = true;
        }

        if (_shapeInfo.bottom && _neighbors[7] != null)
        {
            _neighbors[7]._shapeInfo.top = true;
        }
    }

    public void ResetShapeInfo()
    {
        if (_neighbors[1] != null)
        {
            _neighbors[1]._shapeInfo.bottom = false;
        }
        
        if (_neighbors[3] != null)
        {
            _neighbors[3]._shapeInfo.right = false;
        }

        if (_neighbors[5] != null)
        {
            _neighbors[5]._shapeInfo.left = false;
        }

        if (_neighbors[7] != null)
        {
            _neighbors[7]._shapeInfo.top = false;
        }
        
        _shapeInfo = new ShapeInfo();
        
        UpdatePieces(false, null);
    }
}