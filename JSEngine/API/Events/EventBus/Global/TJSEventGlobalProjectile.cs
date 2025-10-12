using TerraJS.Contents.Attributes;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Events.EventBus.Global
{
    [HideToJS]
    public class TJSEventGlobalProjectile : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            return TJSEngine.GlobalAPI.Event.Projectile.PreAIEvent?.Invoke(projectile) ?? true;
        }

        public override void AI(Projectile projectile)
        {
            TJSEngine.GlobalAPI.Event.Projectile.AIEvent?.Invoke(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            TJSEngine.GlobalAPI.Event.Projectile.PostAIEvent?.Invoke(projectile);
        }
    }
}
