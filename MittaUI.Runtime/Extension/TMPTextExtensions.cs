#if MITTAUI_USE_R3
using System;
using R3;
using TMPro;

namespace MittaUI.Runtime.Extension
{
    public static class TMPTextExtensions
    {
        public static IDisposable SetTextSource(this TMP_Text self, Observable<string> source)
        {
            return source
                .Subscribe(x =>  self.SetText(x))
                .AddTo(self);
        }

        public static IDisposable SetTextSource(this TMP_Text self, Observable<int> source,
            Func<int, string> converter = null)
        {
            return source
                .Subscribe(x =>
                {
                    var text = converter == null ? x.ToString() : converter(x);
                    self.SetText(text);
                })
                .AddTo(self);
        }
    }
}
#endif