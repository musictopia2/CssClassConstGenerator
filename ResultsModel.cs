namespace CssClassConstGenerator;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public BasicList<MethodInfo> Methods { get; set; } = [];
}