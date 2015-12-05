using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXolTodoTxt;
using CommandLine;
using CommandLine.Text;
using System.ComponentModel;

namespace cli
{
    class AddSubOptions
    {
        [ValueList(typeof(List<string>), MaximumElements = -1)]
        public IList<string> Text { get; set; }
        public string TextAsString
        {
            get
            {
                return String.Join(" ", Text);
            }
        }
        // Remainder omitted
    }

    class Options
    {
        [VerbOption("add", HelpText = "Add a todo")]
        public AddSubOptions Add { get; set; }

        // omitting long name, default --verbose
        [Option('v', "verbose", Required = false, DefaultValue = false,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, 
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }

    class Program
    {
        static void Todo(Options opt)
        {

        }

        static void Invoked(string verb, object instance)
        {
            if (verb != null && instance != null)
            {
                if (verb == "add")
                {
                    var addSubOptions = (AddSubOptions)instance;
                    Console.WriteLine(String.Format("Add todo: \"{0}\"", addSubOptions.TextAsString));
                }
            }
        }

        static void Main(string[] args)
        {
            string invokedVerb;
            object invokedVerbInstance;

            var options = new Options();

            if (!Parser.Default.ParseArguments(args, options,
                        (verb, subOptions) =>
                            {
                                // if parsing succeeds the verb name and correct instance
                                // will be passed to onVerbCommand delegate (string,object)
                                invokedVerb = verb;
                                invokedVerbInstance = subOptions;
                                Invoked(invokedVerb, invokedVerbInstance);
                            }
                    )
                )
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            
            // Remainder omitted

            //if (Parser.Default.ParseArguments(args, options))
            //{
            //    // Values are available here
            //    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(options))
            //    {
            //        string name = descriptor.Name;
            //        object value = descriptor.GetValue(options);
            //        Console.WriteLine("{0}={1}", name, value);
            //    }
            //}
        }
    }
}
