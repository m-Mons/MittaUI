using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if MITTAUI_USE_R3
using R3;
#endif

namespace MittaUI.Runtime.Parts
{
    public class ToggleButtonGroup : UIBehaviour
    {
        [SerializeField] private List<BaseToggleButton> _toggleButtons;

        private int _selectedButtonIndex;

        public int SelectedButtonIndex
        {
            get => _selectedButtonIndex;
            set
            {
                _selectedButtonIndex = value;
                OnSelectedButtonIndexChanged?.Invoke(_selectedButtonIndex);
            }
        }

        public Action<int> OnSelectedButtonIndexChanged { get; set; }

#if MITTAUI_USE_R3
        public Observable<int> OnSelectedButtonIndexChangedObservable => Observable.FromEvent<int>(
            handler => OnSelectedButtonIndexChanged += handler,
            handler => OnSelectedButtonIndexChanged -= handler
        );
#endif

        protected override void Awake()
        {
            base.Awake();
            foreach (var (btn, i) in _toggleButtons.Select((btn, i) => (btn, i)))
            {
                btn.OnToggleChangedCallback = isOn =>
                {
                    foreach (var b in _toggleButtons.Where(b => b != btn))
                    {
                        b.ToggleState = false;
                    }

                    if (isOn)
                    {
                        SelectedButtonIndex = i;
                    }
                };
            }
        }

    }
}