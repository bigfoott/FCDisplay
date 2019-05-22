using CustomUI.Settings;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace FCDisplay
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        public static BS_Utils.Utilities.Config Config = new BS_Utils.Utilities.Config("FCDisplay");
        public static string[] effects = { "Fade", "FlyOut", "Flicker", "Shrink" };

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name != "GameCore" || !Config.GetBool("FCDisplay", "Enabled", true, true)) return;
            new GameObject("FCDisplay").AddComponent<DisplayObject>();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name != "MenuCore") return;

            var menu = SettingsUI.CreateSubMenu("FCDisplay");

            float[] effectVals = new float[effects.Length];
            for (int i = 0; i < effects.Length; i++) effectVals[i] = i;

            var enabled = menu.AddBool("Enabled");
            enabled.GetValue += delegate { return Config.GetBool("FCDisplay", "Enabled", true, true); };
            enabled.SetValue += delegate (bool value) { Config.SetBool("FCDisplay", "Enabled", value); };
            enabled.EnabledText = "Enabled";
            enabled.DisabledText = "Disabled";

            var effect = menu.AddList("Miss Effect", effectVals);
            effect.GetValue += delegate { return Config.GetInt("FCDisplay", "MissEffect", 1, true); };
            effect.SetValue += delegate (float value) { Config.SetInt("FCDisplay", "MissEffect", (int)value); };
            effect.FormatValue += delegate (float value) { return effects[(int)value]; };

            var vanilla = menu.AddBool("Vanilla Display (Combo Border)");
            vanilla.GetValue += delegate { return Config.GetBool("FCDisplay", "VanillaEnabled", false, true); };
            vanilla.SetValue += delegate (bool value) { Config.SetBool("FCDisplay", "VanillaEnabled", value); };
            vanilla.EnabledText = "Visible";
            vanilla.DisabledText = "Hidden";
        }

        public void OnApplicationStart() { }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnSceneUnloaded(Scene scene) { }
    }
}
