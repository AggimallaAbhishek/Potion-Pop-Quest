using UnityEngine;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Easing functions for premium animation curves.
    /// All functions accept t in [0,1] and return an eased value.
    /// </summary>
    public static class EasingFunctions
    {
        /// <summary>
        /// Classic smooth-step (same as the existing Smooth function).
        /// </summary>
        public static float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// Smooth acceleration and deceleration.
        /// </summary>
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
        }

        /// <summary>
        /// Fast start, gentle stop — ideal for screen transitions.
        /// </summary>
        public static float EaseOutQuart(float t)
        {
            return 1f - Mathf.Pow(1f - t, 4f);
        }

        /// <summary>
        /// Fast, readable deceleration for tile spawns and light UI motion.
        /// </summary>
        public static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        /// <summary>
        /// Gentle start, fast end — ideal for exits.
        /// </summary>
        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        /// <summary>
        /// Overshoots at the end and settles back — ideal for tile drops landing.
        /// </summary>
        public static float EaseOutBack(float t, float overshoot = 1.70158f)
        {
            var c = overshoot + 1f;
            return 1f + c * Mathf.Pow(t - 1f, 3f) + overshoot * Mathf.Pow(t - 1f, 2f);
        }

        /// <summary>
        /// Exaggerated overshoot for dramatic reveals (star popups, potion creation).
        /// </summary>
        public static float EaseOutBackStrong(float t)
        {
            return EaseOutBack(t, 2.8f);
        }

        /// <summary>
        /// Bounces at the end like a ball — ideal for tile drops.
        /// </summary>
        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
            {
                return n1 * t * t;
            }

            if (t < 2f / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }

        /// <summary>
        /// Springy elastic effect — ideal for potion creation and spawns.
        /// </summary>
        public static float EaseOutElastic(float t)
        {
            if (t <= 0f)
            {
                return 0f;
            }

            if (t >= 1f)
            {
                return 1f;
            }

            const float c4 = 2f * Mathf.PI / 3f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        /// <summary>
        /// Gentle elastic with less oscillation — ideal for UI elements.
        /// </summary>
        public static float EaseOutElasticGentle(float t)
        {
            if (t <= 0f)
            {
                return 0f;
            }

            if (t >= 1f)
            {
                return 1f;
            }

            const float c4 = 2f * Mathf.PI / 4.5f;
            return Mathf.Pow(2f, -8f * t) * Mathf.Sin((t * 8f - 0.75f) * c4) + 1f;
        }

        /// <summary>
        /// Sine-based ease-out — natural deceleration.
        /// </summary>
        public static float EaseOutSine(float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }

        /// <summary>
        /// Exponential ease-out — fast start, very gentle end.
        /// </summary>
        public static float EaseOutExpo(float t)
        {
            return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }

        /// <summary>
        /// Produces a ping-pong pulse: 0 → 1 → 0 using a sine curve.
        /// Useful for continuous pulsing effects.
        /// </summary>
        public static float PingPong(float t)
        {
            return Mathf.Sin(t * Mathf.PI);
        }

        /// <summary>
        /// Returns a looping pulse value based on elapsed time and period.
        /// Output oscillates smoothly between 0 and 1.
        /// </summary>
        public static float LoopingPulse(float elapsed, float period)
        {
            return (Mathf.Sin(elapsed / period * Mathf.PI * 2f - Mathf.PI * 0.5f) + 1f) * 0.5f;
        }

        /// <summary>
        /// Squash-and-stretch curve for tile landing impacts.
        /// Goes through: normal → squash (0.88) → stretch (1.06) → settle (1.0)
        /// </summary>
        public static float SquashStretch(float t)
        {
            if (t < 0.3f)
            {
                // Squash phase: scale down to 0.88
                var squashT = t / 0.3f;
                return Mathf.Lerp(1f, 0.88f, EaseOutCubic(squashT));
            }

            if (t < 0.6f)
            {
                // Stretch phase: spring up to 1.06
                var stretchT = (t - 0.3f) / 0.3f;
                return Mathf.Lerp(0.88f, 1.06f, EaseOutCubic(stretchT));
            }

            // Settle phase: ease back to 1.0
            var settleT = (t - 0.6f) / 0.4f;
            return Mathf.Lerp(1.06f, 1f, EaseOutElasticGentle(settleT));
        }

        /// <summary>
        /// Cycles through HSV hue for rainbow color effects.
        /// Returns a fully saturated color at hue position t (0..1).
        /// </summary>
        public static Color RainbowHue(float t)
        {
            return Color.HSVToRGB(t % 1f, 0.85f, 1f);
        }

        /// <summary>
        /// Damped oscillation for jelly/candy wobble effects.
        /// Returns values centered around 0, decaying over time.
        /// </summary>
        public static float Wobble(float t, float frequency = 8f)
        {
            return Mathf.Sin(t * frequency * Mathf.PI) * Mathf.Pow(1f - t, 2f);
        }

        /// <summary>
        /// Quadratic ease-in-out — smooth symmetric motion.
        /// </summary>
        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
        }
    }
}
