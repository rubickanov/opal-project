using System;
using System.Collections;
using UnityEngine;

namespace Rubickanov.Opal.Presentation.Animation
{
    public static class Anim
    {
        public static IEnumerator ScaleX(Transform target, float from, float to, float duration, EaseType ease = EaseType.OutQuad)
        {
            return Animate(duration, ease, t =>
            {
                var scale = target.localScale;
                scale.x = Mathf.LerpUnclamped(from, to, t);
                target.localScale = scale;
            });
        }

        public static IEnumerator Punch(Transform target, Vector3 punch, float duration)
        {
            var original = target.localScale;
            float halfDuration = duration * 0.5f;

            yield return Animate(halfDuration, EaseType.OutQuad, t =>
                target.localScale = Vector3.LerpUnclamped(original, original + punch, t));
            yield return Animate(halfDuration, EaseType.OutQuad, t =>
                target.localScale = Vector3.LerpUnclamped(original + punch, original, t));
        }

        public static IEnumerator Animate(float duration, EaseType ease, Action<float> onUpdate)
        {
            if (duration <= 0)
            {
                onUpdate(1f);
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = Ease.Evaluate(ease, t);
                onUpdate(eased);
                yield return null;
            }

            onUpdate(1f);
        }
    }
}
