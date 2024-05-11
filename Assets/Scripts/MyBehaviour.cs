using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts
{
    /// <summary>
    /// MonoBehaviourじゃなくてこっちを継承してSampleの実装を行う
    /// </summary>
    public abstract class MyBehaviour : MonoBehaviour, IDisposable , IAsyncDisposable
    {
        /// <summary>
        /// 初期化 絶対に書く
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public abstract UniTask InitializeAsync(CancellationToken ct = default);
        
        /// <summary>
        /// 破棄処理
        /// イベントの購読をやめたり、オブジェクトをプールに戻したり
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// 非同期破棄処理
        /// </summary>
        public virtual async ValueTask DisposeAsync()
        {
            await UniTask.CompletedTask;
        }


        #region ignore UnityEventFunc

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void OnDestroy()
        {
        }

        private void OnEnable()
        {
        }

        #endregion
    }
}