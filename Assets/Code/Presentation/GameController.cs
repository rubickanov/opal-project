using System;
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
        [SerializeField] private int _rows = 4;
        [SerializeField] private int _columns = 4;
        [SerializeField] private bool _autoSave = true;

        private Game _game;
        private readonly List<CardView> _cardViews = new();
        private readonly Dictionary<Card, CardView> _cardViewMap = new();
        private Dictionary<int, Color> _valueColors = new();

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
            ClearCards();
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
            GameSaveManager.DeleteSave();
            ClearCards();

            _game = new Game(_rows, _columns);
            _grid.Setup(_game.Rows, _game.Columns);

            GenerateColors();
            CreateCardViews();
            UpdateStats();
        }

        public void StartNewGame(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            StartNewGame();
        }

        private void LoadGame()
        {
            if (GameSaveManager.TryLoad(out var game, out var colors))
            {
                ClearCards();

                _game = game;
                _rows = _game.Rows;
                _columns = _game.Columns;
                _valueColors = colors;

                _grid.Setup(_rows, _columns);
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
                var cardView = Instantiate(_cardPrefab, _grid.transform);
                var sprite = GetSpriteForCard(card);
                var color = GetColorForCard(card);

                cardView.Init(card, sprite, color);
                cardView.OnClicked += HandleCardClicked;

                _cardViews.Add(cardView);
                _cardViewMap[card] = cardView;
            }
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
            foreach (var cardView in _cardViews)
            {
                cardView.OnClicked -= HandleCardClicked;
                Destroy(cardView.gameObject);
            }

            _cardViews.Clear();
            _cardViewMap.Clear();
        }

        private void UpdateStats()
        {
            OnScoreUpdated?.Invoke(_game.Score);
            OnTurnsUpdated?.Invoke(_game.Moves);
        }
    }
}
