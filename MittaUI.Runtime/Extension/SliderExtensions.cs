#if MITTAUI_USE_R3

using System;
using System.Threading;
using R3;
using UnityEngine.UI;

namespace MittaUI.Runtime.Extension
{
    public static class SliderExtensions
    {
        public static IDisposable SetOnValueChangedDestination(this Slider self, Action<float> onValueChanged)
        {
            return self.onValueChanged
                .AsObservable()
                .Subscribe(onValueChanged.Invoke)
                .AddTo(self);
        }

        public static Observable<float> AsObservable(this Slider self, CancellationToken ct = default)
        {
            return self.onValueChanged.AsObservable(ct);
        }
    }
}

#endif