using System.Reflection;
using UnityEditor;

namespace MittaUI.Editor
{
    [InitializeOnLoad]
    public static class RemoveMenuItem
    {
        static RemoveMenuItem()
        {
            EditorApplication.delayCall += Remove;

            void Remove()
            {
                var methodInfo = typeof(Menu).GetMethod("RemoveMenuItem", BindingFlags.NonPublic | BindingFlags.Static);

                RemoveMenuItem("GameObject/UI/Legacy/Text");
                RemoveMenuItem("GameObject/UI/Legacy/Button");
                RemoveMenuItem("GameObject/UI/Legacy/Dropdown");
                RemoveMenuItem("GameObject/UI/Legacy/Input Field");
                RemoveMenuItem("GameObject/UI/Text");
                RemoveMenuItem("GameObject/UI/Text - TextMeshPro");
                RemoveMenuItem("GameObject/UI/Dropdown - TextMeshPro");
                RemoveMenuItem("GameObject/UI/Input Field - TextMeshPro");
                RemoveMenuItem("GameObject/UI/Button - TextMeshPro");
                RemoveMenuItem("GameObject/UI/Image");
                RemoveMenuItem("GameObject/UI/Raw Image");
                RemoveMenuItem("GameObject/UI/Toggle");
                RemoveMenuItem("GameObject/UI/Slider");
                RemoveMenuItem("GameObject/UI/Scrollbar");
                RemoveMenuItem("GameObject/UI/Scroll View");
                RemoveMenuItem("GameObject/UI/Panel");

                void RemoveMenuItem(string name)
                {
                    methodInfo.Invoke(null, new object[] { name });
                }
            }
        }
    }
}