using MittaUI.Runtime.Parts;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace MittaUI.Framework.Editor
{
    [CustomEditor(typeof(BaseText), true)]
    [CanEditMultipleObjects]
    public class BaseTextEditor : UnityEditor.Editor
    {
        private static readonly GUIContent _alignmentLabel = new("Alignment", "テキストの水平方向と垂直方向の整列");
        private BaseText _baseText;
        private SerializedProperty _fontAssetProp;
        private SerializedProperty _horizontalAlignmentProp;
        private TextMeshProUGUI _textMeshProUGUI;
        private SerializedObject _textMeshProUGUISerializedObject;
        private SerializedProperty _textProp;
        private SerializedProperty _verticalAlignmentProp;

        private void OnEnable()
        {
            _baseText = (BaseText)target;
            _baseText.TryGetComponent(out _textMeshProUGUI);
            _textMeshProUGUISerializedObject = new SerializedObject(_textMeshProUGUI);
            _textProp = _textMeshProUGUISerializedObject.FindProperty("m_text");
            _fontAssetProp = _textMeshProUGUISerializedObject.FindProperty("m_fontAsset");
            _horizontalAlignmentProp = _textMeshProUGUISerializedObject.FindProperty("m_HorizontalAlignment");
            _verticalAlignmentProp = _textMeshProUGUISerializedObject.FindProperty("m_VerticalAlignment");
            _textMeshProUGUI.richText = true;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_textProp, GUIContent.none);
            EditorGUILayout.PropertyField(_fontAssetProp);
            base.OnInspectorGUI();
            DrawAlignment();
            if (EditorGUI.EndChangeCheck())
            {
                _textMeshProUGUISerializedObject.ApplyModifiedProperties();
                _baseText.UpdateFromEditor();
            }
        }

        private void DrawAlignment()
        {
            // TEXT ALIGNMENT
            EditorGUI.BeginChangeCheck();
            var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.currentViewWidth > 504 ? 20 : 40 + 3);
            EditorGUI.BeginProperty(rect, _alignmentLabel, _horizontalAlignmentProp);
            EditorGUI.BeginProperty(rect, _alignmentLabel, _verticalAlignmentProp);
            EditorGUI.PrefixLabel(rect, _alignmentLabel);
            rect.x += EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(rect, _horizontalAlignmentProp, GUIContent.none);
            EditorGUI.PropertyField(rect, _verticalAlignmentProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) _textMeshProUGUISerializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
            EditorGUI.EndProperty();
            EditorGUILayout.Space();
        }
    }
}