namespace CssClassConstGenerator;
internal record ParsedCssFile
{
    public string AssemblyName { get; set; } = ""; //they do need the assembly name.
    public bool DoAllClasses { get; set; }
    public BasicList<ClassInfo> Classes { get; set; } = []; //otherwise, can't group later.
}