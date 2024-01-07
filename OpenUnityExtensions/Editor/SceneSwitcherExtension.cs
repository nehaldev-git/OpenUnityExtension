using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;

namespace OpenUnityExtensions
{
    [Overlay(typeof(SceneView), "Scene Switch Overlay", defaultDockPosition = DockPosition.Top, defaultDockZone = DockZone.TopToolbar, defaultDockIndex = 0)]
    [Icon("Assets/OpenUnityExtensions/Texture/unity.png")]
    public class SceneSwitcherExtension : ToolbarOverlay
    {

        SceneSwitcherExtension() : base(
             SceneListDropDown.id,
             SceneLocator.id
            )
        { }
    }



    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneListDropDown : EditorToolbarDropdown
    {
        private List<string> sceneNames;
        public const string id = "SceneSwitch";
        static string dropChoice = null;

        public SceneListDropDown()
        {
            RefreshSceneList();
            text = SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length);
            tooltip = "Current Scene";
            clicked += ShowDropdown;
        }

        void ShowDropdown()
        {
            var menu = new GenericMenu();
            foreach (var name in sceneNames)
            {

                menu.AddItem(new GUIContent(name), dropChoice == name, () => { LoadSelectedScene(name); text = SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length); });
            }

            menu.ShowAsContext();
            RefreshSceneList();
        }


        private void RefreshSceneList()
        {
            sceneNames = new List<string>();
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (string sceneGuid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(sceneName);
            }
        }
        private void LoadSelectedScene(string name)
        {

            string scenePath = AssetDatabase.FindAssets(sceneNames.Find(n => n == name) + " t:Scene")[0];
            EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(scenePath));

        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneLocator : EditorToolbarButton
    {


        public const string id = "SceneLocator";
        public SceneLocator()
        {

            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/OpenUnityExtensions/Texture/folder.png");
            tooltip = "Locate Current Scene";
            clicked += OnClick;
        }       

        void OnClick()
        {
            string scenePath = AssetDatabase.FindAssets(SceneManager.GetActiveScene().name)[0];
            Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(scenePath));
            Selection.activeObject = sceneAsset;
            EditorGUIUtility.PingObject(sceneAsset);
        }

    }
}

