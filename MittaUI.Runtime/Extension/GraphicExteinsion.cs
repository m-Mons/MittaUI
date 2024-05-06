using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if MITTAUI_USE_UPALETTE
using uPalette.Runtime.Core;
#endif

namespace MittaUI.Runtime.Extension
{
    /// <summary>Graphicの拡張</summary>
    public static class GraphicExtensions
    {

#if MITTAUI_USE_UPALETTE
        /// <summary>uPaletteのEntryIdから色を設定する</summary>
        public static void SetColorFromEntryId(this Graphic graphic, ColorEntryId colorEntryId)
        {
            if (PaletteStore.Instance == null)
            {
                Debug.LogError($"Project内に{nameof(PaletteStore.Instance.ColorPalette)}が存在しないので色を設定できません");
                return;
            }

            // 色を設定する
            if (PaletteStore.Instance.ColorPalette.TryGetActiveValue(colorEntryId.Value, out var colorProperty))
                graphic.color = colorProperty.Value;
        }
#endif
    }
}