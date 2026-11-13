using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Heli
{
    public class SceneRestart : MonoBehaviour
    {
        [SerializeField] private InputActionReference restart;

        private void OnEnable()
        {
            restart.action.Enable();
            restart.action.performed += OnRestart;
        }

        private void OnDisable()
        {
            restart.action.performed -= OnRestart;
            restart.action.Disable();
        }

        private void OnRestart(InputAction.CallbackContext _)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
