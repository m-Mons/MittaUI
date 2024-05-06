using System.Collections.Generic;
using UnityEngine;

namespace MittaUI.Runtime.Constant
{
    /// <summary>
    ///     UI周りの定数を定義したクラス
    /// </summary>
    public static class UIConst
    {
        /// <summary> フォントサイズの定義された辞書 </summary>
        private static readonly Dictionary<FontSize, int> Dictionary = new()
        {
            { FontSize.Px20, 20 },
            { FontSize.Px24, 24 },
            { FontSize.Px32, 32 },
            { FontSize.Px40, 40 },
            { FontSize.Px48, 48 },
            { FontSize.Px60, 60 },
            { FontSize.Px80, 80 },
            { FontSize.Px100, 100 },
            { FontSize.Px120, 120 },
            { FontSize.Px160, 160 },
            { FontSize.Px200, 200 }
        };

        /// TODO constはScriptableObjectにして設定を外出ししたい
        /// <summary>
        ///     デフォルトの解像度
        ///     CanvasReferenceResolutionなどで使う
        /// </summary>
        public static Vector2 DefaultResolution => new(1080f, 1920f);

        /// <summary>
        ///     フォントサイズの取得
        /// </summary>
        public static int GetFontSize(FontSize fontSize)
        {
            if (Dictionary.TryGetValue(fontSize, out var size)) return size;

            Debug.LogError($"定義されていないサイズが指定されました fontSize:{fontSize}");
            // とりあえず40返しておく
            return 40;
        }
    }
}