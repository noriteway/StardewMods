using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BetterBeachBridgeRepair
{
    public sealed class ModConfig
    {
        public bool Disable { get; set; } = false;
        public bool HideGMCM { get; set; } = false;
        public int Price { get; set; } = 50;
        public int BridgeTileX { get; set; } = 58;
        public int BridgeTileY { get; set; } = 13;
    }
}
