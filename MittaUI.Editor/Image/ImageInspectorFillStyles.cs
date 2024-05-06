using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MittaUI.Framework.Editor
{
    /// <summary>
    ///     ImageInspectorのFill用Style
    /// </summary>
    public static class ImageInspectorFillStyles
    {
        public static readonly IReadOnlyDictionary<Image.FillMethod, GUIContent[]> FillStyleDictionary =
            new Dictionary<Image.FillMethod, GUIContent[]>
            {
                {
                    Image.FillMethod.Horizontal, new[]
                    {
                        EditorGUIUtility.TrTextContent("Left"),
                        EditorGUIUtility.TrTextContent("Right")
                    }
                },
                {
                    Image.FillMethod.Vertical, new[]
                    {
                        EditorGUIUtility.TrTextContent("Bottom"),
                        EditorGUIUtility.TrTextContent("Top")
                    }
                },
                {
                    Image.FillMethod.Radial90, new[]
                    {
                        EditorGUIUtility.TrTextContent("Bottom-Left"),
                        EditorGUIUtility.TrTextContent("Top-Left"),
                        EditorGUIUtility.TrTextContent("Top-Right"),
                        EditorGUIUtility.TrTextContent("Bottom-Right")
                    }
                },
                {
                    Image.FillMethod.Radial180, new[]
                    {
                        EditorGUIUtility.TrTextContent("Bottom"),
                        EditorGUIUtility.TrTextContent("Left"),
                        EditorGUIUtility.TrTextContent("Top"),
                        EditorGUIUtility.TrTextContent("Right")
                    }
                },
                {
                    Image.FillMethod.Radial360, new[]
                    {
                        EditorGUIUtility.TrTextContent("Bottom"),
                        EditorGUIUtility.TrTextContent("Right"),
                        EditorGUIUtility.TrTextContent("Top"),
                        EditorGUIUtility.TrTextContent("Left")
                    }
                }
            };
    }
}