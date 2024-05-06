using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace MittaUI.Runtime.Parts
{
    public abstract class BaseButton : UIBehaviour
    {
        /// <summary>
        ///     ボタンがdisabled状態かどうか
        /// </summary>
        [SerializeField] protected bool _isDisabled;

        /// <summary>
        ///     Press時などのアニメーションを動かすときのためのCancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        ///     クリック時のコールバック
        /// </summary>
        public Action OnClickedCallback;

        /// <summary>
        ///     Disable状態でのクリック時コールバック
        /// </summary>
        public Action OnClickedCallbackForDisableState;

        /// <summary>
        ///     ロングタップ時のコールバック
        /// </summary>
        public Action OnLongTappedCallback;

        /// <summary>
        ///     Disable状態でのロングタップ時コールバック
        /// </summary>
        public Action OnLongTappedCallbackForDisableState;

        /// <summary>
        ///     Press時のコールバック
        /// </summary>
        public Action<bool> OnPressedCallback;

        /// <summary>
        ///     Disable状態でのPress時コールバック
        /// </summary>
        public Action<bool> OnPressedCallbackForDisableState;

        public bool IsEnabled => _isDisabled == false;

        public bool IsDisabled => _isDisabled;

        /// <summary>
        ///     クリックをキャンセルするドラッグ量のしきい値
        /// </summary>
        public virtual float ClickCancelDragThreshold { get; set; } = 30f;

        /// <summary>
        ///     ロングタップをキャンセルするドラッグ量のしきい値
        /// </summary>
        public virtual float LongTapCancelDragThreshold { get; set; } = 30f;

        protected sealed override void Awake()
        {
            base.Awake();

            var longTapGesture = GetLongTapGesture();

            Profiler.BeginSample("ButtonBase.AddCallbacks");

            AddCallbacks(longTapGesture);

            Profiler.EndSample();

            Profiler.BeginSample("ButtonBase.Initialize");

            Initialize();

            Profiler.EndSample();
        }

        protected override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            base.OnDestroy();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ImmediateDisabledTransition(_isDisabled);
        }
#endif

        /// <summary>
        ///     初期化処理
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        ///     LongTapGesture取得
        /// </summary>
        protected virtual LongTapGesture GetLongTapGesture()
        {
            Profiler.BeginSample("ButtonBase.GetTapGesture");

            var longTapGesture = gameObject.GetComponent<LongTapGesture>();

            Profiler.EndSample();

            if (longTapGesture == null)
            {
                Profiler.BeginSample("ButtonBase.AddTapGesture");

                longTapGesture = gameObject.AddComponent<LongTapGesture>();

                Profiler.EndSample();
            }

            return longTapGesture;
        }

        protected virtual void AddCallbacks(LongTapGesture longTapGesture)
        {
            // 必要なパラメータを渡す
            longTapGesture.LongTapCancelDragThreshold = LongTapCancelDragThreshold;
            longTapGesture.OnLongTapCanceledCallBack = OnLongTapCanceled;

            longTapGesture.OnClickedCallback += () =>
            {
                if (longTapGesture.DragDelta.sqrMagnitude <= ClickCancelDragThreshold * ClickCancelDragThreshold)
                    OnClickedHandler();
            };
            longTapGesture.OnLongTappedCallback += OnLongTappedHandler;
            longTapGesture.OnPressedCallback += OnPressedHandler;
        }

        /// <summary>
        ///     ボタンがdisabled状態かどうかを設定する
        /// </summary>
        public void SetDisabled(bool isDisabled)
        {
            if (this == null) return;

            if (isDisabled == _isDisabled) return;

            _isDisabled = isDisabled;
            OnDisabled(isDisabled);

            if (gameObject.activeInHierarchy)
                DisabledTransitionAsync(isDisabled, GetCt()).Forget();
            else
                ImmediateDisabledTransition(isDisabled);
        }

        /// <summary>
        ///     クリック時に処理を追加したい場合の拡張ポイント
        /// </summary>
        protected virtual void OnClicked()
        {
        }

        /// <summary>
        ///     ロングタップ時に処理を追加したい場合の拡張ポイント
        /// </summary>
        protected virtual void OnLongTapped()
        {
        }

        /// <summary>
        ///     Press時に処理を追加したい場合の拡張ポイント
        /// </summary>
        protected virtual void OnPressed(bool isPressed)
        {
        }

        /// <summary>
        ///     ボタンのDisable状態変更時に処理を追加したい場合の拡張ポイント
        /// </summary>
        /// <param name="isDisabled">If set to <c>true</c> is disabled.</param>
        protected virtual void OnDisabled(bool isDisabled)
        {
        }

        /// <summary>
        ///     クリック時のトランジション
        /// </summary>
        protected virtual UniTask ClickedTransitionAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        ///     Press時(down,up時)のトランジション
        /// </summary>
        protected virtual UniTask PressedTransitionAsync(bool isPressed, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        ///     ロングタップ時のトランジション
        /// </summary>
        protected virtual UniTask LongTappedTransitionAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnLongTapCanceled()
        {
        }

        /// <summary>
        ///     Disabled(enabled)時のトランジション
        /// </summary>
        protected virtual UniTask DisabledTransitionAsync(bool isDisabled, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        ///     Disabled(enabled)時のトランジション
        ///     activeInHierarchy == false時のトランジションとして使用される
        ///     また、OnValidate()からも呼び出される
        /// </summary>
        protected virtual void ImmediateDisabledTransition(bool isDisabled)
        {
        }

        /// <summary>
        ///     トランジションのためのCancellationTokenを取得する
        /// </summary>
        private CancellationToken GetCt()
        {
            if (_cts != null) return _cts.Token;

            _cts = new CancellationTokenSource();
            return _cts.Token;
        }

        #region ---- LongTapGesture callback handler ----

        protected void OnClickedHandler()
        {
            if (IsDisabled)
            {
                OnClickedCallbackForDisableState?.Invoke();
                return;
            }

            OnClicked();
            ClickedTransitionAsync(GetCt()).Forget();
            OnClickedCallback?.Invoke();
        }

        protected void OnLongTappedHandler()
        {
            if (IsDisabled)
            {
                OnLongTappedCallbackForDisableState?.Invoke();
                return;
            }

            OnLongTapped();
            LongTappedTransitionAsync(GetCt()).Forget();
            OnLongTappedCallback?.Invoke();
        }

        protected void OnPressedHandler(bool isPressed)
        {
            if (IsDisabled)
            {
                OnPressedCallbackForDisableState?.Invoke(isPressed);
                return;
            }

            OnPressed(isPressed);
            PressedTransitionAsync(isPressed, GetCt()).Forget();
            OnPressedCallback?.Invoke(isPressed);
        }

        #endregion
    }
}