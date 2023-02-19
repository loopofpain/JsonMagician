using System.ComponentModel.DataAnnotations;

using McMaster.Extensions.CommandLineUtils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoopOfPain.JsonMagician.Cli
{
    [Command(Name = "json-magican", Description = "Placeholder"),
     Subcommand(typeof(ConvertCommand))]
    class CliRoot
    {
        public static void Main(string[] args)
        {
            CommandLineApplication.Execute<CliRoot>(args);
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify at a subcommand.");
            app.ShowHelp();
            return 1;
        }

        [Command("convert", Description = "Placeholder"),
         Subcommand(typeof(FlattenJson))]
        private class ConvertCommand
        {
            private int OnExecute(IConsole console)
            {
                console.Error.WriteLine("You must specify an action. See --help for more details.");
                return 1;
            }
        }

        [Command("flatten", Description = "Placeholder")]
        private class FlattenJson
        {
            [Required(ErrorMessage = "Placeholder")]
            [Argument(0, Description = "Path to input .json file")]
            public string PathToInputJsonFile { get; } = string.Empty;

            [Required(ErrorMessage = "Placeholder")]
            [Argument(1, Description = "Path to output file")]
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

                console.Out.WriteLine("Json file was read!");

                var flattenedJsonObjectAsDictionary = jsonObjectAsToken.FlattenJsonObject();

                console.Out.WriteLine("Json was flattened!");

                console.Out.WriteLine("Writing result to file...");

                var flattenedJsonText = JsonConvert.SerializeObject(flattenedJsonObjectAsDictionary, Formatting.Indented);

                File.WriteAllText(PathToOutputFile, flattenedJsonText);

                console.Out.WriteLine($"File was created at '{PathToOutputFile}'");

                return 0;
            }
        }
    }
}