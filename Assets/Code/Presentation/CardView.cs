using System;
using UnityEngine;
using UnityEngine.UI;
using Rubickanov.Opal.Domain;

namespace Rubickanov.Opal.Presentation
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _frontImage;
        [SerializeField] private Image _backImage;
        [SerializeField] private GameObject _frontSide;
        [SerializeField] private GameObject _backSide;

        public event Action<CardView> OnClicked;

        public Card Card { get; private set; }

        private Color _originalColor = Color.white;

        public void Init(Card card, Sprite frontSprite, Color color)
        {
            Card = card;
            _frontImage.sprite = frontSprite;
            _originalColor = color;
            _frontImage.color = color;
            gameObject.SetActive(true);
            UpdateVisual();
        }

        public void Reset()
        {
            Card = null;
            _frontImage.sprite = null;
            _frontImage.color = Color.white;
            _originalColor = Color.white;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnClicked?.Invoke(this);
        }

        public void ShowPreview()
        {
            ShowFront();
            SetInteractable(false);
        }

        public void UpdateVisual()
        {
            switch (Card.State)
            {
                case CardState.Hidden:
                    ShowBack();
                    SetInteractable(true);
                    break;

                case CardState.Revealed:
                    ShowFront();
                    SetInteractable(false);
                    break;

                case CardState.PendingHide:
                    ShowFront();
                    SetInteractable(true);
                    break;

                case CardState.Matched:
                    ShowFront();
                    SetInteractable(false);
                    SetMatched();
                    break;
            }
        }

        private void ShowFront()
        {
            _frontSide.SetActive(true);
            _backSide.SetActive(false);
        }

        private void ShowBack()
        {
            _frontSide.SetActive(false);
            _backSide.SetActive(true);
        }

        private void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void SetMatched()
        {
            var color = _originalColor;
            color.a = 0.6f;
            _frontImage.color = color;
        }
    }
}
