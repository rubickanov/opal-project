using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Rubickanov.Opal.Domain;
using Rubickanov.Opal.Presentation.Animation;

namespace Rubickanov.Opal.Presentation
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _frontImage;
        [SerializeField] private Image _backImage;
        [SerializeField] private GameObject _frontSide;
        [SerializeField] private GameObject _backSide;

        [Header("Animation")]
        [SerializeField] private float _flipDuration = 0.15f;
        [SerializeField] private float _matchPunchScale = 0.2f;

        public event Action<CardView> OnClicked;

        public Card Card { get; private set; }

        private Color _originalColor = Color.white;
        private bool _isShowingFront;
        private Coroutine _currentAnimation;

        public void Init(Card card, Sprite frontSprite, Color color)
        {
            Card = card;
            _frontImage.sprite = frontSprite;
            _originalColor = color;
            _frontImage.color = color;
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            UpdateVisualInstant();
        }

        public void Reset()
        {
            StopCurrentAnimation();
            Card = null;
            _frontImage.sprite = null;
            _frontImage.color = Color.white;
            _originalColor = Color.white;
            _isShowingFront = false;
            transform.localScale = Vector3.one;
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
            ShowFrontInstant();
            SetInteractable(false);
        }

        public void UpdateVisual()
        {
            StopCurrentAnimation();

            switch (Card.State)
            {
                case CardState.Hidden:
                    _currentAnimation = StartCoroutine(FlipToBack());
                    break;

                case CardState.Revealed:
                    _currentAnimation = StartCoroutine(FlipToFront(false));
                    break;

                case CardState.PendingHide:
                    _currentAnimation = StartCoroutine(FlipToFront(true));
                    break;

                case CardState.Matched:
                    _currentAnimation = StartCoroutine(MatchAnimation());
                    break;
            }
        }

        public void UpdateVisualInstant()
        {
            StopCurrentAnimation();

            switch (Card.State)
            {
                case CardState.Hidden:
                    ShowBackInstant();
                    SetInteractable(true);
                    break;

                case CardState.Revealed:
                case CardState.PendingHide:
                    ShowFrontInstant();
                    SetInteractable(Card.State == CardState.PendingHide);
                    break;

                case CardState.Matched:
                    ShowFrontInstant();
                    SetInteractable(false);
                    SetMatchedColor();
                    break;
            }
        }

        private IEnumerator FlipToFront(bool interactableAfter)
        {
            SetInteractable(false);

            if (!_isShowingFront)
            {
                yield return Anim.ScaleX(transform, 1f, 0f, _flipDuration, EaseType.InQuad);
                ShowFrontInstant();
                yield return Anim.ScaleX(transform, 0f, 1f, _flipDuration, EaseType.OutQuad);
            }

            SetInteractable(interactableAfter);
        }

        private IEnumerator FlipToBack()
        {
            SetInteractable(false);

            if (_isShowingFront)
            {
                yield return Anim.ScaleX(transform, 1f, 0f, _flipDuration, EaseType.InQuad);
                ShowBackInstant();
                yield return Anim.ScaleX(transform, 0f, 1f, _flipDuration, EaseType.OutQuad);
            }

            SetInteractable(true);
        }

        private IEnumerator MatchAnimation()
        {
            SetInteractable(false);

            if (!_isShowingFront)
            {
                yield return Anim.ScaleX(transform, 1f, 0f, _flipDuration, EaseType.InQuad);
                ShowFrontInstant();
                yield return Anim.ScaleX(transform, 0f, 1f, _flipDuration);
            }

            SetMatchedColor();
            yield return Anim.Punch(transform, Vector3.one * _matchPunchScale, 0.2f);
        }

        private void ShowFrontInstant()
        {
            _frontSide.SetActive(true);
            _backSide.SetActive(false);
            _isShowingFront = true;
        }

        private void ShowBackInstant()
        {
            _frontSide.SetActive(false);
            _backSide.SetActive(true);
            _isShowingFront = false;
        }

        private void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void SetMatchedColor()
        {
            var color = _originalColor;
            color.a = 0.6f;
            _frontImage.color = color;
        }

        private void StopCurrentAnimation()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
                transform.localScale = Vector3.one;
            }
        }
    }
}
