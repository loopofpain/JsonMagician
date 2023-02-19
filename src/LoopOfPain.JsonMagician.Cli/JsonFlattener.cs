using Newtonsoft.Json.Linq;

namespace LoopOfPain.JsonMagician.Cli;

internal static class JsonFlattener
{
    public static IDictionary<string, object?> FlattenJsonObject(this JObject jsonObject)
    {
        var objectsInJsonFile = jsonObject.Children().ToList();

        var tokensAsDictionary = ConvertJsonTokensToDictionary(objectsInJsonFile);

        var cleanDictionary = CleanKeysOfDictionary(tokensAsDictionary);

        return cleanDictionary;
    }

    private static Dictionary<string, object?> ConvertJsonTokensToDictionary(IList<JToken> jToken)
    {
        var children = jToken?.Count > 0 ? jToken.Children().ToList() : new List<JToken>();

        var parsedTokens = new Dictionary<string, object?>();

        foreach (var child in children)
        {
            var parsedTokensFromChild = ConvertSingleTokenToDictionary(child);
            AddToDictionary<string, object?>(ref parsedTokens, parsedTokensFromChild);
        }

        return parsedTokens;
    }

    private static IDictionary<string, object?> CleanKeysOfDictionary(Dictionary<string, object?> data)
    {
        var values = new SortedDictionary<string, object?>();

        foreach (var item in data)
        {
            var currentKey = item.Key + "";
            currentKey = currentKey.Replace("].", "__").Replace(".[", "__").Replace("[", "__").Replace("]", "");
            //currentKey = currentKey.Replace("[", "__").Replace("].", "__").Replace("]", "");

            var splitted = currentKey.Split(".");
            var joined = string.Join("__", splitted);

            values.Add(joined, item.Value);
        }

        return values;
    }

    private static void AddToDictionary<TKey, TValue>(ref Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> collection) 
        where TKey: notnull
    {
        if (collection is null)
        {
            return;
        }

        foreach (var item in collection)
        {
            if (!source.ContainsKey(item.Key))
            {
                source.Add(item.Key, item.Value);
            }
        }
    }


    private static Dictionary<string, object?> ConvertSingleTokenToDictionary(JToken jToken)
    {
        var values = new Dictionary<string, object?>();

        if (jToken is null)
        {
            return values;
        }

        var tokens = jToken.HasValues ? jToken.Children().ToList() : new List<JToken>();

        if (!tokens!.Any())
        {
            values.Add(jToken!.Path, jToken?.Value<object>());
            return values;
        }

        foreach (var token in tokens)
        {
            InnerTraverse(token, ref values);
        }

        return values;
    }

    private static void InnerTraverse(JToken jToken, ref Dictionary<string, object?> dictionary)
    {
        if (jToken is null)
        {
            return;
        }

        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        var tokens = jToken.HasValues ? jToken.Children().ToList() : new List<JToken>();

        if (!tokens.Any())
        {
            dictionary.Add(jToken.Path, jToken.Value<object>());
            return;
        }

        foreach (var token in tokens)
        {
            InnerTraverse(token, ref dictionary);
        }
    }
}