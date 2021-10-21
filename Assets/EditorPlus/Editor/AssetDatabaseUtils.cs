using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// Helper class to easily fetch the project's assets, based on their types and labels.
    /// </summary>
    public static class AssetDatabaseUtils
    {
        private static AssetType[] GuidsToArray<AssetType>(string[] guids) where AssetType : Object {

            List<AssetType> output = new List<AssetType>();
            for (var i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var element = AssetDatabase.LoadAssetAtPath<AssetType>(path);
                if (element) {
                    output.Add(element);
                }
            }
            return output.ToArray();
        }
    
        /// <summary>
        /// Fetches all the assets of type AssetType in the project.
        /// The function will search in the whole project or, if specified, only
        /// in the folders in the searchInFolders array.
        /// </summary>
        /// <param name="searchInFolders">Optional folder filter. If set,
        /// the function will only search in those folders.</param>
        /// <typeparam name="AssetType">The type of the assets to search for.</typeparam>
        /// <returns>All the assets of type AssetType.</returns>
        /// <seealso cref="GetSingle&lt;AssetType&gt;"/>
        /// <seealso cref="GetAllByLabel&lt;AssetType&gt;"/>
        /// <seealso cref="GetAll"/>
        [NotNull]
        public static AssetType[] GetAll<AssetType>(string[] searchInFolders = null) where AssetType : Object {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(AssetType).Name, searchInFolders);
            return GuidsToArray<AssetType>(guids);
        }
    
        /// <summary>
        /// Fetches all the assets of type AssetType in the project, with the specified labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <typeparam name="AssetType">The type of the assets to search for.</typeparam>
        /// <returns>All the assets of type AssetType with the specified labels.</returns>
        /// <seealso cref="GetAll&lt;AssetType&gt;"/>
        /// <seealso cref="GetAll"/>
        [NotNull]
        public static AssetType[] GetAllByLabel<AssetType>(params string[] labels) where AssetType : Object {
            string filter = string.Join(" ", labels.Select(label => $"l:{label}"));
            string[] guids = AssetDatabase.FindAssets(filter, null);
            return GuidsToArray<AssetType>(guids);
        }

        /// <summary>
        /// Fetches the supposedly single asset of type AssetType. If multiple
        /// assets are found, a warning will be logged.<br />
        /// The function will search in the whole project or, if specified, only
        /// in the folders in the searchInFolders array.
        /// </summary>
        /// <param name="searchInFolders">Optional folder filter. If set,
        /// the function will only search in those folders.</param>
        /// <typeparam name="AssetType">The type of the asset to search for.</typeparam>
        /// <returns>One asset of type AssetType, or null if none exist in the project.</returns>
        /// <seealso cref="GetAll&lt;AssetType&gt;"/>
        /// <seealso cref="GetSingleByLabel&lt;AssetType&gt;"/>
        /// <seealso cref="GetSingle"/>
        [CanBeNull]
        public static AssetType GetSingle<AssetType>(string[] searchInFolders = null) where AssetType : Object {
            AssetType[] all = GetAll<AssetType>(searchInFolders);
            return AssertSingle(all);
        }

        /// <summary>
        /// Fetches the supposedly single asset of type AssetType, with the given labels.
        /// If multiple assets are found, a warning will be logged.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <typeparam name="AssetType">The type of the asset to search for.</typeparam>
        /// <returns>One asset of type AssetType with the given labels, or null if none exist in the project.</returns>
        /// <seealso cref="GetSingle&lt;AssetType&gt;"/>
        /// <seealso cref="GetSingle"/>
        [CanBeNull]
        public static AssetType GetSingleByLabel<AssetType>(params string[] labels) where AssetType : Object {
            AssetType[] all = GetAllByLabel<AssetType>(labels);
            return AssertSingle(all);
        }

        /// <summary>
        /// Fetches the supposedly single asset of type AssetType.
        /// If multiple assets are found, a warning will be logged.<br /><br />
        /// If no asset of the given type exist however, it will be created at newAssetPath.
        /// </summary>
        /// <param name="newAssetPath">The location to create the asset at, if it does not already exist.
        /// Must contain the asset name and extension.</param>
        /// <typeparam name="AssetType">The type of the asset to search for.</typeparam>
        /// <returns>One asset of type AssetType.</returns>
        /// <seealso cref="GetSingle&lt;AssetType&gt;"/>
        /// <seealso cref="GetSingle"/>
        [NotNull] 
        public static AssetType GetSingleOrCreate<AssetType>(string newAssetPath) where AssetType : ScriptableObject {
            AssetType asset = GetSingle<AssetType>();
            if (asset is null) {
                asset = ScriptableObject.CreateInstance<AssetType>();
            
                MakeSureDirectoryExist(Path.GetDirectoryName(newAssetPath));
                AssetDatabase.CreateAsset(asset, newAssetPath);
            }
        
            return asset;
        }

        /// <summary>
        /// Fetches all the assets of type assetType in the project.
        /// The function will search in the whole project or, if specified, only
        /// in the folders in the searchInFolders array.
        /// </summary>
        /// <param name="assetType">The type of the assets to search for.</param>
        /// <param name="searchInFolders">Optional folder filter. If set,
        /// the function will only search in those folders.</param>
        /// <returns>All the assets of type assetType.</returns>
        /// <seealso cref="GetAll&lt;AssetType&gt;"/>
        /// <seealso cref="GetSingle"/>
        [NotNull]
        public static ScriptableObject[] GetAll(Type assetType, string[] searchInFolders = null) {
            string[] guids = AssetDatabase.FindAssets("t:" + assetType.Name, searchInFolders);
            return GuidsToArray<ScriptableObject>(guids);
        }

        /// <summary>
        /// Fetches the supposedly single asset of type assetType. If multiple
        /// assets are found, a warning will be logged.<br />
        /// The function will search in the whole project or, if specified, only
        /// in the folders in the searchInFolders array.
        /// </summary>
        /// <param name="assetType">The type of the asset to search for.</param>
        /// <param name="searchInFolders">Optional folder filter. If set,
        /// the function will only search in those folders.</param>
        /// <returns>One asset of type assetType, or null if none exist in the project.</returns>
        /// <seealso cref="GetSingle&lt;AssetType&gt;"/>
        /// <seealso cref="GetAll"/>
        [CanBeNull]
        public static ScriptableObject GetSingle(Type assetType, string[] searchInFolders = null) {
            ScriptableObject[] all = GetAll(assetType, searchInFolders);
            return AssertSingle(all);
        }

        private static AssetType AssertSingle<AssetType>(AssetType[] values) where AssetType : Object {
            if (values.Length > 1) {
                Debug.LogWarning("Called AssetDatabaseUtils.GetSingle with a class that has multiple assets instances.");
            }
            return values.Length == 0 ? null : values[0];
        }
        
        private static void MakeSureDirectoryExist(string directoryPath) {

            if (Directory.Exists(directoryPath)) {
                return;
            }

            string parent = Path.GetDirectoryName(directoryPath);
            MakeSureDirectoryExist(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(directoryPath));
        }
    }
}
