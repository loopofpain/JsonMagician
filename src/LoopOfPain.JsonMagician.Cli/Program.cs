// See https://aka.ms/new-console-template for more information

using LoopOfPain.JsonMagician.Cli.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

Console.WriteLine("Hello, World!");

using MemoryStream inputFileMemoryStream = new MemoryStream(Resources.some_config_file);
using StreamReader inputFileStreamReader = new StreamReader(inputFileMemoryStream);

var config = inputFileStreamReader.ReadToEnd();
var data = (JObject)JsonConvert.DeserializeObject(config);

var objectsInJsonFile = data.Children().ToList();

var tokensAsDictionary = ConvertJsonTokensToDictionary(objectsInJsonFile);

var cleanDictionary = CleanKeysOfDictionary(tokensAsDictionary);

var resultDictionary = JsonConvert.SerializeObject(cleanDictionary);

config += "hi";

IDictionary<string, object?> CleanKeysOfDictionary(Dictionary<string, object?> data)
{
    var values = new SortedDictionary<string, object?>();

    foreach (var item in data)
    {
        var currentKey = item.Key + "";
        currentKey = currentKey.Replace("[", "__").Replace("].", "__").Replace("]", "");

        var splitted = currentKey.Split(".");
        var joined = string.Join("__", splitted);

        values.Add(joined, item.Value);
    }

    return values;
}

Dictionary<string, object?> ConvertJsonTokensToDictionary(IList<JToken> jToken)
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

static void AddToDictionary<TKey, TValue>(ref Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> collection)
{
    if (collection == null)
    {
        throw new ArgumentNullException("Collection is null");
    }

    foreach (var item in collection)
    {
        if (!source.ContainsKey(item.Key))
        {
            source.Add(item.Key, item.Value);
        }
    }
}


Dictionary<string, object?> ConvertSingleTokenToDictionary(JToken jToken)
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

static void InnerTraverse(JToken jToken, ref Dictionary<string, object?> dictionary)
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