using MittaUI.Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace MittaUI.Editor.Image
{
    [CustomEditor(typeof(Runtime.Image), true)]
    [CanEditMultipleObjects]
    public class ImageEditor : UnityEditor.Editor
    {
        private static readonly GUIContent FillOriginLabel = new("Fill Origin");

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            var targetImage = target as Runtime.Image;
            var unityImage = targetImage.GetComponent<UnityEngine.UI.Image>();
            var unityImageSo = new SerializedObject(unityImage);

            var entryIdProperty = serializedObject.FindProperty("_entryId");
            EditorGUILayout.PropertyField(entryIdProperty);

            var spriteProperty = unityImageSo.FindProperty("m_Sprite");
            EditorGUILayout.PropertyField(spriteProperty);

            var materialProperty = unityImageSo.FindProperty("m_Material");
            EditorGUILayout.PropertyField(materialProperty);

            var spriteTypeProperty = unityImageSo.FindProperty("m_Type");
            EditorGUILayout.PropertyField(spriteTypeProperty);

            var imageType = unityImage.type;
            var isFill = imageType == UnityEngine.UI.Image.Type.Filled;
            if (isFill)
            {
                // Fillの時だけFillにまつわるInspector表示
                ++EditorGUI.indentLevel;

                var fillMethodProperty = unityImageSo.FindProperty("m_FillMethod");
                EditorGUILayout.PropertyField(fillMethodProperty);

                var shapeRect = EditorGUILayout.GetControlRect(true);
                var style = ImageInspectorFillStyles.FillStyleDictionary[unityImage.fillMethod];

                var fillOriginProperty = unityImageSo.FindProperty("m_FillOrigin");

                fillOriginProperty.intValue =
                    EditorGUI.Popup(shapeRect, FillOriginLabel, fillOriginProperty.intValue, style);

                var fillAmountProperty = unityImageSo.FindProperty("m_FillAmount");
                EditorGUILayout.PropertyField(fillAmountProperty);

                var fillClockwiseProperty = unityImageSo.FindProperty("m_FillClockwise");
                EditorGUILayout.PropertyField(fillClockwiseProperty);
                --EditorGUI.indentLevel;
            }

            var raycastTargetProperty = unityImageSo.FindProperty("m_RaycastTarget");
            EditorGUILayout.PropertyField(raycastTargetProperty);

            var sourceImageProperty = unityImageSo.FindProperty("m_Sprite");
            var showSetNativeSizeButton =
                imageType is UnityEngine.UI.Image.Type.Simple or UnityEngine.UI.Image.Type.Filled &&
                sourceImageProperty.objectReferenceValue is not null;

            if (showSetNativeSizeButton && GUILayout.Button("Set Native Size", EditorStyles.miniButton))
            {
                Undo.RecordObject(unityImage.rectTransform, "Set Native Size");
                unityImage.SetNativeSize();
                EditorUtility.SetDirty(unityImage);
            }

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck()) unityImageSo.ApplyModifiedProperties();
        }
    }
}