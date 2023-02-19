// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using McMaster.Extensions.CommandLineUtils;

namespace LoopOfPain.JsonMagician.Cli
{
    [Command(Name = "json-magican", Description = "Placeholder"),
     Subcommand(typeof(ConvertCommand))]
    class CliRoot
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<CliRoot>(args);

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

        [Command("flatten", Description = "Placeholder",
    AllowArgumentSeparator = true,
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
        private class FlattenJson
        {
            [Required(ErrorMessage = "Placeholder")]
            [Argument(0, Description = "Path to input .json file")]
            public string PathToInputJsonFile { get; }

            [Required(ErrorMessage = "Placeholder")]
            [Argument(1, Description = "Path to output file")]
            public string PathToOutputFile { get; }

            /// <summary>
            /// When UnrecognizedArgumentHandling is StopParsingAndCollect, any unrecognized arguments
            /// will be collected and set in this property, when set.
            /// </summary>
            public string[] RemainingArguments { get; }

            private int OnExecute(IConsole console)
            {
                if (!File.Exists(PathToInputJsonFile))
                {
                    console.Error.WriteLine($"Input file '{PathToInputJsonFile}' does not exist!");
                    return 1;
                }


                var outPutPath = Path.GetDirectoryName(PathToOutputFile);

                if (!string.IsNullOrEmpty(outPutPath))
                {
                    Directory.CreateDirectory(outPutPath);
                }

                using var inputFileStreamReader = new StreamReader(PathToInputJsonFile);
                var fileAsString = inputFileStreamReader.ReadToEnd();

                return 0;
            }
        }
    }
}