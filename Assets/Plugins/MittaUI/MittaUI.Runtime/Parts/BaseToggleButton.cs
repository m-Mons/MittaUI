using System;
using MittaUI.Runtime.Extension;
using R3;
using UnityEngine;

namespace MittaUI.Runtime.Parts
{
    [DefaultExecutionOrder(-5)]
    [RequireComponent(typeof(SimpleButton))]
    public class BaseToggleButton : UIBehaviour
    {
        [Header("トグルボタンの初期状態")] [SerializeField]
        private bool _toggleState;

        [SerializeField] private SimpleButton _button;

        [Header("チェックボックス反映先のCanvasGroup")] [SerializeField]
        private CanvasGroup _targetGroup;

        private ReactiveProperty<bool> _toggleStateProperty { get; set; }
        public Observable<bool> ToggleStateProperty => _toggleStateProperty;
        public Action<bool> OnToggleChangedCallback { get; set; }


        protected override void Awake()
        {
            base.Awake();

            _toggleStateProperty = new ReactiveProperty<bool>(_toggleState);

            _button
                .ClickedObservable
                .Where(_ => _button.IsDisabled == false)
                .Subscribe(_ => _toggleStateProperty.Value = !_toggleStateProperty.Value)
                .AddTo(this);

            ToggleStateProperty
                .Subscribe(state =>
                {
                    _toggleState = state;
                    SetVisualState();
                    OnToggleChangedCallback?.Invoke(_toggleState);
                }).AddTo(this);
        }

        public void SetToggleState(bool state)
        {
            _toggleStateProperty.Value = state;
        }

        private void SetVisualState()
        {
            if (_toggleStateProperty.Value)
            {
                _targetGroup.Enable();
            }
            else
            {
                _targetGroup.Disable();
            }
        }

        public void SetDisable(bool disable)
        {
            _button.SetDisabled(disable);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SetVisualState();
        }
    }
}
