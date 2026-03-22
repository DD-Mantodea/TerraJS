using System;
using Terraria;

namespace TerraJS.JSEngine.API.Events.SubEvents
{
    public class ProjectileEventAPI : BaseEventAPI
    {
        public Func<Projectile, bool> PreAIEvent;

        public Action<Projectile> AIEvent;

        public Action<Projectile> PostAIEvent;

        public void PreAI(Func<Projectile, bool> @delegate) => PreAIEvent += @delegate;

        public void AI(Action<Projectile> @delegate) => AIEvent += @delegate;

        public void PostAI(Action<Projectile> @delegate) => PostAIEvent += @delegate;
    }
}
