namespace Rubickanov.Opal.Presentation.Animation
{
    public enum EaseType
    {
        Linear,
        InQuad,
        OutQuad,
        OutBack
    }

    public static class Ease
    {
        public static float Evaluate(EaseType type, float t)
        {
            return type switch
            {
                EaseType.Linear => t,
                EaseType.InQuad => t * t,
                EaseType.OutQuad => t * (2 - t),
                EaseType.OutBack => 1 + (--t) * t * (2.70158f * t + 1.70158f),
                _ => t
            };
        }
    }
}
