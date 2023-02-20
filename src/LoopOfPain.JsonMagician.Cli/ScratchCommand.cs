using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

using Newtonsoft.Json.Linq;

using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;

namespace LoopOfPain.JsonMagician.Cli
{
    [Command("scratch", Description = "Placeholder")]
    internal class ScratchCommand
    {
        [Required(ErrorMessage = "Placeholder")]
        [Argument(0, Description = "Path to input .json file")]
        public string PathToInputJsonFile { get; } = string.Empty;

        [Required(ErrorMessage = "Placeholder")]
        [Argument(1, Description = "Path to output .json file")]
        public string PathToOutputFile { get; } = string.Empty;

        private int OnExecute(IConsole console)
        {
            if (!File.Exists(PathToInputJsonFile))
            {
                console.Error.WriteLine($"Input file '{PathToInputJsonFile}' does not exist!");
                return 1;
            }

            var outputDirectory = Path.GetDirectoryName(PathToOutputFile);


            if (string.IsNullOrEmpty(outputDirectory))
            {
                console.WriteLine($"Path to dictory of the output file '{outputDirectory}' could not be determined");

                return 1;
            }

            Directory.CreateDirectory(outputDirectory);

            if (File.Exists(PathToOutputFile))
            {
                console.Out.WriteLine($"Deleting existing file '{PathToOutputFile}'...");

                File.Delete(PathToOutputFile);

                console.Out.WriteLine($"Existing file '{PathToOutputFile}' was deleted successfully!");
            }

            using var inputFileStreamReader = new StreamReader(PathToInputJsonFile);
            var fileAsString = inputFileStreamReader.ReadToEnd();

            var jsonObjectAsToken = (JObject)JsonConvert.DeserializeObject(fileAsString)!;

            var parsed = ParseFlattenedJsonObject(jsonObjectAsToken);

            var outputObject = new ExpandoObject() as IDictionary<string, object>;

            var allKeys = parsed.Keys.ToList();

            foreach (var item in allKeys)
            {
                var regex = new Regex("(__\\d)");
                var isArrayElement = regex.Matches(item);
                var isMatch = regex.IsMatch(item);
                var regexParsedContent = isArrayElement.Select(x => x.ToString()).ToList();
                // Loop through elements
                var keySplittedByArrayStart = isMatch ? item.Split(regexParsedContent[0]) : Array.Empty<string>();
                var keySegments = keySplittedByArrayStart.Any() ? keySplittedByArrayStart[0].Split("__") : item.Split("__");

                var outputObjectReference = outputObject;

                for (var segmentIndex = 0; segmentIndex < keySegments.Length; segmentIndex++)
                {
                    var nameOfProperty = keySegments[segmentIndex];

                    // Debug
                    if(nameOfProperty.Contains("Write") || regexParsedContent.Count > 1)
                    {
                        var here = 0;
                    }

                    if (!PropertyExists(outputObjectReference, nameOfProperty))
                    {
                        if (isMatch && segmentIndex == (keySegments.Length - 1))
                        {
                            outputObjectReference.Add(nameOfProperty, new List<object>());
                            (outputObjectReference[nameOfProperty] as List<object>).Add(parsed[item]);

                            
                        }else if (!isMatch && segmentIndex == (keySegments.Length - 1))
                        {
                            outputObjectReference.Add(nameOfProperty, parsed[item]);
                        }
                        else
                        {
                            outputObjectReference.Add(nameOfProperty, new ExpandoObject());
                            outputObjectReference = outputObjectReference[nameOfProperty] as IDictionary<string, object>;

                        }
                    }
                    else
                    {
                        if (isMatch && segmentIndex == (keySegments.Length - 1))
                        {
                            if (outputObjectReference[nameOfProperty] is List<object>)
                            {
                                (outputObjectReference[nameOfProperty] as List<object>).Add(parsed[item]);
                            }
                        }
                        else
                        {
                            outputObjectReference = outputObjectReference[nameOfProperty] as IDictionary<string, object>;
                        }
                    }
                }
            }

            var flattenedJsonText = JsonConvert.SerializeObject(outputObject, Formatting.Indented);

            File.WriteAllText(PathToOutputFile, flattenedJsonText);

            console.Out.WriteLine($"File was created at '{PathToOutputFile}'");

            //console.Out.WriteLine("Json file was read!");

            //var flattenedJsonObjectAsDictionary = jsonObjectAsToken.FlattenJsonObject();

            //console.Out.WriteLine("Json was flattened!");

            //console.Out.WriteLine("Writing result to file...");

            //var flattenedJsonText = JsonConvert.SerializeObject(flattenedJsonObjectAsDictionary, Formatting.Indented);

            //File.WriteAllText(PathToOutputFile, flattenedJsonText);

            //console.Out.WriteLine($"File was created at '{PathToOutputFile}'");

            return 0;
        }

        private static void SetObjectProperty(string propertyName, string value, object obj)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }

        public static IDictionary<string, object?> ParseFlattenedJsonObject(JObject jsonObject)
        {
            var objectsInJsonFile = jsonObject.Children().ToList();
            var sortedDict = new SortedDictionary<string, object>();

            foreach (var item in objectsInJsonFile)
            {
                var elem = item.Children().FirstOrDefault()!;
                var val = elem.Value<object>();
                var key = elem.Path;
                sortedDict.Add(key, val);
            }

            return sortedDict;
        }
    }


}
