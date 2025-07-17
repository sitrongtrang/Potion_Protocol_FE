// using System;
// using UnityEngine;

// public class CameraController : MonoBehaviour
// {
//     [SerializeField] private Transform _target;
//     public Vector3 offset = new Vector3(0, 0, -10);
//     public float smoothSpeed = 5f;
    
//     void Awake()
//     {
//         EventBus.UpdateTarget += UpdateTarget;
//     }
//     void OnDestroy()
//     {
//         EventBus.UpdateTarget -= UpdateTarget;
//     }
//     void UpdateTarget(Transform target)
//     {
//         _target = target;
//     } 
//     void LateUpdate()
//     {
//         if (_target != null)
//         {
//             Vector3 desiredPosition = _target.position + offset;
//             Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
//             transform.position = smoothedPosition;
//         }
//     }
// }
