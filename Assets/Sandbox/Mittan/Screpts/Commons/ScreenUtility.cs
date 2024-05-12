using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace Samples.Mittan.Commons
{
    public static class ScreenUtility
    {
        private static PageContainer pageContainer;
        public static PageContainer Page
        {
            get => pageContainer;
            set => pageContainer = value;
        }
        private static SheetContainer sheetContainer;
        public static SheetContainer Sheet
        {
            get => sheetContainer;
            set => sheetContainer = value;
        }
        private static ModalContainer defaultModalContainer;
        public static ModalContainer DefaultModal
        {
            get => defaultModalContainer;
            set => defaultModalContainer = value;
        }
        private static ModalContainer notificationModalContainer;
        public static ModalContainer NotificationModal
        {
            get => notificationModalContainer;
            set => notificationModalContainer = value;
        }
    
        #region PageKeys
        public static class Pages
        {
            public enum Names
            {
                Default,
            }
        
            public static IReadOnlyDictionary <Names, string> Label = new Dictionary<Names, string>
            {
                {Names.Default, "UiPanels/Pages/Default"},
            }; 
        }
        #endregion

        #region SheetKeys
        public static class Sheets
        {
            public enum Names
            {
                Default,
            }
        
            public static IReadOnlyDictionary <Names, string> Label = new Dictionary<Names, string>
            {
                {Names.Default, "UiPanels/Sheets/Default"},
            }; 
        }
        #endregion

        #region ModalKeys
        public static class Modals
        {
            public enum Names
            {
                Default,
            }
        
            public static IReadOnlyDictionary <Names, string> Label = new Dictionary<Names, string>
            {
                {Names.Default, "UiPanels/Modals/Default"},
            }; 
        }
        #endregion

        public static async UniTask PopModal(bool isAnimation = true, CancellationToken ct = default)
        {
            await defaultModalContainer.Pop(isAnimation);
        }
    
        public static async UniTask PushModal(string id, bool isAnimation = true, CancellationToken ct = default)
        {
            await defaultModalContainer.Push(id, isAnimation);
        }
    
        public static async UniTask PopPage(bool isAnimation = true, CancellationToken ct = default)
        {
            await pageContainer.Pop(isAnimation);
        }
    
        public static async UniTask PushPage(string id, bool isAnimation = true, CancellationToken ct = default)
        {
            await pageContainer.Push(id, isAnimation);
        }
    }
}