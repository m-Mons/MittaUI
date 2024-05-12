using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

using LitMotion;
using MittaUI.Runtime.TinyTween;

namespace MittaUI.Runtime.Parts
{
    public class BaseToggleTextGroup : UIBehaviour
    {
        [SerializeField] private List<string> _choices;
        private ReactiveProperty<int> _selectedTextIndexProperty;
        public Observable<int> OnSelectedTextIndexChangedObservable => _selectedTextIndexProperty;

        [SerializeField] private SimpleText _titleText;
        public string Title
        {
            get => _titleText.Text;
            set => _titleText.SetText(value);
        }
        
        [SerializeField] private Transform _TextContent;
        [SerializeField] private SimpleText _mainText;
        [SerializeField] private SimpleText _leftText;
        [SerializeField] private SimpleText _rightText;

        [SerializeField] private BaseButton _rightSelectButton;
        [SerializeField] private BaseButton _leftSelectButton;

        private bool _canInteracitive = true;

        protected override void Awake()
        {
            base.Awake();

            _selectedTextIndexProperty = new ReactiveProperty<int>(0);
            SetTexts(0);
            
            _selectedTextIndexProperty.Subscribe(index =>
            {
                Debug.Log(index);
            }).AddTo(this);

            _rightSelectButton.OnClickedObservable.SubscribeAwait(async (_, ct) =>
            {
                if(!_canInteracitive)return;
                _canInteracitive = false;
                
                if(_selectedTextIndexProperty.Value + 1 > _choices.Count - 1) {_selectedTextIndexProperty.Value = 0;}
                else {_selectedTextIndexProperty.Value++;}

                await TweenProvider.Move(_mainText.transform.position, _leftText.transform.position, 0.5f, _TextContent);
                _TextContent.localPosition = Vector3.zero;

                SetTexts(_selectedTextIndexProperty.Value);
                
                _canInteracitive = true;
            }).AddTo(this);

            _leftSelectButton.OnClickedObservable.SubscribeAwait(async (_, ct) =>
            {
                if (!_canInteracitive) return;
                _canInteracitive = false;

                if (_selectedTextIndexProperty.Value - 1 < 0)
                {
                    _selectedTextIndexProperty.Value = _choices.Count - 1;
                }
                else
                {
                    _selectedTextIndexProperty.Value--;
                }

                await TweenProvider.Move(_mainText.transform.position, _rightText.transform.position, 0.5f,
                    _TextContent);
                _TextContent.localPosition = Vector3.zero;

                SetTexts(_selectedTextIndexProperty.Value);

                _canInteracitive = true;
            }).AddTo(this);
        }

        public string GetSelectedValue()
        {
            return _choices[_selectedTextIndexProperty.Value];
        }
        
        private void SetTexts(in int id)
        {
            _mainText.SetText(_choices[id]);
            var rightId = id + 1 > _choices.Count - 1 ? 0 : id + 1;
            var leftId = id - 1 < 0 ? _choices.Count - 1 : id - 1;
            _rightText.SetText(_choices[rightId]);
            _leftText.SetText(_choices[leftId]);
        }

        private void SetMainText(string content)
        {
            _mainText.SetText(content);
        }
    }
}