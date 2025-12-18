#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Rubickanov.Opal.Presentation
{
    public class ControlButtonsView : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;

        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(_gameController.StartNewGame);
            _quitButton.onClick.AddListener(HandleQuit);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(_gameController.StartNewGame);
            _quitButton.onClick.RemoveListener(HandleQuit);
        }

        private void HandleQuit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}