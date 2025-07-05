using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Assets
{
    public abstract class AssetManager<T>
    {
        public virtual void Load()
        {
            GetType().GetFields()
                .Where(t => t.FieldType == typeof(Dictionary<string, T>))
                .ToList().ForEach(t =>
                {
                    var a = t.GetValue(this) as Dictionary<string, T>;
                    LoadOne(t.Name, a);
                });
        }

        public virtual void Clear()
        {
            GetType().GetFields()
                .Where(t => t.FieldType == typeof(Dictionary<string, T>))
                .ToList().ForEach(t =>
                {
                    t.SetValue(this, (Dictionary<string, T>)[]);
                });
        }

        public virtual void Reload()
        {
            Clear();

            Load();
        }

        public abstract void LoadOne(string dir, Dictionary<string, T> dictronary);
    }
}
