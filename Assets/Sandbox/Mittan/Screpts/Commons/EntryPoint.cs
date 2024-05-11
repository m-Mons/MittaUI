using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Samples.Mittan.Commons
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private PageContainer pageContainer;
        [SerializeField] private ModalContainer defaultModalContainer;
        [SerializeField] private ModalContainer notificationModalContainer;

        private static bool Initialized { get; set; }

        private async UniTaskVoid Awake()
        {
            ScreenUtility.DefaultModal = defaultModalContainer;
            ScreenUtility.NotificationModal = notificationModalContainer;
            ScreenUtility.Page = pageContainer;

            //await ScreenUtility.PushModal(ScreenUtility.Modals.Label[ScreenUtility.Modals.Names.Loading]);
            //await ScreenUtility.PushPage(ScreenUtility.Pages.Label[ScreenUtility.Pages.Names.Title]);
            await ScreenUtility.PopModal();
            Initialized = true;
            Debug.Log("Game Start by awake");
        }
    }
}