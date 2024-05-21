using Cysharp.Threading.Tasks;
using MittaUI.Runtime.Constant;
using MittaUI.Runtime.Extension;
using MittaUI.Runtime.Parts;
using R3;

#if UNITY_EDITOR
using UnityEditorInternal;
#endif

using UnityEngine;

using uPalette.Runtime.Core;

namespace MittaUI.Runtime
{
    /// <summary>
    ///     Imageコンポーネント
    /// </summary>
    [AddComponentMenu(AddComponentMenuConst.ImagePath + nameof(Image))]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    [DisallowMultipleComponent]
    public sealed class Image : UIBehaviour
    {
        /// <summary>
        ///     表示するImage本体
        /// </summary>
        [SerializeField] private UnityEngine.UI.Image _image;

        /// <summary>
        ///     色のEntryId(uPaletteのもの)
        /// </summary>
        [SerializeField] private ColorEntryId _entryId = new();


        /// <summary>
        ///     ImageType
        /// </summary>
        public UnityEngine.UI.Image.Type ImageType => _image.type;

        /// <summary>
        ///     画像の表示割合
        /// </summary>
        public float FillAmount => _image.fillAmount;

        public new RectTransform RectTransform => base.RectTransform;

        /// <summary>
        ///     Spriteのサイズ
        /// </summary>
        public Vector2 SpriteSize => _image.sprite.rect.size;

        protected override void Awake()
        {
            base.Awake();
            _image.SetColorFromEntryId(_entryId);

            SubscribePaletteStore();

        }

        /// <summary>
        ///     Spriteの設定
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        /// <summary>
        ///     設定されている画像のサイズに合わせる
        /// </summary>
        public void SetNativeSize()
        {
            _image.SetNativeSize();
        }

        /// <summary>
        ///     FillAmountの設定
        /// </summary>
        public void SetFillAmount(float fillAmount)
        {
            _image.fillAmount = fillAmount;
        }
        /// <summary>
        /// Color設定
        /// </summary>
        public void SetColor(string colorStyleEntryId)
        {
            _entryId.Value = colorStyleEntryId;
            _image.SetColorFromEntryId(_entryId);
        }
        private void SubscribePaletteStore()
        {
            if(PaletteStore.Instance == null) return;
            PaletteStore.Instance.ColorPalette.TryGetActiveValue(_entryId.Value, out var value);

            if (value != null)
            {
                Observable.EveryValueChanged(value, x => x.Value)
                    .Subscribe(color => _image.color = color).AddTo(destroyCancellationToken);
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            TryGetComponent(out _image);
            // Imageはこのコンポーネントより下にする
            ComponentUtility.MoveComponentDown(_image);
            // デフォルトfalse
            _image.raycastTarget = false;

            _entryId.Value = string.Empty;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _image.SetColorFromEntryId(_entryId);
        }

#endif
    }
}