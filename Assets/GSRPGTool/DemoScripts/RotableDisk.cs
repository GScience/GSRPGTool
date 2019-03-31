using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class RotableDisk : MonoBehaviour
{
    public float startPos;

    private bool _isRotating;
    private Vector3 _lastPos = Vector3.zero;

    void Start()
    {
        transform.rotation = Quaternion.AngleAxis(startPos, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && GetComponent<Collider2D>().OverlapPoint(Input.mousePosition))
        {
            if (!_isRotating)
            {
                _isRotating = true;
                _lastPos = Input.mousePosition;
            }
            var distanceVector = Input.mousePosition - _lastPos;
            var diameterVector = Input.mousePosition - transform.position;

            var degree = Vector3.SignedAngle(distanceVector, diameterVector, Vector3.forward);

            if (degree > 0)
                transform.Rotate(Vector3.forward, -distanceVector.magnitude);
            else if (degree < 0)
                transform.Rotate(Vector3.forward, distanceVector.magnitude);
            _lastPos = Input.mousePosition;
        }
        else
            _isRotating = false;
    }
}
