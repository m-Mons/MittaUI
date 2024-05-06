using Cysharp.Threading.Tasks;
using MittaUI.Runtime.TweenProvider;
using UnityEngine;

namespace MittaUI.Runtime.Extension
{
    public static class CanvasGroupExtensions
    {
        public static void Enable(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        public static async UniTask EnableAsync(this CanvasGroup canvasGroup, float fadeDuration)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            // TODO : アニメーション実装
            await canvasGroup.FadeAsync(1f, fadeDuration);
        }

        public static void Disable(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        public static async UniTask DisableAsync(this CanvasGroup canvasGroup, float fadeDuration)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            // TODO : アニメーション実装
            await canvasGroup.FadeAsync(0f, fadeDuration);
        }
    }
}