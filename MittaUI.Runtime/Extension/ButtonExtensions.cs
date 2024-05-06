#if MITTAUI_USE_R3

using System;
using MittaUI.Runtime.Parts;
using R3;
using UnityEngine.UI;

namespace MittaUI.Runtime.Extension
{
    public static class ButtonExtensions
    {
        /// <summary>
        ///     <see cref="BaseButtonWithLongTapGesture.ClickedObservable" />
        /// </summary>
        public static IDisposable SetOnClickDestination(this BaseButtonWithLongTapGesture self, Action onClick)
        {
            return self.ClickedObservable
                .Subscribe(x => onClick.Invoke())
                .AddTo(self);
        }

        public static IDisposable SetOnClickDestination(this Button self, Action onClick)
        {
            return self.onClick
                .AsObservable()
                .Subscribe(x => onClick.Invoke())
                .AddTo(self);
        }
    }
}

#endif