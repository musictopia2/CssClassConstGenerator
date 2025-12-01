namespace CssClassConstGenerator;
internal class EmitClass(ImmutableArray<ResultsModel> results, SourceProductionContext context)
{
    public void Emit()
    {
        int x = 0;
        foreach (var item in results)
        {
            x++;
            WriteItem(item, x);
        }
    }
    private void WriteItem(ResultsModel item, int id)
    {
        SourceCodeStringBuilder builder = new();
        //if you are doing partial classes, then this will do the work.  if something else, rethink.
        builder.WriteServiceExtensionsStub(PopulateDetails, item);
        context.AddSource($"{item.ClassName}{id}.Css.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        foreach (var item in result.Methods)
        {
            w.WriteLine($"/// <summary>CSS class: {item.Value}</summary>");
            w.WriteLine($"""
                public const string {item.MethodName} = "{item.Value}";
                """);
        }
    }
}