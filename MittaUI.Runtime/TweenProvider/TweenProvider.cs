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

namespace MittaUI.Runtime.TweenProvider
{
    public static class TweenProvider
    {
        public static async UniTask Tween(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, CancellationToken ct = default)
        {
#if MITTAUI_USE_LITMOTION
            await TweenLitMotion(from, to, duration, onUpdate, onComplete, ct);
#elif MITTAUI_USE_DOTWEEN && UNITASK_DOTWEEN_SUPPORT
            await TweenDoTween(from, to, duration, onUpdate, onComplete, ct);
#else
            await TweenPureUpdate(from, to, duration, onUpdate, onComplete, ct);
#endif
        }

        public static async UniTask Move(Vector3 from, Vector3 to, float duration, Transform target,
            Action onComplete = null,
            CancellationToken ct = default)
        {
            await Tween(0, 1, duration, t => { target.position = Vector3.Lerp(from, to, t); }, onComplete, ct);
        }

        private static async UniTask TweenPureUpdate(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, CancellationToken ct = default)
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
                await UniTask.Yield();
            }
        }


# if MITTAUI_USE_LITMOTION
        private static async UniTask TweenLitMotion(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, CancellationToken ct = default)
        {
            var tween = LMotion.Create(from, to, duration)
                .Bind(onUpdate);

            await tween.ToUniTask(ct);

            if (onComplete != null)
            {
                onComplete();
            }
        }
#endif

#if MITTAUI_USE_DOTWEEN && UNITASK_DOTWEEN_SUPPORT
        private static async UniTask TweenDoTween(float from, float to, float duration, Action<float> onUpdate,
            Action onComplete = null, CancellationToken ct = default)
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