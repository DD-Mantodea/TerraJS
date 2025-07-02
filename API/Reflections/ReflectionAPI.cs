using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Reflections
{
    public class ReflectionAPI : BaseAPI
    {
        public object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);

        internal override void Unload() { }
    }
}
