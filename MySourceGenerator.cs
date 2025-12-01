namespace CssClassConstGenerator;

[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var declares1 = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
        var declares2 = context.CompilationProvider.Combine(declares1.Collect());
        var xmlfiles = declares2.Select(static (x, _) =>
        {
            var files = x.Right
                .Where(text => text.GetText() is not null)
                .Select(text => new FileInformation
                {
                    Content = text.GetText()!.ToString(),
                    FileName = text.Path,
                    AssemblyName = x.Left.AssemblyName!
                }).ToImmutableArray();
            return files;
        });
        var parsedxml = xmlfiles.Select(static (x, _) =>
        {
            //this is where you parse the xml files.
            //for now, just return empty list.
            BasicList<ParsedCssFile> output = [];
            foreach (var item in x)
            {
                var doc = XDocument.Parse(item.Content);
                ParsedCssFile parsed = new()
                {
                    AssemblyName = item.AssemblyName,
                    DoAllClasses = true, //for now, just do all classes.
                };

                var classes = doc.Root?
                    .Elements("ClassInformation")
                    .Select(e => new ClassInfo(
                        ClassName: e.Element("ClassName")?.Value ?? "",
                        MethodName: e.Element("MethodName")?.Value ?? "",
                        Namespace: e.Element("Namespace")?.Value ?? "",
                        Value: e.Element("Value")?.Value ?? ""
                    ))
                    .ToBasicList();
                parsed.Classes = classes!;
                output.Add(parsed);
            }
            return output.ToImmutableArray();
        });


        var groupedModels = parsedxml.Select((parsedFiles, _) =>
        {
            var flatList = parsedFiles.SelectMany(parsed =>
            {
                // Normal groups
                var standardGroups = parsed.Classes
                    .GroupBy(c => (c.ClassName, parsed.AssemblyName, c.Namespace))
                    .Select(g => new ResultsModel
                    {
                        ClassName = g.Key.ClassName,
                        Namespace = $"{g.Key.AssemblyName}.{g.Key.Namespace}",
                        Methods = g.Select(x => new MethodInfo
                        {
                            MethodName = x.MethodName,
                            Value = x.Value
                        }).ToBasicList()
                    });

                // If DoAllClasses is true, create additional global group
                if (parsed.DoAllClasses)
                {
                    var allGroup = new ResultsModel
                    {
                        ClassName = "AllCssClasses",
                        Namespace = $"{parsed.AssemblyName}.CssHelpers",
                        Methods = parsed.Classes
                            .Select(x => new MethodInfo
                            {
                                MethodName = x.MethodName,
                                Value = x.Value
                            })
                            .ToBasicList()
                    };
                    return standardGroups.Append(allGroup);
                }
                return standardGroups;
            });
            return flatList.ToImmutableArray();
        });
        context.RegisterSourceOutput(groupedModels, Execute);
    }
    private void Execute(SourceProductionContext context, ImmutableArray<ResultsModel> list)
    {
        //TestConsoleClass emit = new(list, context);
        EmitClass emit = new(list, context);
        emit.Emit(); //start out with console.  later do reals once ready.
    }
}