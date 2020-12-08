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
        public static bool TrackingErrorDetectionEnabled { get; internal set; }
        public static float DriftThreshold { get; internal set; }
        public static float PlayerHeight { get; internal set; } // set this manually bc game is big stupid and wont let me do it automatically... it isnt really important anyway

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
            TrackingErrorDetectionEnabled = PluginConfig.Instance.trackingErrorDetectionEnabled;
            DriftThreshold = PluginConfig.Instance.driftThreshold;
            PlayerHeight = PluginConfig.Instance.playerHeight;
        }
        internal static void Save()
        {
            Plugin.Log.Debug("Saving Configuration...");
            PluginConfig.Instance.modEnabled = ModEnabled;
            PluginConfig.Instance.frameDropDetectionEnabled = FrameDropDetectionEnabled;
            PluginConfig.Instance.waitThenActive = WaitThenActive;
            PluginConfig.Instance.frameThreshold = FrameThreshold;
            PluginConfig.Instance.trackingErrorDetectionEnabled = TrackingErrorDetectionEnabled;
            PluginConfig.Instance.driftThreshold = DriftThreshold;
            PluginConfig.Instance.playerHeight = PlayerHeight;
        }
    }
}
