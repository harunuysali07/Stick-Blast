using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Controllers")]
    public GridController gridController;
    
    [Header("Debug Options")]
    [ShowInInspector, ReadOnly] public LevelData levelData;
    [ShowInInspector, ReadOnly] public int RemainingGemCount { get; private set; }
    
    [Header("Shape Variables")]
    [SerializeField] private Shape shapePrefab;
    [SerializeField] private Transform shapeSpawnPoint;
    [SerializeField] private List<Transform> shapePositions;
    
    private List<Shape> _activeShapes = new List<Shape>();

    public LevelController Initialize(LevelData currentLevelData)
    {
        levelData = currentLevelData;
        
        RemainingGemCount = levelData.targetGemCount;

        UIManager.Instance.gamePlay.UpdateTargetCount(RemainingGemCount);

        return this;
    }

    private void Start()
    {
        gridController.Initialize();
        SpawnShape();
    }
    
    public void SpawnShape()
    {
        if (_activeShapes.Count > 0)
        {
            Debug.LogError("There is already an active shape!");
            return;
        }
        
        for (var i = 0; i < shapePositions.Count; i++)
        {
            var shapeData = DataManager.AllShapes.RandomItem();
            var shape = Instantiate(shapePrefab, shapeSpawnPoint.position, Quaternion.identity);
            shape.ApplyShapeData(shapeData);
        
            shape.transform.DOMove(shapePositions[_activeShapes.Count].position, 0.5f).SetEase(Ease.OutCubic);
        
            _activeShapes.Add(shape);   
        }
    }
}