using Heli.Flight;
using UnityEngine;

namespace Heli.View
{
    public class RotorSpin : MonoBehaviour
    {
        private const float RpmToDegPerSec = 360f / 60f;
        
        [SerializeField] private Helicopter helicopter;
        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private float maxRpm = 400f;

        private void Update()
        {
            RotateSpin();
        }

        private void RotateSpin()
        {
            if (helicopter.RpmNormalized <= 0f)
            {
                return;
            }
            
            float degPerSec = helicopter.RpmNormalized * maxRpm * RpmToDegPerSec;
            transform.Rotate(axis, degPerSec * Time.deltaTime, Space.Self);
        }
    }
}
