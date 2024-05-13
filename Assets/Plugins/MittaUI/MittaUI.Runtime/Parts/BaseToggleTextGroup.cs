using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [SerializeField] private BaseText _titleText;
        public string Title
        {
            get => _titleText.Text;
            set => _titleText.SetText(value);
        }
        
        [SerializeField] private Transform _TextContentTransform;
        [SerializeField] private BaseText _mainText;
        [SerializeField] private BaseText _leftText;
        [SerializeField] private BaseText _rightText;

        [SerializeField] private BaseButton _rightSelectButton;
        [SerializeField] private BaseButton _leftSelectButton;

        private bool _canInteracitive = true;

        protected override void Awake()
        {
            base.Awake();

            _selectedTextIndexProperty = new ReactiveProperty<int>(0);
            SetTexts(0);

            _rightSelectButton.OnClickedObservable.Where(_ => _canInteracitive).SubscribeAwait(async (_, ct) =>
            {
                await ChangeToggleValue(_selectedTextIndexProperty.Value + 1,_leftText.transform.position, ct);
            }).AddTo(this);

            _leftSelectButton.OnClickedObservable.Where(_ => _canInteracitive).SubscribeAwait(async (_, ct) =>
            {
                await ChangeToggleValue(_selectedTextIndexProperty.Value - 1 + _choices.Count,_rightText.transform.position, ct);
            }).AddTo(this);
        }

        private async UniTask ChangeToggleValue(int value, Vector3　to, CancellationToken ct)
        {
            _canInteracitive = false;

            _selectedTextIndexProperty.Value = value % _choices.Count;

            await TweenProvider.Move(_mainText.transform.position, to, 0.5f,
                _TextContentTransform);
            _TextContentTransform.localPosition = Vector3.zero;

            SetTexts(_selectedTextIndexProperty.Value);

            _canInteracitive = true;
        }
        
        public string GetSelectedValue()
        {
            return _choices[_selectedTextIndexProperty.Value];
        }
        
        private void SetTexts(in int id)
        {
            _mainText.SetText(_choices[id]);
            var rightId = (id + 1) % _choices.Count;
            var leftId = (id - 1 + _choices.Count) % _choices.Count;
            _rightText.SetText(_choices[rightId]);
            _leftText.SetText(_choices[leftId]);
        }

        private void SetMainText(string content)
        {
            _mainText.SetText(content);
        }
    }
}