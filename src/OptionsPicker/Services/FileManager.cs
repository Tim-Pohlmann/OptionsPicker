using OptionsPicker.Models;
using System.Globalization;
using System.Text;

namespace OptionsPicker.Services;

public class FileManager : IFileManager
{
    public string ExportOptions(IReadOnlyList<Option> options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# OptionsPicker Export");
        sb.AppendLine($"# Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"# Total options: {options.Count}");
        sb.AppendLine();

        foreach (var option in options)
        {
            // Format weight as integer if it's a whole number, otherwise with minimal decimals
            var weightStr = option.Weight == Math.Floor(option.Weight)
                ? option.Weight.ToString("F0", CultureInfo.InvariantCulture)
                : option.Weight.ToString("G", CultureInfo.InvariantCulture);

            sb.AppendLine($"{option.Name}:{weightStr}");
        }

        return sb.ToString().TrimEnd();
    }

    public async Task<IReadOnlyList<Option>> ImportOptionsAsync(string fileContent)
    {
        await Task.Yield(); // Make it truly async

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return new List<Option>();
        }

        var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var options = new Dictionary<string, Option>(); // Use dictionary to handle duplicates
        var lineNumber = 0;

        foreach (var rawLine in lines)
        {
            lineNumber++;
            var line = rawLine.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }

            try
            {
                var option = ParseOptionLine(line, lineNumber);
                options[option.Name] = option; // Last occurrence wins for duplicates
            }
            catch (ArgumentException)
            {
                // Re-throw ArgumentException as-is (for validation errors)
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing line {lineNumber}: '{line}'. {ex.Message}", ex);
            }
        }

        return options.Values.ToList();
    }

    public async Task<string> GenerateDownloadFileAsync(IReadOnlyList<Option> options)
    {
        await Task.Yield(); // Make it truly async

        var content = ExportOptions(options);
        var encodedContent = Uri.EscapeDataString(content);
        return $"data:text/plain;charset=utf-8,{encodedContent}";
    }

    private static Option ParseOptionLine(string line, int lineNumber)
    {
        // Handle multiple colons by splitting on the last colon
        var lastColonIndex = line.LastIndexOf(':');

        string name;
        double weight = 1.0; // Default weight

        if (lastColonIndex == -1)
        {
            // No colon found, use default weight
            name = line.Trim();
        }
        else
        {
            name = line.Substring(0, lastColonIndex).Trim();
            var weightStr = line.Substring(lastColonIndex + 1).Trim();

            if (!string.IsNullOrEmpty(weightStr))
            {
                if (!double.TryParse(weightStr, NumberStyles.Float, CultureInfo.InvariantCulture, out weight))
                {
                    throw new FormatException($"Invalid weight value '{weightStr}'. Weight must be a valid number.");
                }
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Option name cannot be empty.");
        }

        if (weight <= 0)
        {
            throw new ArgumentException($"Weight must be greater than 0, but was {weight}.");
        }

        return Option.Create(name, weight);
    }
}