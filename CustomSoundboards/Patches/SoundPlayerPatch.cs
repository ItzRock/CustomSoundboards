using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CustomSoundboards.Patches {
    [HarmonyPatch(typeof(SoundPlayerItem))]
    public class SoundPlayerPatch {

        public static List<SFX_Instance> sounds = new();

        [HarmonyPostfix]
        [HarmonyPatch("ConfigItem")]
        public static void ConfigItem(ItemInstanceData data, SoundPlayerItem __instance) {
            sounds = new();
            CustomSoundBoards.Instance.StartCoroutine(PatchAudio(__instance, data));
        }

        public static IEnumerator PatchAudio(SoundPlayerItem instance, ItemInstanceData data) {
            string[] urls = CustomSoundBoards.SoundboardURLs.Value.Split(",");
            for (int i = 0; i < urls.Length; i++) {
                string url = urls[i];
                CustomSoundBoards.Logger.LogInfo($"Applying {url}.");
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN)) {
                    yield return www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.ConnectionError) {
                        CustomSoundBoards.Logger.LogError(www.error);
                    } else {
                        AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                        SFX_Instance sound = ScriptableObject.CreateInstance<SFX_Instance>();
                        string[] split = url.Split("/");
                        sound.name = split[split.Length-1];
                        sound.settings = new() {
                            pitch_Variation = 0f,
                            volume_Variation = 0f
                        };

                        sound.clips = new[] { myClip };
                        sounds.Add(sound);
                    }
                }
            }
            CustomSoundBoards.Logger.LogInfo($"Patched Sounds {sounds.Count} | {instance.sounds.Length}.");
            for (int i = 0; i < instance.sounds.Length; i++) {
                sounds.Add(instance.sounds[i]);
            }

            instance.sounds = sounds.ToArray();

            if (data.TryGetEntry<IntRangeEntry>(out IntRangeEntry entry)) {
                data.RemoveDataEntry(entry);
            }

            instance.selectionEntry = new IntRangeEntry {
                selectedValue = 0,
                maxValue = instance.sounds.Length
            };
            data.AddDataEntry(instance.selectionEntry);
        }
    }
}
