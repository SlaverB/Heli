using UnityEngine;

namespace Heli.View
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 5f, -16f);
        [SerializeField] private float lookHeight = 1.5f;
        [SerializeField] private float smoothTime = 0.25f;
        [SerializeField] private float turnSpeed = 6f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (!target) return;

            PoseToTarget();
            RotateToTarget();
        }

        private void PoseToTarget()
        {
            // follow heading only, otherwise the horizon rolls with the airframe
            Quaternion heading = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
            Vector3 wanted = target.position + heading * offset;
            transform.position = Vector3.SmoothDamp(transform.position, wanted, ref _velocity, smoothTime);
        }
        
        private void RotateToTarget()
        {
            Vector3 lookAt = target.position + Vector3.up * lookHeight;
            Quaternion look = Quaternion.LookRotation(lookAt - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 1f - Mathf.Exp(-turnSpeed * Time.deltaTime));
        }
    }
}
