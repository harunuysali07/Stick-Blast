using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Shape Data_0", menuName = "Scriptable Object / Shape Data", order = 101)]
public class ShapeData : ScriptableObjectWithID
{
    public List<ShapeInfo> shapeInfos;

    [SerializeField] private bool isUnlockedByDefault;
    [ShowInInspector] public bool IsUnlocked
    {
        get => PlayerPrefs.GetInt(ObjectID + "_Unlocked", isUnlockedByDefault ? 1 : 0) == 1;
        set => PlayerPrefs.SetInt(ObjectID + "_Unlocked", value ? 1 : 0);
    }
}

[Serializable]
public class ShapeInfo
{
    public bool top;
    public bool right;
    public bool bottom;
    public bool left;

    public bool Compare(ShapeInfo other)
    {
        return !((top && other.top) || (right && other.right) || (bottom && other.bottom) || (left && other.left));
    }

    public bool IsFull => top && right && bottom && left;
    public bool IsEmpty => !top && !right && !bottom && !left;
}
