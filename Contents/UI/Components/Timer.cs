using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Xna.Framework;

namespace TerraJS.Contents.UI.Components
{
    public class Timer : Component
    {
        public Timer(int timerCount = 10, EventHandler<GameTime> onUpdate = null)
        {
            Timers = new float[timerCount];

            OnUpdate = onUpdate;
        }

        public float[] Timers;


        public EventHandler<GameTime> OnUpdate;

        public override void LeftClick(object sender, int pressTime, Vector2 mouseStart) { }

        public override void RightClick(object sender, int pressTime, Vector2 mouseStart) { }

        public override void Update(GameTime gameTime)
        {
            OnUpdate?.Invoke(this, gameTime);
        }

        public float this[int index]
        {
            get => Timers[index];
            set => Timers[index] = value;
        }
    }
}
