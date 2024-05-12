using System;
using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MittaUI.Runtime.Parts
{
    public class LongTapGesture : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        /// <summary>
        /// クリック判定種別
        /// </summary>
        public enum ClickDetectType
        {
            OnClick, // OnClickで行う
            OnPointerUp // OnPointerUpで行う
        }

        /// <summary>
        /// ロングタップを認識するまでの時間
        /// </summary>
        public virtual float LongTapTime { get; set; } = 0.5f;

        /// <summary>
        /// 最後にpressされた時間
        /// </summary>
        private float _lastPressTime;

        /// <summary>
        /// ドラッグされたかどうか
        /// </summary>
        public bool IsDragged { get; private set; }

        /// <summary>
        /// 押下されているかどうか
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Press時のpointerId
        /// </summary>
        public int PressedPointerId { get; private set; }

        /// <summary>
        /// クリック時のコールバック（ロングタップ時には呼ばれない）
        /// </summary>
        public Observable<Unit> ClickedObservable => _onClickedSubject;

        private readonly Subject<Unit> _onClickedSubject = new Subject<Unit>();


        /// <summary>
        /// ロングタップ時のコールバック
        /// </summary>
        public Observable<Unit> OnLongTappedObservable => _onLongTappedSubject;

        private readonly Subject<Unit> _onLongTappedSubject = new Subject<Unit>();

        /// <summary>
        /// Press時のコールバック
        /// </summary>
        public Observable<bool> OnPressedObservable => _onPressedSubject;

        private readonly Subject<bool> _onPressedSubject = new Subject<bool>();


        /// <summary>
        /// ロングタップ中にキャンセルしたときのコールバック
        /// </summary>
        public Observable<Unit> LongTapCanceledObservable => _onLongTapCanceledSubject;

        private readonly Subject<Unit> _onLongTapCanceledSubject = new Subject<Unit>();

        /// <summary>
        /// タップの位置
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// クリック判定時に指を離したraycasterからみたscreen座標
        /// </summary>
        public Vector2 ClickedPosition { get; private set; }

        /// <summary>
        /// ドラッグによる移動量
        /// </summary>
        public Vector2 DragDelta { get; private set; }

        /// <summary>
        /// マルチタッチを認めないかどうか
        /// </summary>
        public bool IsMultiTouchDisabled { get; private set; }

        /// <summary>
        /// ロングタップ発動のためのコルーチン
        /// </summary>
        private Coroutine _longTapCoroutine;

        /// <summary>
        /// ロングタップ発火のしきい値
        /// </summary>
        public float LongTapCancelDragThreshold { get; set; }

        /// <summary>
        /// ロングタップ待機中かどうか
        /// </summary>
        private bool _isLongTapWaiting;

        /// <summary>
        /// ドラッグイベントを貫通させるかどうか
        /// </summary>
        public bool ShouldPenetrateDragEvent { get; set; } = true;

        /// <summary>
        /// クリックイベント判定タイプ
        /// </summary>
        public ClickDetectType ClickType { get; set; } = ClickDetectType.OnClick;

        /// <summary>
        /// ロングタップになるほど長くタップしていたらクリック時の処理はしない
        /// </summary>
        public bool SuppressLongTapClick { get; set; } = true;

        /// <summary>
        /// マルチタッチをdisabled状態にするかどうかを設定する
        /// </summary>
        public void SetMultiTouchDisabled(bool isDisabled)
        {
            IsMultiTouchDisabled = isDisabled;
        }

        /// <summary>
        /// OnPointerClickイベント
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (ClickType != ClickDetectType.OnClick) return;
            if (IsMultiTouchDisabled && eventData.pointerId > 0)
                // pointerIdはマウスだとマイナス
                return;

            CheckClick(eventData);
        }

        /// <summary>
        /// クリックチェック
        /// クリックしてたらコールバック発火
        /// </summary>
        protected virtual void CheckClick(PointerEventData eventData)
        {
            if (SuppressLongTapClick && Time.realtimeSinceStartup - _lastPressTime >= LongTapTime)
                // ロングタップになるほど長くタップしていたらクリック時の処理はしない
                return;

            ClickedPosition = eventData.position;

            _onClickedSubject.OnNext(default);
        }

        /// <summary>
        /// OnPointerDownイベント
        /// </summary>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (IsMultiTouchDisabled && eventData.pointerId > 0) return;

            IsPressed = true;
            PressedPointerId = eventData.pointerId;
            IsDragged = false;
            _lastPressTime = Time.realtimeSinceStartup;
            Position = eventData.position;
            DragDelta = Vector2.zero;
            _onPressedSubject.OnNext(true);
            CancelLongTapCoroutine();
            _longTapCoroutine = StartCoroutine(LongTapEnumerator());
        }

        /// <summary>
        /// OnPointerDownイベント
        /// </summary>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (IsMultiTouchDisabled && eventData.pointerId > 0) return;

            IsPressed = false;
            _onPressedSubject.OnNext(false);
            CancelLongTapCoroutine();

            if (ClickType == ClickDetectType.OnPointerUp) CheckClick(eventData);
        }

        protected virtual void OnApplicationPause(bool isPause)
        {
            if (isPause == false)
                if (IsPressed)
                    // 復帰時に指を離した扱いする
                    OnPointerUp(new PointerEventData(EventSystem.current) { pointerId = PressedPointerId });
        }

        /// <summary>
        /// ドラッグイベント全てを親に伝播させるために定義している
        /// </summary>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            PenetrateDragEvent(eventData, ExecuteEvents.initializePotentialDrag);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (IsMultiTouchDisabled && eventData.pointerId > 0)
            {
                PenetrateDragEvent(eventData, ExecuteEvents.beginDragHandler);
                return;
            }

            DragDelta = eventData.delta;
            PenetrateDragEvent(eventData, ExecuteEvents.beginDragHandler);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            PenetrateDragEvent(eventData, ExecuteEvents.endDragHandler);
        }

        /// <summary>
        /// OnDragイベント
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (IsMultiTouchDisabled && eventData.pointerId > 0)
            {
                PenetrateDragEvent(eventData, ExecuteEvents.dragHandler);
                return;
            }

            IsDragged = true;
            DragDelta += eventData.delta;
            PenetrateDragEvent(eventData, ExecuteEvents.dragHandler);

            if (LongTapCancelDragThreshold > 0 &&
                DragDelta.sqrMagnitude > LongTapCancelDragThreshold * LongTapCancelDragThreshold)
                CancelLongTapCoroutine();
        }

        /// <summary>
        /// ドラッグイベント貫通処理
        /// </summary>
        private void PenetrateDragEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> eventFunc)
            where T : IEventSystemHandler
        {
            if (ShouldPenetrateDragEvent == false) return;
            UIEventUtility.PenetrateParentHandler(transform, eventData, eventFunc);
        }

        /// <summary>
        /// ロングタップが認識されたときに呼ばれる
        /// </summary>
        private IEnumerator LongTapEnumerator()
        {
            _isLongTapWaiting = true;
            yield return new WaitForSeconds(LongTapTime);
            _isLongTapWaiting = false;
            _onLongTappedSubject.OnNext(default);
        }

        /// <summary>
        /// ロングタップのコルーチンが発動中ならキャンセルする
        /// </summary>
        private void CancelLongTapCoroutine()
        {
            if (_isLongTapWaiting) _onLongTapCanceledSubject.OnNext(default);

            _isLongTapWaiting = false;
            if (_longTapCoroutine != null)
            {
                StopCoroutine(_longTapCoroutine);
                _longTapCoroutine = null;
            }
        }
    }
}