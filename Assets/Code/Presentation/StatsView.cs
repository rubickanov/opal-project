using TMPro;
using UnityEngine;

namespace Rubickanov.Opal.Presentation
{
    public class StatsView : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;

        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _turns;

        private void OnEnable()
        {
            _gameController.OnScoreUpdated += UpdateScore;
            _gameController.OnTurnsUpdated += UpdateTurns;
        }

        private void OnDisable()
        {
            _gameController.OnScoreUpdated -= UpdateScore;
            _gameController.OnTurnsUpdated -= UpdateTurns;
        }

        private void UpdateScore(int score)
        {
            _score.text = score.ToString();
        }

        private void UpdateTurns(int turns)
        {
            _turns.text = turns.ToString();
        }
    }
}