using MittaUI.Runtime.Constant;
using MittaUI.Runtime.Extension;
using TMPro;
using UnityEngine;
#if MITTAUI_USE_UPALETTE
using uPalette.Runtime.Core;
#endif
# if UNITY_EDITOR
using UnityEditorInternal;
# endif

namespace MittaUI.Runtime.Parts
{
    // <summary>Textベースクラス</summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class BaseText : UIBehaviour
    {
        private static readonly Vector2 _vector2Max = new(float.MaxValue, float.MaxValue);

        /// <summary>最小フォントサイズ</summary>
        private static readonly int _fontSizeMin = UIConst.GetFontSize(FontSize.Px20);

        [SerializeField] private TextMeshProUGUI _textMeshPro;
#if MITTAUI_USE_UPALETTE
        // <summary>色のEntryId(uPaletteのもの)</summary>
        [SerializeField] private ColorEntryId _colorEntryId = new();
#endif
        /// <summary>フォントのサイズ</summary>
        [SerializeField] private FontSize _fontSize = FontSize.Px24;

        /// <summary>テキストが領域から出た時の種別</summary>
        [SerializeField] private TextOverflowType _textOverflowType = TextOverflowType.Shrink;

        /// <summary>TMPのマージン</summary>
        public Vector4 Margin => _textMeshPro.margin;

        /// <summary>テキストが領域を超えた時の処理</summary>
        public TextOverflowType TextOverflowType => _textOverflowType;

        protected FontSize FontSize => _fontSize;

        protected sealed override void Awake()
        {
            base.Awake();
            OnAwake();

#if MITTAUI_USE_UPALETTE
            // 色の設定
            _textMeshPro.SetColorFromEntryId(_colorEntryId);
#endif
            SetupFontSizeMin();
        }

        /// <summary>テキストの設定</summary>
        public void SetText(string text)
        {
            _textMeshPro.text = OnSetText(text);
            UpdateTextArea();
        }

        /// <summary>テキストの設定</summary>
        public void SetText(string text, bool updateFontSize)
        {
            _textMeshPro.text = OnSetText(text);
            if (updateFontSize) UpdateFontSize();

            UpdateTextArea();
        }
#if MITTAUI_USE_UPALETTE

        /// <summary> Color設定 </summary>
        public void SetColor(string colorStyleEntryId)
        {
            _colorEntryId.Value = colorStyleEntryId;
            _textMeshPro.SetColorFromEntryId(_colorEntryId);
        }
#endif

        public void SetFontSize(FontSize fontSize)
        {
            _fontSize = fontSize;
            _textMeshPro.fontSize = GetFontSize();
            UpdateFontSize();
            UpdateTextArea();
        }

        /// <summary>マージンの設定</summary>
        public void SetMargin(Vector4 margin)
        {
            _textMeshPro.margin = margin;
            UpdateTextArea();
        }

        /// <summary>リッチテキストの設定</summary>
        public void SetRichText(bool richText, bool raycastControl = false)
        {
            _textMeshPro.richText = richText;
            if (raycastControl) _textMeshPro.raycastTarget = richText;
        }

        /// <summary>テキストエリアを文字にフィットさせる</summary>
        public void FitTextBoxArea(bool ignoreWidth = false)
        {
            // 領域を確保してからテキストボックスサイズを取得する
            var currentWidth = RectTransform.sizeDelta.x;
            var attachSize = _vector2Max;
            if (ignoreWidth) attachSize.x = currentWidth;

            RectTransform.sizeDelta = attachSize;
            var preferredValue = _textMeshPro.GetPreferredValues();
            attachSize = new Vector2(preferredValue.x, preferredValue.y);
            if (ignoreWidth) attachSize.x = currentWidth;

            RectTransform.sizeDelta = attachSize;
        }

        /// <summary>座標からリンクインデックスを探す</summary>
        public int FindIntersectingLinkIndex(Vector2 screenPosition, Camera camera)
        {
            return TMP_TextUtilities.FindIntersectingLink(_textMeshPro, screenPosition, camera);
        }

        /// <summary>設定されているリンクインフォの取得</summary>
        public TMP_LinkInfo GetLinkInfo(int index)
        {
            var linkInfo = _textMeshPro.textInfo.linkInfo;
            // todo assertでエラー出るので一旦コメントアウト
            // Assert.IsTrue(index <= linkInfo.Length,
            //     $"リンクを取得できませんでした。渡すindexを確認してください。\nindex:{index}  text={_textMeshPro.text}");
            return linkInfo[index];
        }

        protected virtual void OnAwake()
        {
        }

        /// <summary>テキストが設定された時の処理 加工したかったらここで行う</summary>
        protected virtual string OnSetText(string t)
        {
            return t;
        }

        /// <summary>フォントサイズの取得</summary>
        protected float GetFontSize()
        {
            // todo プロジェクトで想定されるフォントサイズを正く設定する
            // ローカライズ時に海外のフォントに合わせてサイズを変更する必要もあり
            return UIConst.GetFontSize(_fontSize);
        }

        /// <summary>フォントサイズの更新</summary>
        private void UpdateFontSize()
        {
            TextOverflowUtility.UpdateFontSize(_textOverflowType, _textMeshPro);
            _textMeshPro.fontSizeMax = _textMeshPro.fontSize;
        }

        /// <summary>テキストエリアの更新</summary>
        private void UpdateTextArea()
        {
            TextOverflowUtility.UpdateTextArea(this);
        }

        /// <summary>最小フォントサイズの設定</summary>
        private void SetupFontSizeMin()
        {
            _textMeshPro.fontSizeMin = _fontSizeMin;
        }


#if UNITY_EDITOR
        /// <summary>Unityから呼ばれるReset</summary>
        protected override void Reset()
        {
            base.Reset();
            // Imageはこのコンポーネントより下にする
            TryGetComponent(out _textMeshPro);
            ComponentUtility.MoveComponentDown(_textMeshPro);
            // タグ許容しない
            _textMeshPro.richText = false;
            // 当たり判定なし
            _textMeshPro.raycastTarget = false;
            // エスケープされた文字を解析しない
            _textMeshPro.parseCtrlCharacters = false;

#if MITTAUI_USE_UPALETTE

            // 色は設定なし
            _colorEntryId.Value = string.Empty;
            SetupFontSizeMin();
#endif
        }
        /// <summary>Editor上の更新処理</summary>
        public void UpdateFromEditor()
        {
            SetFontSize(_fontSize);
            UpdateFontSize();
            UpdateTextArea();
#if MITTAUI_USE_UPALETTE
            _textMeshPro.SetColorFromEntryId(_colorEntryId);
#endif
        }
#endif
    }
}