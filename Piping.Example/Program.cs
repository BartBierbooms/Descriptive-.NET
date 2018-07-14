using System;
using Serilog;
using Piping.Example.ExampleBusiness.Scenario;

namespace Piping.Example
{
    public class Program
    {

 
        static void Main(string[] args)
        {

            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Serilog.Log.Logger = log;
            Serilog.Log.Information("Hello, Serilog!");

            Console.WriteLine("Type any key to proceed. Example of piping, but the piping doesn't return functions");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Type Q to quit.");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Type T For Test.");
            Console.WriteLine("Type any other key for Run.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();

            var input = Console.ReadLine();


            while (input.Trim().ToUpper() != "Q")
            {
                if (input.Trim().ToUpper() == "T")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    Console.WriteLine("Type 1 for partial Test on SaveBasket");
                    Console.WriteLine("Type 2 for partial Test on reserveProduct");
                    Console.WriteLine("Type 3 for Full Test on scenario");
                    Console.WriteLine("Type any other key for exiting Test.");
                    var inputTest = Console.ReadLine().Trim().ToUpper();
                    while (inputTest == "1" || inputTest == "2" || inputTest == "3")
                    {

                        Piping.Example.ExampleTest.Test.TestMe(int.Parse(inputTest));

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Type 1 for partial Test on SaveBasket");
                        Console.WriteLine("Type 2 for partial Test on reserveProduct");
                        Console.WriteLine("Type 3 for Full Test on scenario");
                        Console.WriteLine("Type any other key for exiting Test.");
                        inputTest = Console.ReadLine().Trim().ToUpper();

                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Type Q to quit.");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Type T For Test.");
                    Console.WriteLine("Type any other key for Run.");
                }
                else {

                    RunScenario.Run();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Type Q to quit.");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Type T For Test.");
                    Console.WriteLine("Type any other key for Run.");
                }

                input = Console.ReadLine();

            }
        }
    }
}
