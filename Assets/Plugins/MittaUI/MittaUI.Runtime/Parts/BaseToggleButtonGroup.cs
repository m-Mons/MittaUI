using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;

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

            _selectedButtonIndexProperty.Value = 0;

            for (var i = 0; i < _toggleButtons.Count; i++)
            {
                _toggleButtons[i].SetToggleState(i == 0);
                _toggleButtons[i].SetDisable(i == 0);
                
            }
        }
        
        public void SetSelectedButtonIndex(int index)
        {
            if (index < 0 || index >= _toggleButtons.Count)
            {
                return;
            }

            _selectedButtonIndexProperty.Value = index;
        }
    }
}