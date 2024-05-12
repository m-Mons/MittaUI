using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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
        public Observable<Unit> OnClickedObservable => _onClickedSubject;

        private readonly Subject<Unit> _onClickedSubject = new Subject<Unit>();

        /// <summary>
        ///     Disable状態でのクリック時コールバック
        /// </summary>
        public Observable<Unit> OnClickedForDisableStateObservable => _onClickedForDisableStateSubject;

        private readonly Subject<Unit> _onClickedForDisableStateSubject = new Subject<Unit>();

        /// <summary>
        ///     ロングタップ時のコールバック
        /// </summary>
        public Observable<Unit> OnLongTappedObservable => _onLongTappedSubject;

        private readonly Subject<Unit> _onLongTappedSubject = new Subject<Unit>();

        /// <summary>
        ///     Disable状態でのロングタップ時コールバック
        /// </summary>
        public Observable<Unit> OnLongTappedForDisableStateObservable => _onLongTappedForDisableStateSubject;

        private readonly Subject<Unit> _onLongTappedForDisableStateSubject = new Subject<Unit>();

        /// <summary>
        ///     Press時のコールバック
        /// </summary>
        public Observable<bool> OnPressedObservable => _onPressedSubject;

        private readonly Subject<bool> _onPressedSubject = new Subject<bool>();

        /// <summary>
        ///     Disable状態でのPress時コールバック
        /// </summary>
        public Observable<bool> OnPressedForDisableStateObservable => _onPressedForDisableStateSubject;

        private readonly Subject<bool> _onPressedForDisableStateSubject = new Subject<bool>();

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
            longTapGesture
                .LongTapCanceledObservable
                .Subscribe(_ => OnLongTapCanceled())
                .AddTo(destroyCancellationToken);

            longTapGesture
                .ClickedObservable
                .Subscribe(_ =>
                {
                    if (longTapGesture.DragDelta.sqrMagnitude <= ClickCancelDragThreshold * ClickCancelDragThreshold)
                        OnClickedHandler();
                })
                .AddTo(destroyCancellationToken);

            longTapGesture.OnLongTappedObservable
                .Subscribe(_ => OnLongTappedHandler())
                .AddTo(destroyCancellationToken);

            longTapGesture
                .OnPressedObservable
                .Subscribe(OnPressedHandler)
                .AddTo(destroyCancellationToken);
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
                _onClickedForDisableStateSubject.OnNext(Unit.Default);
                return;
            }

            OnClicked();
            ClickedTransitionAsync(GetCt()).Forget();
            _onClickedSubject.OnNext(Unit.Default);
        }

        protected void OnLongTappedHandler()
        {
            if (IsDisabled)
            {
                _onLongTappedForDisableStateSubject.OnNext(Unit.Default);
                return;
            }

            OnLongTapped();
            LongTappedTransitionAsync(GetCt()).Forget();
            _onLongTappedSubject.OnNext(Unit.Default);
        }

        protected void OnPressedHandler(bool isPressed)
        {
            if (IsDisabled)
            {
                _onPressedForDisableStateSubject.OnNext(isPressed);
                return;
            }

            OnPressed(isPressed);
            PressedTransitionAsync(isPressed, GetCt()).Forget();
            _onPressedSubject.OnNext(isPressed);
        }

        #endregion
    }
}