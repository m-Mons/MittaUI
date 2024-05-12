using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using Debug = UnityEngine.Debug;

namespace Samples.Mittan.Commons
{
    enum TestShowViewType
    {
        sheet,
        page,
        modal,
    }

    public class EntryPointTestMode : MonoBehaviour
    {
        [SerializeField] private PageContainer pageContainer;
        [SerializeField] private ModalContainer defaultModalContainer;
        [SerializeField] private ModalContainer notificationModalContainer;

    
        [SerializeField] private ScreenUtility.Pages.Names pageName;
        [SerializeField] private ScreenUtility.Sheets.Names sheetName;
        [SerializeField] private ScreenUtility.Modals.Names modalName;
        [SerializeField] private TestShowViewType type;
    
        private static bool Initialized { get; set; }
    
        private async UniTaskVoid Awake()
        {
            ScreenUtility.DefaultModal = defaultModalContainer;
            ScreenUtility.NotificationModal = notificationModalContainer;
            ScreenUtility.Page = pageContainer;

            switch (type)
            {
                case TestShowViewType.sheet:
                    //await ScreenUtility.PushPage(ScreenUtility.Pages.Label[ScreenUtility.Pages.Names.Title]);
                    break;
                case TestShowViewType.page:
                    await ScreenUtility.PushPage(ScreenUtility.Pages.Label[pageName]);
                    break;
                case TestShowViewType.modal:
                    await ScreenUtility.PushModal(ScreenUtility.Modals.Label[modalName]);
                    break;
            }

            Initialized = true;
            Debug.Log("Game Start by awake");
        }
    }
}
