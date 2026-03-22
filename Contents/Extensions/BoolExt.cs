namespace TerraJS.Contents.Extensions
{
    public static class BoolExt
    {
        public static void Reverse(this ref bool @bool)
        {
            @bool = !@bool;
        }
    }
}
