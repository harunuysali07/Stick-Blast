using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Shape : MonoBehaviour
{
    [SerializeField] private List<Piece> pieces;
    [SerializeField] private Canvas canvas;
    
    private ShapeData _shapeData;

    [Button]
    public void ApplyShapeData(ShapeData shapeData)
    {
        if (shapeData == null)
        {
            Debug.LogError("ShapeData is null!");
            return;
        }
            
        _shapeData = shapeData;
        
        for (var i = 0; i < pieces.Count; i++)
        {
            pieces[i].ApplyShapeInfo(_shapeData.shapeInfos[i], GetNeighbors(i));
        }

        Resize();
    }

    private void Resize()
    {
        var nonEmptyShapeInfoCount = _shapeData.shapeInfos.Count(x => x.top || x.right || x.bottom || x.left);
        var activeImages = canvas.GetComponentsInChildren<Image>(false);

        if (activeImages.Length != 3 && nonEmptyShapeInfoCount <= 1)
            return;

        if (nonEmptyShapeInfoCount > 1)
            canvas.transform.localScale -= Vector3.one * (0.0007f * nonEmptyShapeInfoCount); 
        
        var positions = Vector3.zero;

        foreach (var image in activeImages)
        {
            positions += image.transform.position;
        }
        
        positions /= activeImages.Length;
        
        canvas.transform.position = positions;
        canvas.transform.localPosition *= -1f;
    }
    
    private List<ShapeInfo> GetNeighbors(int index)
    {
        var neighbors = new List<ShapeInfo>();
        
        if (index - 3 >= 0)
        {
            neighbors.Add(_shapeData.shapeInfos[index - 3]);
        }
        else
        {
            neighbors.Add(null);
        }
        
        if (index + 1 < pieces.Count && (index + 1) % 3 != 0)
        {
            neighbors.Add(_shapeData.shapeInfos[index + 1]);
        }
        else
        {
            neighbors.Add(null);
        }
        
        if (index + 3 < pieces.Count)
        {
            neighbors.Add(_shapeData.shapeInfos[index + 3]);
        }
        else
        {
            neighbors.Add(null);
        }
        
        if (index - 1 >= 0 && index % 3 != 0)
        {
            neighbors.Add(_shapeData.shapeInfos[index - 1]);
        }
        else
        {
            neighbors.Add(null);
        }

        return neighbors;
    }
}
