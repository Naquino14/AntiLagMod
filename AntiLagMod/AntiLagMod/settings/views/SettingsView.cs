using System;
using System.Collections.Generic;
using System.Linq;
using AntiLagMod;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Diagnostics;
using AntiLagMod.settings.views;


namespace AntiLagMod.settings.views
{
    [ViewDefinition("AntiLagMod.settings.views.SettingsView.bsml")]
    public class SettingsView : BSMLAutomaticViewController
    {


        //:flooshed:
        [UIValue("mod-enabled")]
        public bool modEnabled
        {
            get => Configuration.ModEnabled;
            set => Configuration.ModEnabled = value;
        }

        [UIValue("frame-drop-enabled")]public bool frameDropDetection
        {
            get => Configuration.FrameDropDetectionEnabled;
            set => Configuration.FrameDropDetectionEnabled = value;
        }

        [UIValue("frame-drop-threshold")]
        public float frameThreshold
        {
            get => Configuration.FrameThreshold;
            set => Configuration.FrameThreshold = value;
        }

        [UIValue("max-framerate")]
        private int maxFramerate = 144; //this is default for testing purposes

        [UIValue("wait-then-active")]
        public float waitThenActive
        {
            get => Configuration.WaitThenActive;
            set => Configuration.WaitThenActive = value;
        }

        [UIValue("drift-detection-enabled")]
        public bool driftDetection
        {
            get => Configuration.TrackingErrorDetectionEnabled;
            set => Configuration.TrackingErrorDetectionEnabled = value;
        }

        [UIAction("save")]
        private void SaveButtonPress()
        {
            SaveConfig();
            Plugin.Log.Debug("SaveButtonPress() fired");
        }

        [UIAction("more-info")]
        private void MoreInfoClicked()
        {
            Process.Start("https://github.com/Naquino14/Anti-Lag-Mod");
        }

        [UIValue("bb-min")]
        private float bbMin = 10f; // for testing purposes
        

        [UIValue("bb-max")]
        private float bbMax = 100f; // for testing purposes


        [UIValue("drift-threshold")]
        public float driftThreshold
        {
            get => Configuration.DriftThreshold;
            set => Configuration.DriftThreshold = value;
        }

        [UIAction("show-bb")]
        private void ShowBBPressed()
        {
            AntiLagModController.EnableBB();
            
        }

        [UIAction("initiate-trolling")]
        private void ButtonPress() // ima leave this here as an easter egg 
        {
            Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ&list=PLahKLy8pQdCM0SiXNn3EfGIXX19QGzUG3");
        }



        private void SaveConfig() // prob doesnt actually do anything useful but its here anyway
        {
            Plugin.SaveConfig();
            AntiLagModController.Refresh();
        }

    }
}
