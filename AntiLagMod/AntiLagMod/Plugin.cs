using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using System.Configuration;
using AntiLagMod;
using AntiLagMod.settings;
using AntiLagMod.settings.utilities;

namespace AntiLagMod
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        
        

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Config config)
        {
            Instance = this;
            Log = logger;
            AntiLagMod.settings.Configuration.Init(config);
            //Log.Info("AntiLagMod initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnALMMODApplicationStart");
            new GameObject("AntiLagModController").AddComponent<AntiLagModController>();
            Load();

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }

        public static void SaveConfig() // prob doesnt actually do anything useful but its here
        {
            settings.Configuration.Save();
        }

        private void Load() // called when the application starts
        {
            SettingsUI.CreateMenu();
            settings.Configuration.Load();
            
        }

    }
}
