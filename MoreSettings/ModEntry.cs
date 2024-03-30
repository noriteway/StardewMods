using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq.Expressions;

//using SpaceShared;
//using SpaceShared.UI;

//using GenericModConfigMenu;

namespace MoreSettings
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;
        private float musicVol;
        private float ambientVol;
        private float footstepVol;
        private float soundVol;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if(e.NewMenu is GameMenu menu)
            {
                //Monitor.Log("NewMenu is a GameMenu", LogLevel.Debug);
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                page.options.Insert(16, new OptionsButton("Mute", () => Mute(page)));
                //foreach(var option in page.options) Monitor.Log(option.label, LogLevel.Debug);
                //page.options.Add(new OptionsButton(I18n.Button_ModOptions(), () => this.OpenListMenu()));
                
            }
        }

        private void Mute(OptionsPage page)
        {
            Monitor.Log("Muting all audio", LogLevel.Debug);

            // Store existing volume levels
            musicVol = Game1.options.musicVolumeLevel;
            soundVol = Game1.options.soundVolumeLevel;
            ambientVol = Game1.options.ambientVolumeLevel;
            footstepVol = Game1.options.footstepVolumeLevel;

            // Set current levels to zero
            //Game1.options.musicVolumeLevel = 0;
            //Game1.options.ambientVolumeLevel = 0;
            //Game1.options.footstepVolumeLevel = 0;
            //Game1.options.soundVolumeLevel = 0;
            Game1.options.changeSliderOption(1, 0);
            Game1.options.changeSliderOption(2, 0);
            Game1.options.changeSliderOption(20, 0);
            Game1.options.changeSliderOption(21, 0);


            //Game1.activeClickableMenu = null;
            //Game1.activeClickableMenu = new GameMenu(GameMenu.optionsTab, 16, false);
            RefreshGameMenu(GameMenu.optionsTab, page.currentItemIndex, false);
            //also check exit w/o sound and initialize?

            // Refresh UI
            //Game1.menu
            // not assign and unassign Game1.activeClickableMenu...
            // not Game1.options.setSliderToProperValue(new OptionsSlider("Ooga", 1))...;
            //page.draw();
            //Monitor.Log(Game1.currentGameTime.ToString(), LogLevel.Debug);
            //Game1.activeClickableMenu.update(Game1.currentGameTime);
        }

        private void Unmute()
        {
            // Restore with stored levels
            Game1.options.changeSliderOption(1, (int)(musicVol * 100));
            Game1.options.changeSliderOption(2, (int)(soundVol * 100));
            Game1.options.changeSliderOption(20, (int)(ambientVol * 100));
            Game1.options.changeSliderOption(21, (int)(footstepVol * 100));
        }

        private void ToggleFullscreen()
        {
            Game1.toggleFullscreen();
        }

        private void RefreshGameMenu(int startingTab, int startingIndex, bool playOpeningSound)
        {
            Game1.activeClickableMenu = null;
            Game1.activeClickableMenu = new GameMenu(startingTab, startingIndex, playOpeningSound);

            // It does seem to destroy the old object, but keep an eye out
            //Monitor.Log(Game1.nextClickableMenu.Count.ToString(), LogLevel.Debug);
            //Monitor.Log(Game1.endOfNightMenus.Count.ToString(), LogLevel.Debug);
            //Monitor.Log(Game1.onScreenMenus.Count.ToString(), LogLevel.Debug);
        }
    }
}
