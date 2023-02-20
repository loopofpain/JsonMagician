using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

using Newtonsoft.Json.Linq;

using Newtonsoft.Json;

namespace LoopOfPain.JsonMagician.Cli
{
    [Command("flatten", Description = "Placeholder")]
    internal class FlattenJsonCommand
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
