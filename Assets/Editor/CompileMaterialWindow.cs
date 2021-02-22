﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.UI;
public class CompileNoteWindow : EditorWindow
{

    private GorillaCosmetics.Data.Descriptors.GorillaMaterialDescriptor[] notes;
    [MenuItem("Window/Material Exporter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CompileNoteWindow), false, "Material Exporter", false);
    }
    public Vector2 scrollPosition = Vector2.zero;

    private void OnFocus()
    {
        notes = GameObject.FindObjectsOfType<GorillaCosmetics.Data.Descriptors.GorillaMaterialDescriptor>();
    }
    void OnGUI()
    {
        var window = EditorWindow.GetWindow(typeof(CompileNoteWindow), false, "Material Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (GorillaCosmetics.Data.Descriptors.GorillaMaterialDescriptor note in notes)
        {
            if (note != null)
            {

                ScrollSpace += (16 + 17 + 17 + 20 + 20);

            }
        }
        float currentWindowWidth = EditorGUIUtility.currentViewWidth;
        float windowWidthIncludingScrollbar = currentWindowWidth;
        if (window.position.size.y >= ScrollSpace)
        {
            windowWidthIncludingScrollbar += 30;
        }
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, window.position.size.y), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - 20, ScrollSpace), false, false);

        //GUILayout.ScrollViewScope
        GUILayout.Label("Notes", EditorStyles.boldLabel, GUILayout.Height(16));
        GUILayout.Space(20);

        foreach (GorillaCosmetics.Data.Descriptors.GorillaMaterialDescriptor note in notes)
        {
            if (note != null)
            {
                GUILayout.Label("GameObject : " + note.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                note.AuthorName = EditorGUILayout.TextField("Author name", note.AuthorName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                note.MaterialName = EditorGUILayout.TextField("Material name", note.MaterialName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + note.MaterialName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = note.gameObject;
                    if (noteObject != null && note != null)
                    {
                        string path = EditorUtility.SaveFilePanel("Save material file", "", note.MaterialName + ".gmat", "gmat");
                        Debug.Log(path == "");

                        if (path != "")
                        {
                            string fileName = Path.GetFileName(path);
                            string folderPath = Path.GetDirectoryName(path);

                            Selection.activeObject = noteObject;
                            EditorUtility.SetDirty(note);
                            EditorSceneManager.MarkSceneDirty(noteObject.scene);
                            EditorSceneManager.SaveScene(noteObject.scene);
                            PrefabUtility.CreatePrefab("Assets/_Material.prefab", Selection.activeObject as GameObject);
                            AssetBundleBuild assetBundleBuild = default(AssetBundleBuild);
                            assetBundleBuild.assetNames = new string[] {
                            "Assets/_Material.prefab"
                        };

                            assetBundleBuild.assetBundleName = fileName;

                            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

                            BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, EditorUserBuildSettings.activeBuildTarget);
                            EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
                            EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
                            AssetDatabase.DeleteAsset("Assets/_Material.prefab");
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                            File.Move(Application.temporaryCachePath + "/" + fileName, path);
                            AssetDatabase.Refresh();
                            EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Exportation Failed!", "Path is invalid.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Exportation Failed!", "Note GameObject is missing.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }

}