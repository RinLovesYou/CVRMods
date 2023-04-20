using MelonLoader;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

[assembly: MelonInfo(typeof(ModLoadSeparator.ModLoadSeparator), "ModLoadSeparator", "1.0.1", "RinLovesYou")]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonAdditionalCredits("Hordini")]

namespace ModLoadSeparator
{
    public class ModLoadSeparator : MelonPlugin
    {
        public static MelonLogger.Instance MyLogger => new("ModLoadSeparator", Color.Cyan);

        internal static bool IsInVR = false;

        internal static string ModsPath = $"{Directory.GetCurrentDirectory()}/Mods";

        public override void OnApplicationEarlyStart()
        {
            if (!Directory.Exists("Mods/Desktop"))
                Directory.CreateDirectory("Mods/Desktop");

            if (!Directory.Exists("Mods/VR"))
                Directory.CreateDirectory("Mods/VR");

            IsInVR = Environment.CommandLine.Contains("vr");

            string vrOrDesktop = IsInVR ? "VR" : "Desktop";

            ModsPath = $"{ModsPath}/{vrOrDesktop}";

            MyLogger.Msg($"Loading {vrOrDesktop} Mods!");

            ScanMods();
        }

        public void ScanMods()
        {
            LoadModsFolder(ModsPath);
        }

        public void LoadModsFolder(string path)
        {
            string[] dir = Directory.GetFiles(ModsPath);

            foreach (string file in dir)
            {
                if (!file.EndsWith(".dll"))
                    continue;

                MelonAssembly melonAssembly = MelonAssembly.LoadMelonAssembly(file);
                if (melonAssembly != null)
                {
                    RegisterSorted(melonAssembly.LoadedMelons);
                }

                string modName = file.Split('\\').Last().Replace(".dll", "");
            }

        }
    }
}