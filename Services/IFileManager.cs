using OptionsPicker.Models;

namespace OptionsPicker.Services;

public interface IFileManager
{
    string ExportOptions(IReadOnlyList<Option> options);
    Task<IReadOnlyList<Option>> ImportOptionsAsync(string fileContent);
    Task<string> GenerateDownloadFileAsync(IReadOnlyList<Option> options);
}