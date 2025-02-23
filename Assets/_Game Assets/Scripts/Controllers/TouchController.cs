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
        
        var ray = _mainCamera.ScreenPointToRay(position);
        
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.TryGetComponent(out Shape shape))
            {
                _gridController.SelectShape(shape);
            }
        }
    }
    
    private void OnTouchMove(Vector3 position)
    {
        _gridController.MoveShape(position);
    }
    
    private void OnTouchEnd(Vector2 position)
    {
        _gridController.DeSelectShape();
    } 
}