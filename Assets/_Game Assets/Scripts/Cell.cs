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
    
    [Header("Shape Images")]
    [SerializeField] private List<Image> cornerImages; // 0: TopLeft, 1: TopRight, 2: BottomRight, 3: BottomLeft
    [SerializeField] private List<Image> sideImages; // 0: Top, 1: Right, 2: Bottom, 3: Left
    [SerializeField] private List<Image> pieceImages; // 0:Top, 1:Right, 2:Bottom, 3:Left
    [SerializeField] private Image centerImage;
    
    [SerializeField] private Color32 emptyColor;
    [SerializeField] private Color32 ghostColor;

    public void Awake()
    {
        cornerImages.ForEach(x => x.color = emptyColor);
        sideImages.ForEach(x => x.color = emptyColor);
    }
}