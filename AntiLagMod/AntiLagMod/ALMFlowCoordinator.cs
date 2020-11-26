using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiLagMod.settings.views;
using BeatSaberMarkupLanguage;
using HMUI;

namespace AntiLagMod
{
    class ALMFlowCoordinator : FlowCoordinator
    {
        private SettingsView settingsView;

        void Awake()
        {
            if (!settingsView)
            {
                settingsView = BeatSaberUI.CreateViewController<SettingsView>();
            }
        }
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {

            try
            {
                if (firstActivation)
                {
                    SetTitle("Anti Lag Mod");
                    showBackButton = true;
                    ProvideInitialViewControllers(settingsView); 
                    
                }
            } catch (Exception exception)
            {
                Plugin.Log.Error(exception);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            //base.BackButtonWasPressed(topViewController);
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }


    }
}
