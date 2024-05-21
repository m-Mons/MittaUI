using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#if MITTAUI_USE_DOTWEEN && UNITASK_DOTWEEN_SUPPORT
using DG.Tweening;
#endif

#if MITTAUI_USE_LITMOTION
using LitMotion;
#endif

namespace MittaUI.Runtime.TinyTween
{
    public static class TweenProvider
    {
        // MEMO : LitMotionはよしなにCastできるはず
        // TODO : DOTweenのEaseタイプの確認
        public enum EaseType
        {
            Linear = 0,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
            CustomAnimationCurve
        }

        public static async UniTask Tween(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, EaseType ease = default, CancellationToken ct = default, GameObject bind = null)
        {
#if MITTAUI_USE_LITMOTION
            await TweenLitMotion(from, to, duration, onUpdate, onComplete, ease, ct, bind);
#elif MITTAUI_USE_DOTWEEN && UNITASK_DOTWEEN_SUPPORT
            await TweenDoTween(from, to, duration, onUpdate, onComplete,ease, ct, bind);
#else
            await TweenPureUpdate(from, to, duration, onUpdate, onComplete, ct, bind);
#endif
        }

        public static async UniTask Move(Vector3 from, Vector3 to, float duration, Transform target,
            Action onComplete = null, EaseType ease = default, CancellationToken ct = default, GameObject bind = null)
        {
            await Tween(0, 1, duration, t => { target.position = Vector3.Lerp(from, to, t); }, onComplete, ease, ct, bind);
        }

        public static async UniTask Scale(Vector3 from, Vector3 to, float duration, Transform target,
            Action onComplete = null, EaseType ease = default, CancellationToken ct = default, GameObject bind = null)
        {
            await Tween(0, 1, duration, t => { target.localScale = Vector3.Lerp(from, to, t); }, onComplete, ease, ct, bind);
        }

        private static async UniTask TweenPureUpdate(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, CancellationToken ct = default, GameObject bind = null)
        {
            float time = 0;
            while (time < duration)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                time += Time.deltaTime;
                onUpdate.Invoke(Mathf.Lerp(from, to, time / duration));
                await UniTask.Yield(cancellationToken: ct);
            }
            
            onComplete?.Invoke();
        }


# if MITTAUI_USE_LITMOTION
        private static async UniTask TweenLitMotion(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, EaseType ease = default, CancellationToken ct = default, GameObject bind = null)
        {
            var tween = LMotion.Create(from, to, duration)
                .WithEase((Ease)ease)
                .Bind(onUpdate);
            
            if (bind != null)
            {
                tween.AddTo(bind);
            }

            await tween.ToUniTask(ct);

            if (onComplete != null)
            {
                onComplete();
            }
        }
#endif

#if MITTAUI_USE_DOTWEEN && UNITASK_DOTWEEN_SUPPORT
        private static async UniTask TweenDoTween(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null,EaseType ease = default, CancellationToken ct = default)
        {
            var tween = DOTween.To(() => from, x => onUpdate(x), to, duration);
            await tween.ToUniTask(cancellationToken: ct);
            onComplete?.Invoke();
        }
#endif

        public static async UniTask FadeAsync(this CanvasGroup graphic, float targetAlpha, float duration,
            CancellationToken ct = default)
        {
            var alpha = graphic.alpha;
            await Tween(alpha, targetAlpha, duration, x => graphic.alpha = x, ct: ct);
        }

        public static async UniTask FadeAsync(this Graphic graphic, float targetAlpha, float duration,
            CancellationToken ct = default)
        {
            // TODO ラムダ式で変数キャプチャしてるのでパフォーマンス悪い、いい感じにする
            var color = graphic.color;
            await Tween(color.a, targetAlpha, duration, x =>
            {
                color.a = x;
                graphic.color = color;
            }, ct: ct);
        }
    }
}