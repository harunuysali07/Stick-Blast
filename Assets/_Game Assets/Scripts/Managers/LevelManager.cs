using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [ReadOnly] public LevelController currentLevel;

    [SerializeField, AssetsOnly] private List<LevelController> levels;

    public LevelManager Initialize()
    {
        if (FindObjectOfType<LevelController>())
        {
            currentLevel = FindObjectOfType<LevelController>()?.Initialize(DataManager.AllLevels[DataManager.CurrentLevelIndex % DataManager.AllLevels.Count]);
            
            Debug.LogWarning("Level Initialized From Scene !".LogColor(Color.yellow), this);
            return this;
        }

        if (levels is null || levels.Count <= 0)
        {
            Debug.LogError("Levels Missing !", this);
            return this;
        }

        var levelToInitialize = levels[DataManager.CurrentLevelIndex % levels.Count];

        currentLevel = Instantiate(levelToInitialize).Initialize(DataManager.AllLevels[DataManager.CurrentLevelIndex % DataManager.AllLevels.Count]);

        return this;
    }
    
    public static void ReloadScene()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}