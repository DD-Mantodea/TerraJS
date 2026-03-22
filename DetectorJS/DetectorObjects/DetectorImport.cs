namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorImport(string className, string moduleName) : DetectorObject
    {
        public string ClassName = className;

        public string ModuleName = moduleName;

        public override string Serialize() => $"import {{ {ClassName} }} from \"{ModuleName}\"";

        public override bool Equals(object obj) => obj is DetectorImport import && ClassName == import.ClassName && ModuleName == import.ModuleName;
    }
}
