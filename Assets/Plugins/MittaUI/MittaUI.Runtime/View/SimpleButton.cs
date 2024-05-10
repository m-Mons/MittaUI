using System.Threading;
using Cysharp.Threading.Tasks;
using MittaUI.Runtime.Constant;
using MittaUI.Runtime.Parts;
using UnityEngine;
using MittaUI.Runtime.TinyTween;
#if MITTAUI_USE_NAUGHTYATTRIBUTES
using NaughtyAttributes;
#endif

namespace MittaUI.Runtime
{
    [AddComponentMenu(AddComponentMenuConst.ButtonBath + nameof(SimpleButton))]
    public sealed class SimpleButton : BaseButtonWithLongTapGesture
    {
        [Header("===ボタンのアニメーションのパラメータ===")] [Header("アニメーションをさせるかどうか    ")] [SerializeField]
        private bool _doAnimation = true;

        [SerializeField]
        [Header("ボタンをアニメーションさせるTransform")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private Transform _buttonAnimationTransform;

        [SerializeField]
        [Header("押下後のスケール")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private float _buttonAnimationPressScale = 0.9f;

        [SerializeField]
        [Header("完全に押下するまでの時間")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private float _buttonAnimationPressDuration = 0.25f;

        [SerializeField]
        [Header("押下時のイージング")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private TweenProvider.EaseType _buttonAnimationPressEase =
            TweenProvider.EaseType.OutCubic;

        [SerializeField]
        [Header("元の大きさに戻るまでの時間")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private float _buttonAnimationPullDuration = 0.25f;

        [SerializeField]
        [Header("デフォルトのスケール")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private float _buttonAnimationDefaultScale = 1f;

        [SerializeField]
        [Header("元の大きさの戻る時のイージング")]
#if MITTAUI_USE_NAUGHTYATTRIBUTES
        [ShowIf(nameof(_doAnimation))]
#endif
        private TweenProvider.EaseType _buttonAnimationPullEase =
            TweenProvider.EaseType.OutBounce;

        private CancellationTokenSource _animeCts;

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _buttonAnimationTransform ??= GetComponent<Transform>();
        }
#endif

        protected override void OnPressed(bool isPressed)
        {
            if (!_doAnimation) return;

            if (_animeCts != null)
            {
                _animeCts.Cancel();
            }

            _animeCts = new CancellationTokenSource();

            if (isPressed)
            {
                TweenProvider.Scale(Vector3.one, Vector3.one * _buttonAnimationPressScale,
                    _buttonAnimationPressDuration, _buttonAnimationTransform,
                    ease: _buttonAnimationPullEase,
                    ct: _animeCts.Token).Forget();
            }
            else
            {
                TweenProvider.Scale(Vector3.one * _buttonAnimationPressScale, Vector3.one,
                    _buttonAnimationPullDuration, _buttonAnimationTransform,
                    ease: _buttonAnimationPullEase,
                    ct: _animeCts.Token).Forget();
            }
        }
    }
}