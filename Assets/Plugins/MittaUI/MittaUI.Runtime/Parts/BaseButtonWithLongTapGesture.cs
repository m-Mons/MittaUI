using R3;
using UnityEngine;

namespace MittaUI.Runtime.Parts
{
    [RequireComponent(typeof(LongTapGesture))]
    public abstract class BaseButtonWithLongTapGesture : BaseButton
    {
        [SerializeField] protected float _longTapTime = 0.5f;

        [SerializeField] protected LongTapGesture _longTapGesture;

        /// <summary>
        ///     クリック時のイベント（ロングタップ時には呼ばれない）
        /// </summary>
        public Observable<Unit> ClickedObservable => _longTapGesture.ClickedObservable;

        /// <summary>
        ///     ロングタップ中にキャンセルしたときのイベント
        /// </summary>
        public Observable<Unit> LongTapCanceledObservable => _longTapGesture.OnLongTappedObservable;

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