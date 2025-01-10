using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private float _scaleFactor = 1.25f; // Во сколько раз увеличивается объект при подборе
    [SerializeField] private float _snapThreshold = 0.5f; // Максимальное расстояние для примагничивания
    [SerializeField] private LayerMask _snapLayers;       // Слои для поиска точек примагничивания
    
    private Rigidbody2D _rigidbody2D;
    private Vector3 _originalScale;  
    private bool _isDragging;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _originalScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseDrag()
    {
        DragObject();
    }

    private void OnMouseUp()
    {
        StopDragging();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isDragging)
            return;
        
        if (other.tag == "Floor")
        {
            Reset();
        }
    }

    private void StartDragging()
    {
        _isDragging = true;
        MobileSceneScroll.Instance.SetItemDragging(_isDragging);
        transform.localScale = _originalScale * _scaleFactor;

        Reset();
    }

    private void Reset()
    {
        // Делаем обнуление
        _rigidbody2D.velocity = new Vector2(0, 0);
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    private void DragObject()
    {
        // Перемещаем объект за мышью
        var mouseWorldPosition = GetMouseWorldPosition();
        var newPosition = mouseWorldPosition;
        
        transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    private void StopDragging()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        SnapToClosestCollider();
        
        _isDragging = false;
        transform.localScale = _originalScale;
        MobileSceneScroll.Instance.SetItemDragging(_isDragging);
    }
    
    private void SnapToClosestCollider()
    {
        // Ищем ближайшие коллайдеры в радиусе snapThreshold
        var colliders = Physics2D.OverlapCircleAll(transform.position, _snapThreshold, _snapLayers);

        if (colliders.Length > 0)
        {
            Transform closestCollider = null;
            var closestPoint = Vector2.zero;
            var closestDistance = Mathf.Infinity;

            foreach (var collider in colliders)
            {
                // Определяем ближайшую точку на коллайдере
                Vector2 point = collider.ClosestPoint(transform.position);
                float distance = Vector2.Distance(transform.position, point);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider.transform;
                    closestPoint = point;
                }
            }

            // Если найдена ближайшая точка, примагничиваем объект
            if (closestCollider != null)
            {
                transform.position = new Vector3(closestPoint.x, closestPoint.y, transform.position.z);
                Reset();
            }
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        var mouseScreenPosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }
}
