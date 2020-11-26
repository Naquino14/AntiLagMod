using AntiLagMod.settings.utilities;
using IPA.Config;
using IPA.Config.Stores;
using System;

namespace AntiLagMod.settings
{
    public class Configuration
    {
        public static bool ModEnabled { get; internal set; }
        public static float FrameThreshold { get; internal set; }
        public static bool FrameDropDetectionEnabled { get; internal set; }
        public static float WaitThenActive { get; internal set; }
        public static bool DriftDetectionEnabled { get; internal set; }
        public static float DriftThreshold { get; internal set; }

        internal static void Init(Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
        }
        internal static void Load()
        {
            Plugin.Log.Debug("Loading Configuration...");
            ModEnabled = PluginConfig.Instance.modEnabled;
            FrameDropDetectionEnabled = PluginConfig.Instance.frameDropDetectionEnabled;
            WaitThenActive = PluginConfig.Instance.waitThenActive;
            FrameThreshold = PluginConfig.Instance.frameThreshold;
            DriftDetectionEnabled = PluginConfig.Instance.driftDetectionEnabled;
            DriftThreshold = PluginConfig.Instance.driftThreshold;
        }
        internal static void Save()
        {
            Plugin.Log.Debug("Saving Configuration...");
            PluginConfig.Instance.modEnabled = ModEnabled;
            PluginConfig.Instance.frameDropDetectionEnabled = FrameDropDetectionEnabled;
            PluginConfig.Instance.waitThenActive = WaitThenActive;
            PluginConfig.Instance.frameThreshold = FrameThreshold;
            PluginConfig.Instance.driftDetectionEnabled = DriftDetectionEnabled;
            PluginConfig.Instance.driftThreshold = DriftThreshold;
        }
    }
}
