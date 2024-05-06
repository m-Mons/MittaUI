using System;
using MittaUI.Runtime.Extension;
#if MITTAUI_USE_R3
using R3;
#endif
using UnityEngine;

namespace MittaUI.Runtime.Parts
{
    [RequireComponent(typeof(SimpleButton))]
    public class BaseToggleButton : UIBehaviour
    {
        [SerializeField] private bool _toggleState;
        
        [SerializeField] private BaseButton _button;

        [Header("チェックボックス反映先のCanvasGroup")] [SerializeField]
        private CanvasGroup _targetGroup;

        public bool ToggleState
        {
            get => _toggleState;
            set
            {
                _toggleState = value;
                OnToggleChangedCallback?.Invoke(_toggleState);
            }
        }

        public Action<bool> OnToggleChangedCallback { get; set; }

#if MITTAUI_USE_R3
        public Observable<bool> OnToggleChanged => Observable.FromEvent<bool>(
            handler => OnToggleChangedCallback += handler,
            handler => OnToggleChangedCallback -= handler
        );
#endif
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_toggleState)
            {
                _targetGroup.Enable();
            }
            else
            {
                _targetGroup.Disable();
            }
        }
    }
}