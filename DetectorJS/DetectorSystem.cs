using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace TerraJS.DetectorJS
{
    public class DetectorSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            Detector.FlushMainThreadActions();
        }

        public override void OnWorldUnload()
        {
            Detector.Cancel();
        }
    }
}
