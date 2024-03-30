using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq.Expressions;
using System.Reflection.Emit;

//using SpaceShared;
//using SpaceShared.UI;

//using GenericModConfigMenu;

namespace MoreSettings
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;

        private bool muted = false;

        private float musicVol;
        private float ambientVol;
        private float footstepVol;
        private float soundVol;

        private int muteButtonIndex = 16;

        private int ticksToWait;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if(e.NewMenu is GameMenu menu)
            {
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                string buttonLabel = "Mute";
                if(muted) buttonLabel = "Unmute";
                page.options.Insert(muteButtonIndex, new OptionsButton(buttonLabel, () => ToggleMute(page)));
                
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            
        }

        private void ToggleMute(OptionsPage page)
        {
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

                // Set UI changes to be made after GameMenu refresh takes effect
                ticksToWait = 1;
                Helper.Events.GameLoop.UpdateTicked += MuteUI;
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
            RefreshGameMenu(GameMenu.optionsTab, page.currentItemIndex, false);

            muted = !muted;
        }

        private void RefreshGameMenu(int startingTab, int startingIndex, bool playOpeningSound)
        {
            Game1.activeClickableMenu = null;
            Game1.activeClickableMenu = new GameMenu(startingTab, startingIndex, playOpeningSound);
        }

        private void MuteUI(object? sender, UpdateTickedEventArgs e)
        {
            if(ticksToWait-- < 0)
            {
                GameMenu gameMenu = (GameMenu)Game1.activeClickableMenu;
                OptionsPage newPage = (OptionsPage)gameMenu.pages[GameMenu.optionsTab];

                foreach(var option in newPage.options)
                {
                    if(option.label.Contains("Volume")) option.greyedOut = true;
                }

                Helper.Events.GameLoop.UpdateTicked -= MuteUI;
            }
        }

        private void ToggleFullscreen()
        {
            Game1.toggleFullscreen();
        }
    }
}
