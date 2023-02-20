using McMaster.Extensions.CommandLineUtils;

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

    }
}