#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rubickanov.Opal.Presentation
{
    public class ControlButtonsView : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;

        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private TMP_InputField _cardsAmountInputField;

        private void OnEnable()
        {
            _gameController.OnCardAmountUpdated += UpdateCardAmountView;
            _restartButton.onClick.AddListener(HandleRestart);
            _quitButton.onClick.AddListener(HandleQuit);
        }

        private void OnDisable()
        {
            _gameController.OnCardAmountUpdated -= UpdateCardAmountView;
            _restartButton.onClick.RemoveListener(HandleRestart);
            _quitButton.onClick.RemoveListener(HandleQuit);
        }
        
        private void UpdateCardAmountView(int amount)
        {
            _cardsAmountInputField.text = amount.ToString();
        }

        private void HandleRestart()
        {
            if (int.TryParse(_cardsAmountInputField.text, out int amount))
            {
                _gameController.StartNewGame(amount);
            }
            else
            {
                _gameController.StartNewGame();
            }
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