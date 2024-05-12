using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static UnityEngine.Mathf;

namespace MittaUI.Editor.Utility
{
    public class MyMenuItems
    {
        [MenuItem("GameObject/MittaUI", false, 0)]
        static void Execute() => InstantiatePrefabWindow.Open(
#if MITTAUI_RELEASE
            "Packages/com.mitta.mitta-ui/MittaUI.Runtime/Prefabs"
#else
            "Assets/Plugins/MittaUI/MittaUI.Runtime/Prefabs"
#endif
        );
    }

    /// <summary>
    /// 指定したディレクトリ配下を対象にした Prefab 一覧を表示し、選択した Prefab を配置するメニューウィンドウ
    /// </summary>
    public sealed class InstantiatePrefabWindow : ScriptableObject, ISearchWindowProvider
    {
        /// <summary>
        /// 指定したディレクトリパスとアセットパスから得られる情報群
        /// </summary>
        class Data
        {
            const string Separator = "/";
            public string AssetPath { get; }
            public string FileName { get; }
            public string[] DirectoryNames { get; }

            public Data(string assetPath, string basePath)
            {
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var directory = Path.GetDirectoryName(assetPath.Remove(0, basePath.Length + Separator.Length));
                AssetPath = assetPath;
                FileName = fileName;
                DirectoryNames = directory.Split(new[] { Separator }, StringSplitOptions.None);
            }
        }

        class Constants
        {
            public static readonly Texture IconPrefab = EditorGUIUtility.IconContent("GameObject Icon").image;
        }

        string basePath;

        /// <summary>
        /// メニューを開く
        /// </summary>
        /// <param name="basePath">指定したディレクトリ配下を対象にメニューを作成する</param>
        public static void Open(string basePath = "Assets")
        {
            var hierarchyWindow = Resources.FindObjectsOfTypeAll<EditorWindow>()
                .First(window => window.GetType().Name == "SceneHierarchyWindow");
            var provider = CreateInstance<InstantiatePrefabWindow>();
            provider.basePath = basePath;
            var position = new Vector2(hierarchyWindow.position.x + hierarchyWindow.position.width / 2,
                hierarchyWindow.position.y + 56);
            SearchWindow.Open(new SearchWindowContext(position, hierarchyWindow.position.width), provider);
        }

        /// <summary>
        /// データ構造の作成
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var assetPaths = AssetDatabase.FindAssets("t:prefab", new[] { basePath })
                .Select(AssetDatabase.GUIDToAssetPath);
            var data = assetPaths.Select(assetPath => new Data(assetPath, basePath));
            var entries = DataToEntries(data);
            return entries.ToList();
        }

        /// <summary>
        /// 選択時の処理
        /// </summary>
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            DestroyImmediate(this);
            var assetPath = entry.userData as string;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.transform.SetParent(Selection.activeTransform, false);
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
            Selection.activeObject = instance;
            return true;
        }

        /// <summary>
        /// パスリストからメニューのデータ構造を作成
        /// </summary>
        IEnumerable<SearchTreeEntry> DataToEntries(IEnumerable<Data> dataList)
        {
            yield return new SearchTreeGroupEntry(new GUIContent("Select Prefab"));
            var data2 = default(Data); // 1 = current, 2 = prev
            foreach (var data1 in dataList)
            {
                var directoryNames1 = data1?.DirectoryNames;
                var directoryNames2 = data2?.DirectoryNames;
                var level = 1;
                var max = Max
                (
                    directoryNames1?.Length ?? 0,
                    directoryNames2?.Length ?? 0
                );
                for (var i = 0; i < max; i++)
                {
                    var name1 = directoryNames1?.ElementAtOrDefault(i);
                    var name2 = directoryNames2?.ElementAtOrDefault(i);
                    if (string.IsNullOrEmpty(name1))
                    {
                        break;
                    }

                    if (string.IsNullOrEmpty(name2))
                    {
                        yield return new SearchTreeGroupEntry(new GUIContent(name1)) { level = level };
                    }
                    else if (name1 != name2)
                    {
                        yield return new SearchTreeGroupEntry(new GUIContent(name1)) { level = level };
                        i = max;
                    }

                    level++;
                }

                yield return new SearchTreeEntry(new GUIContent(data1.FileName, Constants.IconPrefab))
                    { level = level, userData = data1.AssetPath };
                data2 = data1;
            }
        }
    }
}