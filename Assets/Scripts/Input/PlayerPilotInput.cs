using UnityEngine;
using UnityEngine.InputSystem;

namespace Heli.Controls
{
    public class PlayerPilotInput : MonoBehaviour, IPilotInputSource
    {
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference altitude;
        [SerializeField] private InputActionReference yaw;

        public PilotInput Current { get; private set; }

        private void OnEnable()
        {
            ActivateActions();
        }

        private void OnDisable()
        {
            DeactivateActions();
        }

        private void Update()
        {
            PollingInput();
        }

        private void PollingInput()
        {
            Current = new PilotInput
            {
                    Cyclic = move.action.ReadValue<Vector2>(),
                    Collective = altitude.action.ReadValue<float>(),
                    Pedal = yaw.action.ReadValue<float>()
            };
        }

        private void ActivateActions()
        {
            move.action.Enable();
            altitude.action.Enable();
            yaw.action.Enable();
        }

        private void DeactivateActions()
        {
            move.action.Disable();
            altitude.action.Disable();
            yaw.action.Disable();
            Current = default;
        }
    }
}
