using UnityEngine;

public class TouchController : MonoBehaviour
{
    private GridController _gridController;
    private Camera _mainCamera;
    
    private void Start()
    {
        _gridController = GameManager.Instance.levelManager.currentLevel.gridController;
        _mainCamera = Camera.main;
        
        GameManager.Instance.touchManager.OnTouchBegin += OnTouchBegin;
        GameManager.Instance.touchManager.OnTouchMoveWorld += OnTouchMove;
        GameManager.Instance.touchManager.OnTouchEnd += OnTouchEnd;
    }

    private void OnTouchBegin(Vector2 position)
    {
        GameManager.Instance.touchManager.TouchDistance = _mainCamera.transform.position.magnitude;
    }
    
    private void OnTouchMove(Vector3 position)
    {
        
    }
    
    private void OnTouchEnd(Vector2 position)
    {
        
    } 
}