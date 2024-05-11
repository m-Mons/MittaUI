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

        private ReactiveProperty<int> _selectedButtonIndexProperty;

        public Observable<int> OnSelectedButtonIndexChangedObservable => _selectedButtonIndexProperty;

        protected override void Awake()
        {
            base.Awake();

            _selectedButtonIndexProperty = new ReactiveProperty<int>(0);
            _selectedButtonIndexProperty.Subscribe(index =>
            {
                Debug.Log(index);
            }).AddTo(this);

            foreach (var (btn, i) in _toggleButtons.Select(static (btn, i) => (btn, i)))
            {
                btn
                    .ToggleStateProperty
                    .Where(v => v)
                    .Subscribe(_ =>
                    {
                        _selectedButtonIndexProperty.Value = i;
                        foreach (var b in _toggleButtons.Where(b => b != btn))
                        {
                            b.SetToggleState(false);
                            b.SetDisable(false);
                        }

                        btn.SetToggleState(true);
                        
                        // onの時の再度タイプ阻止
                        btn.SetDisable(true);
                    }).AddTo(this);
            }
        }
    }
}