using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Managers
{
    public abstract class AssetManager<T>
    {
        internal Dictionary<string, Dictionary<string, T>> LoadTargets = new();

        public event EventHandler PreLoad;

        public event EventHandler PostLoad;

        public virtual void Load()
        {
            GetType().GetFields()
                .Where(t => t.FieldType == typeof(Dictionary<string, T>))
                .ToList().ForEach(t =>
                {
                    var a = t.GetValue(this);
                    LoadTargets.Add(t.Name, t.GetValue(this) as Dictionary<string, T>);
                });
            PreLoad?.Invoke(LoadTargets, new EventArgs());
            foreach (var target in LoadTargets)
            {
                LoadOne(target.Key, target.Value);
                PostLoad?.Invoke(target, new EventArgs());
            }
        }

        public abstract void LoadOne(string dir, Dictionary<string, T> dictronary);

        public abstract bool ExistAsset(string path);
    }
}
