using UnityEngine;
using System;

public class ClickAndDragDetector
{
    private Vector3 dragStart;
    private bool isDragging;
    private float dragThreshold = 5f;
    
    public bool IsDragging => isDragging;
    public Vector3 DragStartPosition => dragStart;

    public event Action<Vector3> OnClick;
    public event Action<Vector3, Vector3> OnDragComplete;
    public event Action<Vector3> OnDragStart;
    public event Action<Vector3> OnDragging;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDetection(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateDetection(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDetection(Input.mousePosition);
        }
    }

    private void StartDetection(Vector3 position)
    {
        dragStart = position;
        isDragging = false;
    }

    private void UpdateDetection(Vector3 currentPosition)
    {
        if (!isDragging)
        {
            if (Vector3.Distance(dragStart, currentPosition) > dragThreshold)
            {
                isDragging = true;
                OnDragStart?.Invoke(dragStart);
            }
        }
        
        if (isDragging)
        {
            OnDragging?.Invoke(currentPosition);
        }
    }

    private void EndDetection(Vector3 endPosition)
    {
        if (isDragging)
        {
            OnDragComplete?.Invoke(dragStart, endPosition);
        }
        else
        {
            OnClick?.Invoke(endPosition);
        }
        
        isDragging = false;
    }
} 