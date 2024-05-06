#if MITTAUI_USE_R3

using System;
using R3;
using UnityEngine;

namespace MittaUI.Runtime.Extension
{
    public static class GamObjectExtensions
    {
        public static IDisposable SetActiveSelfSource(this GameObject self, Observable<bool> source,
            bool invert = false)
        {
            return source
                .Subscribe(x =>
                {
                    x = invert ? !x : x;
                    self.SetActive(x);
                })
                .AddTo(self);
        }
    }
}

#endif