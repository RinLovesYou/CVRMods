using System;
using System.Diagnostics;
using System.IO;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Newton.Engine;
using HarmonyLib;
using MelonLoader;
using static ProtonMediaControlFix.MediaControlFixMod;

namespace ProtonMediaControlFix
{
    public class MediaControlFixMod : MelonMod
    {
        public static MelonLogger.Instance Log;

        public static MelonPreferences_Category MyPreferenceCategory = 
            MelonPreferences.CreateCategory("LinuxMediaControlFix");

        public static MelonPreferences_Entry<int> PlayerCtlPort =
            MyPreferenceCategory.CreateEntry("Playerctl_Port", 8080);

        public override void OnApplicationStart()
        {
            Log = LoggerInstance;
            Log.Msg("Connecting to playerctl socket");
            try
            {
                Playerctl.Connect();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return;
            }
            Log.Msg("Success!");
        }
    }

    [HarmonyPatch(typeof(MediaControls), "MediaActionPlay")]
    internal class PlayPatch
    {
        public static void Prefix() => Playerctl.PlayPause();
    }

    [HarmonyPatch(typeof(MediaControls), "MediaActionPrev")]
    internal class PreviousPatch
    {
        public static void Prefix() => Playerctl.Previous();
    }
    
    [HarmonyPatch(typeof(MediaControls), "MediaActionFwd")]
    internal class SkipPatch
    {
        public static void Prefix() => Playerctl.Skip();
    }
}