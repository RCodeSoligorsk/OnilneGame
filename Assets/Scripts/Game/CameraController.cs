using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offsets = new Vector3(0.0f, 5.0f, -4.0f);
    [SerializeField] private float _smooth = 0.5f;
    [SerializeField] private float _forwardOffsetMultiplier = 2.0f;

    private Transform _target;
    private Vector3 _velocity;
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_target == null)
            return;

        Vector3 newPosition = Vector3.SmoothDamp(transform.position, _target.position + _offsets, ref _velocity, _smooth);
        transform.position = newPosition;
    }
}
