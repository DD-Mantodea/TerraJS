using Jint.Runtime.Interop;
using System;
using System.Reflection;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Utils;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    [HideToJS]
    public class JintHook : ModSystem
    {
        public override void Load()
        {
            var canChangeType = TypeUtils.InteropHelper.GetMethod("CanChangeType", BindingFlags.NonPublic | BindingFlags.Static);

            MonoModHooks.Add(canChangeType, CanChangeType);

            var convert = TypeUtils.DefaultTypeConverter.GetMethod("TryConvert", BindingFlags.NonPublic | BindingFlags.Instance);

            MonoModHooks.Add(convert, TryConvert);
        }

        private bool CanChangeType(Func<object, Type, bool> orig, object obj, Type type)
        {
            using (new Logging.QuietExceptionHandle())
                return orig(obj, type);
        }

        private delegate bool TryConvertOrig(DefaultTypeConverter self, object? value, Type type, IFormatProvider provider, bool propagateException, out object? converted, out string? problemMessage);

        private bool TryConvert(TryConvertOrig orig, DefaultTypeConverter self, object? value, Type type, IFormatProvider provider, bool propagateException, out object? converted, out string? problemMessage)
        {
            using (new Logging.QuietExceptionHandle())
                return orig(self, value, type, provider, propagateException, out converted, out problemMessage);
        }
    }
}
