using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using McMaster.Extensions.CommandLineUtils;

namespace LoopOfPain.JsonMagician.Cli
{

    [Command("convert", Description = "Placeholder"),
     Subcommand(typeof(FlattenJsonCommand))]
    internal class ConvertCommand
    {
        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }
    }

}
