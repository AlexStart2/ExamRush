using System;
using System.Collections.Generic;
using System.IO;

public static class SimpleEnvLoader
{
    public static Dictionary<string, string> LoadEnvFile(string filePath)
    {
        var envVars = new Dictionary<string, string>();

        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogWarning($"File .env not found at: {filePath}");
            return envVars;
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            var parts = trimmedLine.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');
                envVars[key] = value;

                Environment.SetEnvironmentVariable(key, value);
            }
        }

        return envVars;
    }
}
