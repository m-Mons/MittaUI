#if MITTAUI_USE_R3
using R3;
#endif
using UnityEngine;

namespace MittaUI.Runtime.Parts
{
    [RequireComponent(typeof(LongTapGesture))]
    public abstract class BaseButtonWithLongTapGesture : BaseButton
    {
        [SerializeField] protected float _longTapTime = 0.5f;

        [SerializeField] protected LongTapGesture _longTapGesture;

#if MITTAUI_USE_R3

        /// <summary>
        ///     クリック時のイベント（ロングタップ時には呼ばれない）
        /// </summary>
        public Observable<Unit> ClickedObservable => Observable.FromEvent(
            x => _longTapGesture.OnClickedCallback += x,
            x => _longTapGesture.OnClickedCallback -= x);

        /// <summary>
        ///     ロングタップ時のイベント
        /// </summary>
        public Observable<Unit> LongTappedObservable => Observable.FromEvent(
            x => OnLongTappedCallback += x,
            x => OnLongTappedCallback -= x);

        /// <summary>
        ///     ロングタップ中にキャンセルしたときのイベント
        /// </summary>
        public Observable<Unit> LongTapCanceledObservable => Observable.FromEvent(
            x => _longTapGesture.OnLongTapCanceledCallBack += x,
            x => _longTapGesture.OnLongTapCanceledCallBack -= x);

        /// <summary>
        ///     Press時のイベント
        /// </summary>
        public Observable<bool> PressedObservable => Observable.FromEvent<bool>(
            x => OnPressedCallback += x,
            x => OnPressedCallback -= x);

        /// <summary>
        ///     Disable状態でのクリック時のイベント
        /// </summary>
        public Observable<Unit> ClickedForDisableStateObservable => Observable.FromEvent(
            x => OnClickedCallbackForDisableState += x,
            x => OnClickedCallbackForDisableState -= x);

        /// <summary>
        ///     Disable状態でのロングタップ時のイベント
        /// </summary>
        public Observable<Unit> LongTappedForDisableStateObservable => Observable.FromEvent(
            x => OnLongTappedCallbackForDisableState += x,
            x => OnLongTappedCallbackForDisableState -= x);

        /// <summary>
        ///     Disable状態でのPress時のイベント
        /// </summary>
        public Observable<bool> PressedForDisableStateObservable => Observable.FromEvent<bool>(
            handler => OnPressedCallbackForDisableState += handler,
            handler => OnPressedCallbackForDisableState -= handler);
#endif
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _longTapGesture ??= GetComponent<LongTapGesture>();
        }
#endif

        protected override void Start()
        {
            base.Start();
            _longTapGesture.LongTapTime = _longTapTime;
        }

        protected override LongTapGesture GetLongTapGesture()
        {
            return _longTapGesture;
        }
    }
}