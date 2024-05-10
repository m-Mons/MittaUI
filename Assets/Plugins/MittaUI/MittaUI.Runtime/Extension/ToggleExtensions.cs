#if MITTAUI_USE_R3

using System;
using R3;
using UnityEngine.UI;

namespace MittaUI.Runtime.Extension
{
    public static class ToggleExtensions
    {
        public static IDisposable SetOnValueChangedDestination(this Toggle self, Action<bool> onValueChanged)
        {
            return self.onValueChanged
                .AsObservable()
                .Subscribe(onValueChanged.Invoke)
                .AddTo(self);
        }
    }
}

#endif