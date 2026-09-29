using System.ComponentModel.DataAnnotations;

namespace FileStreaming.Api.Settings;

public sealed class ConnectionStringsSettings : ISettings
{
    public static string SectionName => "ConnectionStrings";

    [Required(ErrorMessage = "A connection string não foi configurada.")]
    public string ConnectionString { get; init; } = string.Empty;
}
