using UnityEngine;
using UnityEngine.InputSystem;

namespace Heli.Controls
{
    public class PlayerPilotInput : MonoBehaviour, IPilotInputSource
    {
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference altitude;
        [SerializeField] private InputActionReference yaw;

        public PlayerPilotInput(InputActionReference altitude)
        {
            this.altitude = altitude;
        }

        public PilotInput Current { get; private set; }

        private void OnEnable()
        {
            move.action.Enable();
            altitude.action.Enable();
            yaw.action.Enable();
        }

        private void OnDisable()
        {
            move.action.Disable();
            altitude.action.Disable();
            yaw.action.Disable();
            Current = default;
        }
    
        private void Update()
        {
            Current = new PilotInput
            {
                Cyclic = move.action.ReadValue<Vector2>(),
                Collective = altitude.action.ReadValue<float>(),
                Pedal = yaw.action.ReadValue<float>()
            };
        }
    }
}
