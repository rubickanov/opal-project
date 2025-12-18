using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rubickanov.Opal.Domain;
using Random = UnityEngine.Random;

namespace Rubickanov.Opal.Presentation
{
    public class GameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AdaptiveGrid _grid;
        [SerializeField] private CardView _cardPrefab;

        [Header("Game Settings")]
        [SerializeField] private List<Sprite> _cardSprites;
        [SerializeField, Min(4)] private int _cardCount = 16;
        [SerializeField, Min(0)] private float _previewDuration = 2f;
        [SerializeField] private bool _autoSave = true;

        private Game _game;
        private readonly List<CardView> _activeCards = new();
        private readonly List<CardView> _pool = new();
        private readonly Dictionary<Card, CardView> _cardViewMap = new();
        private Dictionary<int, Color> _valueColors = new();
        private bool _isPreviewActive;

        public event Action<int> OnCardAmountUpdated;
        public event Action<int> OnScoreUpdated;
        public event Action<int> OnTurnsUpdated;
        public event Action OnGameFinished;

        private void Start()
        {
            if (_autoSave && GameSaveManager.HasSave())
            {
                LoadGame();
            }
            else
            {
                StartNewGame();
            }
        }

        private void OnDestroy()
        {
            foreach (var cardView in _activeCards)
            {
                cardView.OnClicked -= HandleCardClicked;
            }

            foreach (var cardView in _pool)
            {
                if (cardView != null)
                {
                    Destroy(cardView.gameObject);
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _autoSave && _game is { IsFinished: false })
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            if (_autoSave && _game is { IsFinished: false })
            {
                SaveGame();
            }
        }

        public void StartNewGame()
        {
            StopAllCoroutines();
            GameSaveManager.DeleteSave();
            ClearCards();

            var (rows, columns) = CalculateGridDimensions(_cardCount);
            _game = new Game(rows, columns);
            _grid.Setup(_game.Rows, _game.Columns);

            GenerateColors();
            CreateCardViews();
            UpdateStats();

            OnCardAmountUpdated?.Invoke(_game.Cards.Count);

            if (_previewDuration > 0)
            {
                StartCoroutine(PreviewCardsRoutine());
            }
        }

        private IEnumerator PreviewCardsRoutine()
        {
            _isPreviewActive = true;

            foreach (var cardView in _activeCards)
            {
                cardView.ShowPreview();
            }

            yield return new WaitForSeconds(_previewDuration);

            foreach (var cardView in _activeCards)
            {
                cardView.UpdateVisual();
            }

            _isPreviewActive = false;
        }

        public void StartNewGame(int cardCount)
        {
            _cardCount = cardCount;
            StartNewGame();
        }

        private static (int rows, int columns) CalculateGridDimensions(int cardCount)
        {
            if (cardCount < 4) cardCount = 4;
            if (cardCount % 2 != 0) cardCount--;

            int sqrt = (int)Math.Sqrt(cardCount);

            for (int rows = sqrt; rows >= 2; rows--)
            {
                if (cardCount % rows == 0)
                {
                    int columns = cardCount / rows;
                    return (rows, columns);
                }
            }

            return (2, cardCount / 2);
        }

        private void LoadGame()
        {
            if (GameSaveManager.TryLoad(out var game, out var colors))
            {
                ClearCards();

                _game = game;
                _cardCount = _game.Rows * _game.Columns;
                _valueColors = colors;

                _grid.Setup(_game.Rows, _game.Columns);
                CreateCardViews();
                UpdateStats();
            }
            else
            {
                StartNewGame();
            }
        }

        private void SaveGame()
        {
            GameSaveManager.Save(_game, _valueColors);
        }

        private void GenerateColors()
        {
            _valueColors.Clear();
            int pairCount = _game.Cards.Count / 2;

            for (int i = 0; i < pairCount; i++)
            {
                float hue = (float)i / pairCount;
                float saturation = Random.Range(0.6f, 0.9f);
                float value = Random.Range(0.8f, 1f);

                _valueColors[i] = Color.HSVToRGB(hue, saturation, value);
            }
        }

        private void CreateCardViews()
        {
            foreach (var card in _game.Cards)
            {
                var cardView = GetOrCreateCardView();
                var sprite = GetSpriteForCard(card);
                var color = GetColorForCard(card);

                cardView.Init(card, sprite, color);
                cardView.OnClicked += HandleCardClicked;

                _activeCards.Add(cardView);
                _cardViewMap[card] = cardView;
            }
        }

        private CardView GetOrCreateCardView()
        {
            if (_pool.Count > 0)
            {
                var cardView = _pool[^1];
                _pool.RemoveAt(_pool.Count - 1);
                return cardView;
            }

            return Instantiate(_cardPrefab, _grid.transform);
        }

        private Sprite GetSpriteForCard(Card card)
        {
            if (_cardSprites == null || _cardSprites.Count == 0)
            {
                return null;
            }

            return _cardSprites[card.Value % _cardSprites.Count];
        }

        private Color GetColorForCard(Card card)
        {
            return _valueColors.TryGetValue(card.Value, out var color) ? color : Color.white;
        }

        private void HandleCardClicked(CardView cardView)
        {
            if (_isPreviewActive)
            {
                return;
            }

            var revealData = _game.RevealCard(cardView.Card);

            UpdateChangedCardViews(revealData.ChangedCards);
            UpdateStats();

            if (revealData.Result == RevealResult.MatchAndFinish)
            {
                GameSaveManager.DeleteSave();
                OnGameFinished?.Invoke();
            }
            else if (_autoSave)
            {
                SaveGame();
            }
        }

        private void UpdateChangedCardViews(List<Card> changedCards)
        {
            foreach (var card in changedCards)
            {
                if (_cardViewMap.TryGetValue(card, out var cardView))
                {
                    cardView.UpdateVisual();
                }
            }
        }

        private void ClearCards()
        {
            foreach (var cardView in _activeCards)
            {
                cardView.OnClicked -= HandleCardClicked;
                cardView.Reset();
                _pool.Add(cardView);
            }

            _activeCards.Clear();
            _cardViewMap.Clear();
        }

        private void UpdateStats()
        {
            OnScoreUpdated?.Invoke(_game.Score);
            OnTurnsUpdated?.Invoke(_game.Moves);
        }
    }
}