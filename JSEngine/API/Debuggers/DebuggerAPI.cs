using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Runtime.Interop;
using TerraJS.API;
using Terraria;

namespace TerraJS.JSEngine.API.Debuggers
{
    public class DebuggerAPI : BaseAPI
    {
        public void TestObject(object o)
        {
            Main.NewText(o.GetType());

            Main.NewText(o.ToString());
        }

        public void TestTypeReference(TypeReference reference)
        {
            Main.NewText(reference.ReferenceType.Name);

            Main.NewText(reference.ToString());
        }

        internal override void Unload()
        {
            
        }
    }
}
