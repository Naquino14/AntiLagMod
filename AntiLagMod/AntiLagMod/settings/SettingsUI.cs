using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace AntiLagMod.settings
{
    internal class SettingsUI
    {
        public static ALMFlowCoordinator flowCoordinator;
        public static bool isCreated = false;

        public static void CreateMenu()
        {
            if (!isCreated)
            {
                MenuButton menuButton = new MenuButton("Anti Lag Mod", "Adjust Anti Lag Mod Settings Here!", MenuButtonPressed, true);
                MenuButtons.instance.RegisterButton(menuButton);
                isCreated = true;
            }

            
        }

        public static void ShowALMFlow()
        {
            if (flowCoordinator == null)
            {
                flowCoordinator = BeatSaberUI.CreateFlowCoordinator<ALMFlowCoordinator>();
            }
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }

        private static void MenuButtonPressed() => ShowALMFlow();
    }
}
