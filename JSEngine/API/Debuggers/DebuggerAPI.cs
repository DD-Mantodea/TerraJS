using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Interop;
using System.Dynamic;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Debuggers
{
    public class DebuggerAPI : BaseAPI
    {
        public void TestObject(object o)
        {
            Main.NewText(o.GetType());

            Main.NewText(o.ToString());
        }

        public void TestJsValue(ScriptFunction o)
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
