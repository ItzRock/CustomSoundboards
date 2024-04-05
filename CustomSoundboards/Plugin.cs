using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace CustomSoundboards {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class CustomSoundBoards : BaseUnityPlugin {
        internal new static ManualLogSource Logger { get; private set; }
        internal static ConfigEntry<string> SoundboardURLs { get; private set; }

        public static CustomSoundBoards Instance { get; private set; }
        private void Awake() {
            Instance = this;
            Logger = base.Logger;

            string DefaultSounds = "https://www.myinstants.com/media/sounds/metal-pipe-clang.mp3,https://www.myinstants.com/media/sounds/vine-boom.mp3,https://www.myinstants.com/media/sounds/dry-fart.mp3";

            SoundboardURLs = Config.Bind("General", "Sounds", DefaultSounds,
                "A list of URL's seperated by commas where the sounds will be pull from.");

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony harmony = new Harmony("CustomSoundBoards");
            harmony.PatchAll();
        }

    }
}
