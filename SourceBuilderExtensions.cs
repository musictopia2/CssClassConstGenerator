namespace CssClassConstGenerator;
internal static class SourceBuilderExtensions
{
    public static void WriteServiceExtensionsStub(this SourceCodeStringBuilder builder, Action<ICodeBlock, ResultsModel> action, ResultsModel result)
    {
        builder.WriteLine("#nullable enable");
        builder.WriteLine($"namespace {result.Namespace};")
            .WriteLine($"public static partial class {result.ClassName}")
            .WriteCodeBlock(w =>
            {
                action.Invoke(w, result);
            });
    }
}