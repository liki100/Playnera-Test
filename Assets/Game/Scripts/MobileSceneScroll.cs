using System;
using UnityEngine;

public class MobileSceneScroll : MonoBehaviour
{
    public static MobileSceneScroll Instance;
    
    [SerializeField] private float _moveSpeed = 0.01f;
    [SerializeField] private float _minX = -20f;
    [SerializeField] private float _maxX = 20f;
    [SerializeField] private float _minY = 0f;
    [SerializeField] private float _maxY = 0f;

    private Vector2 _touchStartPos;
    private bool _isDragging;
    private bool _isItemDragging;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_isItemDragging) 
            return;

        if (Input.touchCount <= 0) 
            return;
        
        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _touchStartPos = touch.position;
            _isDragging = true;
        }
        else if (touch.phase == TouchPhase.Moved && _isDragging)
        {
            var touchDelta = touch.position - _touchStartPos;
            var newPosition = transform.position - new Vector3(touchDelta.x, touchDelta.y, 0) * _moveSpeed;
                
            newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, _minY, _maxY);

            transform.position = newPosition;
            _touchStartPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isDragging = false;
        }
    }

    public void SetItemDragging(bool value)
    {
        _isItemDragging = value;
    }
}