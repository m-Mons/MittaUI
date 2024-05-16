using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

using LitMotion;
using MittaUI.Runtime.Extension;
using MittaUI.Runtime.TinyTween;

namespace MittaUI.Runtime.Parts
{
    public class BaseToggleTextRotateGroup : UIBehaviour
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
        
        [SerializeField] private BaseText _mainText;
        [SerializeField] private BaseText _leftText;
        [SerializeField] private BaseText _leftPreviewText;
        [SerializeField] private BaseText _rightText;
        [SerializeField] private BaseText _rightPreviewText;

        [SerializeField] private BaseButton _rightSelectButton;
        [SerializeField] private BaseButton _leftSelectButton;

        [SerializeField] private List<Transform> _textShowerList = new List<Transform>();
        private List<Vector3> _textShowerDefaultPositionsList = new List<Vector3>();

 
        private bool _canInteracitive = true;

        protected override void Awake()
        {
            base.Awake();

            _selectedTextIndexProperty = new ReactiveProperty<int>(0);
            SetTexts(0);
            _textShowerList.ForEach(t => _textShowerDefaultPositionsList.Add(t.localPosition));

            _rightSelectButton.OnClickedObservable.Where(_ => _canInteracitive).SubscribeAwait(async (_, ct) =>
            {
                await ChangeToggleValue(1,_leftText.transform.position, ct);
            }).AddTo(this);

            _leftSelectButton.OnClickedObservable.Where(_ => _canInteracitive).SubscribeAwait(async (_, ct) =>
            {
                await ChangeToggleValue(-1,_rightText.transform.position, ct);
            }).AddTo(this);
        }

        private async UniTask ChangeToggleValue(int direction, Vector3　to, CancellationToken ct)
        {
            _canInteracitive = false;

            var changedValue = direction > 0 ? _selectedTextIndexProperty.Value + 1 : _selectedTextIndexProperty.Value - 1 + _choices.Count;
            _selectedTextIndexProperty.Value = changedValue % _choices.Count;

            await
            (
                TweenProvider.Move(_textShowerList[0].position, _textShowerList[direction < 0 ? 1 : 0].position, 0.5f, _textShowerList[0]),
                TweenProvider.Move(_textShowerList[1].position, _textShowerList[1 - direction].position, 0.5f, _textShowerList[1]),
                TweenProvider.Move(_textShowerList[2].position, _textShowerList[2 - direction].position, 0.5f, _textShowerList[2]),
                TweenProvider.Move(_textShowerList[3].position, _textShowerList[3 - direction].position, 0.5f, _textShowerList[3]),
                TweenProvider.Move(_textShowerList[4].position, _textShowerList[direction > 0 ? 3 : 4].position, 0.5f, _textShowerList[4])
            );

            _textShowerDefaultPositionsList.Select((v, i) => new {Value = v, Index = i }).ForEach(_ => _textShowerList[_.Index].localPosition =  _.Value);
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
            var rightPreviewId = (id + 2) % _choices.Count;
            _rightText.SetText(_choices[rightId]);
            _rightPreviewText.SetText(_choices[rightPreviewId]);

            var leftId = (id - 1 + _choices.Count) % _choices.Count;
            var leftPreviewId = (id - 2 + _choices.Count) % _choices.Count;
            _leftText.SetText(_choices[leftId]);
            _leftPreviewText.SetText(_choices[leftPreviewId]);
        }

        private void SetMainText(string content)
        {
            _mainText.SetText(content);
        }
    }
}