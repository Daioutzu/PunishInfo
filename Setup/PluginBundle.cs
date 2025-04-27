using BepInEx;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PunishInfo.Setup
{
    internal static class PluginBundle
    {
        public static Dictionary<string, GameObject> prefabs { get; private set; } = new Dictionary<string, GameObject>();
        public static Dictionary<string, Sprite> sprites { get; private set; } = new Dictionary<string, Sprite>();

        public static bool Load(BaseUnityPlugin plugin, string bundleName)
        {
            DirectoryInfo bundleDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(plugin.Info.Location), "Bundles"));
            string bundlePath = Path.Combine(bundleDirectory.FullName, bundleName);

            var assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle == null)
                return false;

            LoadPrefabs(assetBundle);
            LoadSprites(assetBundle);
            return true;
        }

        public static bool Load(BaseUnityPlugin plugin, string prefabBundle, string uibunble)
        {
            DirectoryInfo bundleDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(plugin.Info.Location), "Bundles"));

            string bundlePath = Path.Combine(bundleDirectory.FullName, uibunble);
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle == null)
                return false;

            LoadSprites(assetBundle);

            bundlePath = Path.Combine(bundleDirectory.FullName, prefabBundle);
            assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle == null)
                return false;

            LoadPrefabs(assetBundle);
            return true;
        }

        private static void LoadPrefabs(AssetBundle bundle)
        {
            var gameobjects = bundle.LoadAllAssets<GameObject>();
            foreach (var item in gameobjects)
            {
                prefabs.Add(item.name, item);
            }
        }

        private static void LoadSprites(AssetBundle bundle)
        {
            Sprite[] spritesLoc = bundle.LoadAllAssets<Sprite>();
            foreach (Sprite item in spritesLoc)
            {
                sprites.Add(item.name, item);
            }
        }
    }
}