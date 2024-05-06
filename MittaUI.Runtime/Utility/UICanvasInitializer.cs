using MittaUI.Runtime.Constant;
using UnityEngine;
using UnityEngine.UI;

namespace MittaUI.Runtime
{
    /// <summary>
    ///     プロジェクト共通でCanvas初期化用
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UICanvasInitializer : MonoBehaviour
    {
        private CanvasScaler _scaler;

        private void Reset()
        {
            Execute();
        }

        private void OnEnable()
        {
            Execute();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Execute();
        }
#endif

        private void Execute()
        {
            _scaler ??= GetComponent<CanvasScaler>();

            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            _scaler.matchWidthOrHeight = 0f;
            _scaler.referenceResolution = UIConst.DefaultResolution;
        }
    }
}