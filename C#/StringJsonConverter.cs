using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Linq;

namespace StringJsonConverter
{
    public class JsonConverter
    {
        public class ConversionResult<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Error { get; set; }
        }

        // Convert string to JsonDocument
        public static ConversionResult<JsonDocument> ToJsonDocument(string input)
        {
            try
            {
                var document = JsonDocument.Parse(input);
                return new ConversionResult<JsonDocument>
                {
                    Success = true,
                    Data = document
                };
            }
            catch (JsonException ex)
            {
                return new ConversionResult<JsonDocument>
                {
                    Success = false,
                    Error = $"JSON parsing error: {ex.Message}"
                };
            }
        }

        // Convert string to JsonNode
        public static ConversionResult<JsonNode> ToJsonNode(string input)
        {
            try
            {
                var node = JsonNode.Parse(input);
                return new ConversionResult<JsonNode>
                {
                    Success = true,
                    Data = node
                };
            }
            catch (JsonException ex)
            {
                return new ConversionResult<JsonNode>
                {
                    Success = false,
                    Error = $"JSON parsing error: {ex.Message}"
                };
            }
        }

        // Convert string to Dictionary
        public static ConversionResult<Dictionary<string, object>> ToDictionary(string input)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(input, options);
                return new ConversionResult<Dictionary<string, object>>
                {
                    Success = true,
                    Data = dictionary
                };
            }
            catch (JsonException ex)
            {
                return new ConversionResult<Dictionary<string, object>>
                {
                    Success = false,
                    Error = $"JSON parsing error: {ex.Message}"
                };
            }
        }

        // Convert string to dynamic object
        public static ConversionResult<dynamic> ToDynamic(string input)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var obj = JsonSerializer.Deserialize<dynamic>(input, options);
                return new ConversionResult<dynamic>
                {
                    Success = true,
                    Data = obj
                };
            }
            catch (JsonException ex)
            {
                return new ConversionResult<dynamic>
                {
                    Success = false,
                    Error = $"JSON parsing error: {ex.Message}"
                };
            }
        }

        // Convert string to strongly typed object
        public static ConversionResult<T> ToType<T>(string input)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var obj = JsonSerializer.Deserialize<T>(input, options);
                return new ConversionResult<T>
                {
                    Success = true,
                    Data = obj
                };
            }
            catch (JsonException ex)
            {
                return new ConversionResult<T>
                {
                    Success = false,
                    Error = $"JSON parsing error: {ex.Message}"
                };
            }
        }

        // Convert malformed JSON string to valid JSON
        public static ConversionResult<string> SanitizeJson(string input)
        {
            try
            {
                // Remove comments
                input = RemoveComments(input);

                // Fix missing quotes around property names
                input = FixPropertyNames(input);

                // Fix single quotes
                input = input.Replace('\'', '"');

                // Fix trailing commas
                input = RemoveTrailingCommas(input);

                // Validate the result
                using (JsonDocument.Parse(input))
                {
                    return new ConversionResult<string>
                    {
                        Success = true,
                        Data = input
                    };
                }
            }
            catch (JsonException ex)
            {
                return new ConversionResult<string>
                {
                    Success = false,
                    Error = $"JSON sanitization error: {ex.Message}"
                };
            }
        }

        private static string RemoveComments(string json)
        {
            var multiLinePattern = @"/\*.*?\*/";
            var singleLinePattern = @"//.*?$";

            json = Regex.Replace(json, multiLinePattern, "", RegexOptions.Singleline);
            json = Regex.Replace(json, singleLinePattern, "", RegexOptions.Multiline);

            return json;
        }

        private static string FixPropertyNames(string json)
        {
            // Fix unquoted property names
            var propertyPattern = @"(\{|\,)\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*:";
            return Regex.Replace(json, propertyPattern, "$1\"$2\":");
        }

        private static string RemoveTrailingCommas(string json)
        {
            // Remove trailing commas in objects
            json = Regex.Replace(json, @",(\s*})", "$1");
            // Remove trailing commas in arrays
            json = Regex.Replace(json, @",(\s*])", "$1");
            return json;
        }
    }

    // Example usage
    public class Program
    {
        public record Person(string Name, int Age);

        public static void Main()
        {
            // Example JSON strings
            var validJson = @"{""name"": ""John"", ""age"": 30}";
            var malformedJson = @"{name: 'John', age: 30,}"; // Malformed JSON

            // Using JsonDocument
            var docResult = JsonConverter.ToJsonDocument(validJson);
            if (docResult.Success)
            {
                Console.WriteLine("Successfully parsed to JsonDocument");
            }

            // Using Dictionary
            var dictResult = JsonConverter.ToDictionary(validJson);
            if (dictResult.Success)
            {
                var dict = dictResult.Data;
                Console.WriteLine($"Name: {dict["name"]}, Age: {dict["age"]}");
            }

            // Using strongly typed object
            var personResult = JsonConverter.ToType<Person>(validJson);
            if (personResult.Success)
            {
                var person = personResult.Data;
                Console.WriteLine($"Person: {person.Name}, {person.Age}");
            }

            // Sanitizing malformed JSON
            var sanitizeResult = JsonConverter.SanitizeJson(malformedJson);
            if (sanitizeResult.Success)
            {
                Console.WriteLine($"Sanitized JSON: {sanitizeResult.Data}");
            }
        }
    }
}
