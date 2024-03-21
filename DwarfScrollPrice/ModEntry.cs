using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using System.Linq;
using static StardewValley.Minigames.CraneGame;

namespace DwarfScrollPrice
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig? Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            // Debugging Events
            //helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if(e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    if(Config != null)
                    {
                        data["96"].Price = Config.price1;
                        data["97"].Price = Config.price2;
                        data["98"].Price = Config.price3;
                        data["99"].Price = Config.price4;
                    }
                });
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if(configMenu is null)
            {
                //Monitor.Log($"Config Menu is null.", LogLevel.Debug);
                return;
            }

            if(Config == null)
            {
                //Monitor.Log($"Config is null.", LogLevel.Debug);
                return;
            }

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => OnSave()
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price1,
                setValue: value => Config.price1 = (int)value,
                name: () => "Dwarf Scroll I",
                tooltip: () => "Sets the sell price of dwarf scroll I.",
                min: 1,
                fieldId: "ds1"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price2,
                setValue: value => Config.price2 = (int)value,
                name: () => "Dwarf Scroll II",
                tooltip: () => "Sets the sell price of dwarf scroll II.",
                min: 1,
                fieldId: "ds2"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price3,
                setValue: value => Config.price3 = (int)value,
                name: () => "Dwarf Scroll III",
                tooltip: () => "Sets the sell price of dwarf scroll III.",
                min: 1,
                fieldId: "ds3"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price4,
                setValue: value => Config.price4 = (int)value,
                name: () => "Dwarf Scroll IV",
                tooltip: () => "Sets the sell price of dwarf scroll IV.",
                min: 1,
                fieldId: "ds4"
            );

            /*configMenu.OnFieldChanged(
                mod: ModManifest,
                onChange: OnChange
            );*/
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {

        }

        /*void OnChange(string fieldId, object value)
        {
            Monitor.Log(value.ToString(), LogLevel.Debug);
        }*/

        void OnSave()
        {
            if(Config != null)
            {
                Helper.WriteConfig(Config);

                Game1.objectData["96"].Price = Config.price1;
                Game1.objectData["97"].Price = Config.price2;
                Game1.objectData["98"].Price = Config.price3;
                Game1.objectData["99"].Price = Config.price4;
            }

            if(Game1.player.Items.ContainsId("(O)96"))
            {
                Monitor.Log("Dwarf Scroll I detected in inventory...", LogLevel.Debug);
                var scrollItems = Game1.player.Items.GetById("(O)96");

                for(int i = 0; i < scrollItems.Count(); i++)
                {
                    Item newScrollItem = ItemRegistry.Create("(O)96");
                    Item oldScrollItem = scrollItems.ElementAt(i);
                    newScrollItem.Stack = oldScrollItem.Stack;
                    //GetOneCopyFrom(Item source)

                    //Game1.player.Items.RemoveButKeepEmptySlot(oldScrollItem);
                    Game1.player.Items.Remove(oldScrollItem);
                    Game1.player.Items.Add(newScrollItem);
                }
            }

            Monitor.Log("Values Saved", LogLevel.Debug);
        }
    }
}
