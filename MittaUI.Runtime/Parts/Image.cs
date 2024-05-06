using MittaUI.Runtime.Constant;
using MittaUI.Runtime.Extension;
using MittaUI.Runtime.Parts;
#if MITTAUI_USE_R3
using R3;
#endif
using UnityEditorInternal;
using UnityEngine;

#if MITTAUI_USE_UPALETTE
using uPalette.Runtime.Core;
#endif

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

#if MITTAUI_USE_UPALETTE
        /// <summary>
        ///     色のEntryId(uPaletteのもの)
        /// </summary>
        [SerializeField] private ColorEntryId _entryId = new();
#endif

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
#if MITTAUI_USE_UPALETTE
            _image.SetColorFromEntryId(_entryId);
#endif
#if MITTAUI_USE_R3 && MITTAUI_USE_UPALETTE
            SubscribePaletteStore();
#endif
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
#if MITTAUI_USE_UPALETTE
        /// <summary>
        /// Color設定
        /// </summary>
        public void SetColor(string colorStyleEntryId)
        {
            _entryId.Value = colorStyleEntryId;
            _image.SetColorFromEntryId(_entryId);
        }
#endif

#if MITTAUI_USE_UPALETTE && MITTAUI_USE_R3
        private void SubscribePaletteStore()
        {
            PaletteStore.Instance.ColorPalette.TryGetActiveValue(_entryId.Value, out var value);

            if (value != null)
                Observable.EveryValueChanged(value, x => x.Value)
                    .Subscribe(color => _image.color = color).AddTo(this);
        }
#endif
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            TryGetComponent(out _image);
            // Imageはこのコンポーネントより下にする
            ComponentUtility.MoveComponentDown(_image);
            // デフォルトfalse
            _image.raycastTarget = false;

#if MITTAUI_USE_UPALETTE
            _entryId.Value = string.Empty;
#endif
        }
#if MITTAUI_USE_UPALETTE
        protected override void OnValidate()
        {
            base.OnValidate();
            _image.SetColorFromEntryId(_entryId);
        }
#endif
#endif
    }
}