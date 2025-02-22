using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Side Parts")]
    [SerializeField] private GameObject topSide;
    [SerializeField] private GameObject rightSide;
    [SerializeField] private GameObject bottomSide;
    [SerializeField] private GameObject leftSide;
    
    [Header("Corner Parts")]
    [SerializeField] private GameObject topLeftCorner;
    [SerializeField] private GameObject topRightCorner;
    [SerializeField] private GameObject bottomRightCorner;
    [SerializeField] private GameObject bottomLeftCorner;

    [Header("Cap Parts")]
    [SerializeField] private GameObject topLeftCap;
    [SerializeField] private GameObject topRightCap;
    [SerializeField] private GameObject rightTopCap;
    [SerializeField] private GameObject rightBottomCap;
    [SerializeField] private GameObject bottomRightCap;
    [SerializeField] private GameObject bottomLeftCap;
    [SerializeField] private GameObject leftBottomCap;
    [SerializeField] private GameObject leftTopCap;

    public void ApplyShapeInfo(ShapeInfo shapeInfo, List<ShapeInfo> neighbors)
    {
        if (!shapeInfo.top && !shapeInfo.right && !shapeInfo.bottom && !shapeInfo.left)
            gameObject.SetActive(false);
        
        var topNeighbor = neighbors[0];
        var rightNeighbor = neighbors[1];
        var bottomNeighbor = neighbors[2];
        var leftNeighbor = neighbors[3];
        
        topSide.SetActive(shapeInfo.top);
        rightSide.SetActive(shapeInfo.right);
        bottomSide.SetActive(shapeInfo.bottom);
        leftSide.SetActive(shapeInfo.left);
        
        topLeftCorner.SetActive(shapeInfo.top && shapeInfo.left);
        topRightCorner.SetActive(shapeInfo.top && shapeInfo.right);
        bottomRightCorner.SetActive(shapeInfo.bottom && shapeInfo.right);
        bottomLeftCorner.SetActive(shapeInfo.bottom && shapeInfo.left);

        if (topNeighbor != null)
        {
            leftTopCap.SetActive(shapeInfo.left && !shapeInfo.top && !topNeighbor.left);
            rightTopCap.SetActive(shapeInfo.right && !shapeInfo.top && !topNeighbor.right);
        }
        else
        {
            leftTopCap.SetActive(shapeInfo.left && !shapeInfo.top);
            rightTopCap.SetActive(shapeInfo.right && !shapeInfo.top);
        }
        
        if (rightNeighbor != null)
        {
            topRightCap.SetActive(shapeInfo.top && !shapeInfo.right && !rightNeighbor.top);
            bottomRightCap.SetActive(shapeInfo.bottom && !shapeInfo.right && !rightNeighbor.bottom);
        }
        else
        {
            topRightCap.SetActive(shapeInfo.top && !shapeInfo.right);
            bottomRightCap.SetActive(shapeInfo.bottom && !shapeInfo.right);
        }
        
        if (bottomNeighbor != null)
        {
            rightBottomCap.SetActive(shapeInfo.right && !shapeInfo.bottom && !bottomNeighbor.right);
            leftBottomCap.SetActive(shapeInfo.left && !shapeInfo.bottom && !bottomNeighbor.left);
        }
        else
        {
            rightBottomCap.SetActive(shapeInfo.right && !shapeInfo.bottom);
            leftBottomCap.SetActive(shapeInfo.left && !shapeInfo.bottom);
        }
        
        if (leftNeighbor != null)
        {
            topLeftCap.SetActive(shapeInfo.top && !shapeInfo.left && !leftNeighbor.top);
            bottomLeftCap.SetActive(shapeInfo.bottom && !shapeInfo.left && !leftNeighbor.bottom);
        }
        else
        {
            topLeftCap.SetActive(shapeInfo.top && !shapeInfo.left);
            bottomLeftCap.SetActive(shapeInfo.bottom && !shapeInfo.left);
        }
    }
}
