#nullable disable

using Quire.ThirdParty;

namespace Quire;

public partial class PrimeCommandLineArguments
{
    [Argument(ArgumentType.AtMostOnce, ShortName = "e", LongName = "Embedding", DefaultValue = false,
        HelpText = "The application will run as a COM service.")]
    public bool Embedding;

    [DefaultArgument(ArgumentType.Multiple, HelpText = "Optional file to open")]
    public string[] SourcePaths;

    [Argument(ArgumentType.AtMostOnce, ShortName = "c", LongName = "culture",
        HelpText = "The culture to use for localization purposes.")]
    public string Culture;

    [Argument(ArgumentType.AtMostOnce, ShortName = "o", LongName = "EmbeddingObsolete", DefaultValue = false,
        HelpText = "The application will run as a COM service for special COM Clients only, like Creo Analysis Features.")]
    public bool EmbeddingObsolete;

    [Argument(ArgumentType.AtMostOnce, ShortName = "n", LongName = "EmbeddingEngNote", DefaultValue = false,
        HelpText = "The application will run as a COM service in Engineering Notebook mode.")]
    public bool EmbeddingEngNote;

    [Argument(ArgumentType.AtMostOnce, ShortName = "p", LongName = "EmbeddingEngNotePlus", DefaultValue = false,
        HelpText = "The application will run as a COM service in Engineering Notebook Plus mode.")]
    public bool EmbeddingEngNotePlus;
}
