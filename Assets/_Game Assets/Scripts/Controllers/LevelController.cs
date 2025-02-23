using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Color32 ThemeColor => levelData.themeColor;
    
    [Header("Controllers")]
    public GridController gridController;
    
    [Header("Debug Options")]
    [ShowInInspector, ReadOnly] public LevelData levelData;
    [ShowInInspector, ReadOnly] public int RemainingGemCount { get; private set; }
    
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
    }
}