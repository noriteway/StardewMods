using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MoreSettings
{
    public sealed class ModConfig
    {
        public SButton MuteKey { get; set; } = StardewModdingAPI.SButton.K;
        public int MuteButtonIndex { get; set; } = 16;
        public SButton FullscreenKey { get; set; } = StardewModdingAPI.SButton.F11;
        public bool UseWindowedBorderless { get; set; } = true;
    }
}
