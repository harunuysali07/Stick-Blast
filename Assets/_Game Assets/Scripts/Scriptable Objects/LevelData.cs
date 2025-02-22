using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Level Data_0", menuName = "Scriptable Object / Level Data", order = 101)]
public class LevelData : ScriptableObjectWithID
{
    public int targetGemCount;
}