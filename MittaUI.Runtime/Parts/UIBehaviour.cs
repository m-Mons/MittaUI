using UnityEngine;

namespace MittaUI.Runtime.Parts
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBehaviour : UnityEngine.EventSystems.UIBehaviour
    {
        /// <summary>
        ///     RectTransformのキャッシュ
        /// </summary>
        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (IsDestroyed()) return null;

                if (_rectTransform == null) _rectTransform = transform as RectTransform;

                return _rectTransform;
            }
        }

        protected override void OnDestroy()
        {
            _rectTransform = null;
            base.OnDestroy();
        }
    }
}