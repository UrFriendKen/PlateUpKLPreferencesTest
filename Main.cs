using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenMods;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenKLPreferencesTest
{
    public class Main : BaseMod, IModSystem
    {
        public const string MOD_GUID = "com.example.KLPrefTest";
        public const string MOD_NAME = "KLPrefTest";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "My Name";
        public const string MOD_GAMEVERSION = ">=1.1.3";
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif

        // Create a static PreferenceManager to handle your settings
        internal static PreferenceManager PrefManager;

        // Create cached references to your preference
        internal static PreferenceInt intPreference;
        internal static PreferenceBool boolPreference;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // Initialisation
            PrefManager = new PreferenceManager(Main.MOD_GUID);
            intPreference = PrefManager.RegisterPreference(new PreferenceInt("TestInt", defaultValue: 1));
            boolPreference = PrefManager.RegisterPreference(new PreferenceBool("TestBool", defaultValue: true));
            
            // Loading previous user settings (If it exists)
            PrefManager.Load();

            // Register Menus with KL
            SetupMenus();
        }

        private void SetupMenus()
        {
            // Replace ALL instances of TestMenu with your menu type


            //Setting Up For Main Menu (Remove if not required)
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(TestMenu<MainMenuAction>), new TestMenu<MainMenuAction>(args.Container, args.Module_list));
            };
            ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(TestMenu<MainMenuAction>), typeof(MainMenuAction));



            //Setting Up For Pause Menu
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(TestMenu<PauseMenuAction>), new TestMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(TestMenu<PauseMenuAction>), typeof(PauseMenuAction));
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }


    public class TestMenu<T> : KLMenu<T>
    {
        public TestMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        Option<int> intOption;
        Option<bool> boolOption;

        public override void Setup(int player_id)
        {
            Redraw();
        }

        private void Redraw()
        {
            ModuleList.Clear();

            AddLabel("Test Mod");

            AddProfileSelector(Main.MOD_GUID, delegate (string s) {
                Redraw();
            }, Main.PrefManager, true);

            AddLabel("Test Int");
            intOption = new Option<int>(
                new List<int>() { 1, 2, 3 },
                Main.intPreference.Get(),
                new List<string>() { "1", "2", "3" });
            intOption.OnChanged += (object _, int i) =>
            {
                Main.intPreference.Set(i);
                Main.PrefManager.Save();
            };
            Add<int>(intOption);

            AddLabel("Test Bool");
            boolOption = new Option<bool>(
                new List<bool>() { false, true },
                Main.boolPreference.Get(),
                new List<string>() { "false", "true" });
            boolOption.OnChanged += (object _, bool b) =>
            {
                Main.boolPreference.Set(b);
                Main.PrefManager.Save();
            };
            Add<bool>(boolOption);

            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }
    }
}
