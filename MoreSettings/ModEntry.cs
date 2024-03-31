using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

//using SpaceShared;
//using SpaceShared.UI;

using GenericModConfigMenu;

namespace MoreSettings
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;

        private bool muted = false;
        private bool fullscreen = false;

        private float musicVol;
        private float ambientVol;
        private float footstepVol;
        private float soundVol;

        private int ticksToWait;

        //private GameMenu? currentGameMenu;

        private bool needUpdate = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if(e.NewMenu is GameMenu menu)
            {
                //currentGameMenu = menu;

                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                string buttonLabel = "Mute";
                if(muted)
                {
                    buttonLabel = "Unmute";
                    foreach(var option in page.options)
                    {
                        if(option.label.Contains("Volume")) option.greyedOut = true;
                    }
                }
                
                if(Config != null) page.options.Insert(Config.MuteButtonIndex, new OptionsButton(buttonLabel, () => ToggleMute()));
                else page.options.Insert(16, new OptionsButton(buttonLabel, () => ToggleMute()));
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get MCM API(if installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            // Exit if no config
            if(configMenu == null) return;
            if(Config == null) return;

            // Register mod for MCM
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // Add UI for MCM
            /*configMenu.AddKeybind(
                mod: ModManifest,
                getValue: () => Config.MuteKey
            );*/

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MuteButtonIndex,
                setValue: value => Config.MuteButtonIndex = (int)value,
                name: () => "Mute Button Index",
                tooltip: () => "Determines where the mute button is placed in the options menu. The default places it just below the audio header.",
                min: 0
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.UseWindowedBorderless,
                setValue: value => Config.UseWindowedBorderless = (bool)value,
                name: () => "Use Windowed Borderless",
                tooltip: () => "Use windowed borderless mode instead of fullscreen mode. This will make the delay to switch less, and doesn't look different, but windowed modes are less performant."
            );
        }

        private void ToggleMute()
        {
            Monitor.Log("Toggling mute after load.", LogLevel.Debug);

            if(!muted)
            {
                // Store existing volume levels
                musicVol = Game1.options.musicVolumeLevel;
                soundVol = Game1.options.soundVolumeLevel;
                ambientVol = Game1.options.ambientVolumeLevel;
                footstepVol = Game1.options.footstepVolumeLevel;

                // Set volume levels to zero
                Game1.options.changeSliderOption(1, 0);
                Game1.options.changeSliderOption(2, 0);
                Game1.options.changeSliderOption(20, 0);
                Game1.options.changeSliderOption(21, 0);

                // Set UI changes to be made
                //NOTE: Try onupdateticking?
                //ticksToWait = 1;
                //Helper.Events.GameLoop.UpdateTicked += MuteUI;
            }
            else
            {
                // Set to stored volume levels (value here is an int * 100 to represent float)
                Game1.options.changeSliderOption(1, (int)(musicVol * 100));
                Game1.options.changeSliderOption(2, (int)(soundVol * 100));
                Game1.options.changeSliderOption(20, (int)(ambientVol * 100));
                Game1.options.changeSliderOption(21, (int)(footstepVol * 100));
            }

            // Refresh GameMenu
            //Don't need to force update gamemenu if it isn't already open
            if(Game1.activeClickableMenu is GameMenu gameMenu)
            {
                OptionsPage optionsPage = (OptionsPage)gameMenu.pages[GameMenu.optionsTab];
                RefreshGameMenu(gameMenu.currentTab, optionsPage.currentItemIndex, false);
            }

            /*if(!muted)
            {
                //MuteUI();
                OptionsPage newPage = (OptionsPage)currentGameMenu.pages[GameMenu.optionsTab];
                foreach(var option in newPage.options)
                {
                    if(option.label.Contains("Volume")) option.greyedOut = true;
                }
            }*/

            muted = !muted;
        }

        private void ToggleMuteBeforeLoad()
        {
            Monitor.Log("Toggling mute before load.", LogLevel.Debug);
        }

        private void RefreshGameMenu(int startingTab, int startingIndex, bool playOpeningSound)
        {
            Game1.activeClickableMenu = null;
            //currentGameMenu = new GameMenu(startingTab, startingIndex, playOpeningSound);
            //Game1.activeClickableMenu = currentGameMenu;
            Game1.activeClickableMenu = new GameMenu(startingTab, startingIndex, playOpeningSound);
        }

        /*private void MuteUI(object? sender, UpdateTickedEventArgs e)
        {
            if(ticksToWait-- < 0)
            {
                if(currentGameMenu == null)
                {
                    Monitor.Log("Current game menu is null.", LogLevel.Error);
                    Helper.Events.GameLoop.UpdateTicked -= MuteUI;
                    return;
                }

                OptionsPage newPage = (OptionsPage)currentGameMenu.pages[GameMenu.optionsTab];

                foreach(var option in newPage.options)
                {
                    if(option.label.Contains("Volume")) option.greyedOut = true;
                }

                Helper.Events.GameLoop.UpdateTicked -= MuteUI;
            }
        }

        private void MuteUI()
        {
            if(currentGameMenu == null)
            {
                Monitor.Log("Current game menu is null.", LogLevel.Error);
                return;
            }

            OptionsPage newPage = (OptionsPage)currentGameMenu.pages[GameMenu.optionsTab];

            foreach(var option in newPage.options)
            {
                if(option.label.Contains("Volume")) option.greyedOut = true;
            }
        }*/

        public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if(Config == null) return;

            if(e.Button == Config.MuteKey)
            {
                Monitor.Log("Mute key pressed", LogLevel.Debug);
                //if(Game1.activeClickableMenu != null) Monitor.Log(Game1.activeClickableMenu.GetType().ToString(), LogLevel.Debug);
                //else Monitor.Log("null", LogLevel.Debug);
                if(Game1.activeClickableMenu is TitleMenu) ToggleMuteBeforeLoad();
                else ToggleMute();
            }

            if(e.Button == Config.FullscreenKey)
            {
                Monitor.Log("Fullscreen key pressed", LogLevel.Debug);
                ToggleFullscreen();
            }
        }

        public void ToggleFullscreen()
        {
            if(fullscreen)
            {
                if(Config != null && Config.UseWindowedBorderless) Game1.options.setWindowedOption("Windowed Borderless");
                else Game1.options.setWindowedOption("Fullscreen");
            }
            else Game1.options.setWindowedOption("Windowed");

            fullscreen = !fullscreen;
        }

        public void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            
        }
    }
}
