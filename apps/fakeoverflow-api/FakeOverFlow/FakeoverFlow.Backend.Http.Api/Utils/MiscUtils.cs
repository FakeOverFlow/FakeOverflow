using System.Security.Claims;

namespace FakeoverFlow.Backend.Http.Api.Utils;

public static class MiscUtils 
{
    /// <summary>
    /// Splits a string into key-value pairs based on the specified delimiters and parses it into a dictionary.
    /// </summary>
    /// <param name="value">The input string to be split and parsed.</param>
    /// <param name="seperator">The delimiter used to separate key-value pairs in the string. Default is ";".</param>
    /// <param name="keyValueSeperator">The delimiter used to separate keys from values within each pair. Default is "=".</param>
    /// <returns>A dictionary containing the parsed key-value pairs from the input string.</returns>
    public static Dictionary<string, string> SplitAndParseFromString(this string value, string seperator = ";",
        string keyValueSeperator = "=")
    {
        var keyValuePairs = value.Split(seperator);
        var dictionary = new Dictionary<string, string>();
        foreach (var keyValuePair in keyValuePairs)
        {
            var keyValue = keyValuePair.Split(keyValueSeperator);
            dictionary.Add(keyValue[0], keyValue[1]);
        }
        return dictionary;
    }
    
    public static ClaimsPrincipal GetPrincipalFromContext(this HttpContext context)
    {
        return context.User;
    }
}