using UnityEngine;
using UnityEngine.EventSystems;

namespace MittaUI.Runtime
{
    public class UIEventUtility
    {
        /// <summary>
        /// イベントを貫通させて親に伝播する
        /// </summary>
        public static void PenetrateParentHandler<T>(Transform transform,
            PointerEventData eventData,
            ExecuteEvents.EventFunction<T> eventFunc)
            where T : IEventSystemHandler
        {
            var handler = ExecuteEvents.GetEventHandler<T>(transform.parent.gameObject);
            ExecuteEvents.Execute(handler, eventData, eventFunc);
        }
    }
}