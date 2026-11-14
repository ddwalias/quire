using System.Text.Json.Serialization;

sealed record GeneratedAssemblyBinding(
    [property: JsonPropertyName("assembly")] string Assembly,
    [property: JsonPropertyName("allowed_missing_types")] string[] AllowedMissingTypes);
