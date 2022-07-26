using BROT.Exceptions;

namespace BROT
{
    internal class Program
    {
        const string AUTHOR = "kievzenit";
        const string LICENSE = "MIT";
        const byte MAJOR_VERSION = 0;
        const byte MINOR_VERSION = 1;
        const byte PATCH_VERSION = 0; 

        const byte DEFAULT_ROTATION_DEGREE = 13;

        static byte rotationDegree = DEFAULT_ROTATION_DEGREE;
        static bool isPositiveRotation = true;

        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 0: PrintUsage(); break;
                case 1: OneInputArgumentHandler(args[0]); break;
                default: MultipleInputArgumentsHandler(args); break;
            }

            //TODO: add support for directories, not only files
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
@$"BROT usage:
    -h or --help: prints this message
    -d or --degrees number: to specify rotation degrees (default: 13)
    -p or --positive: to specify rotation type as positive (default)
    -n or --negative: to specify rotation type as negative
    [files...] filenames to be rotated, note: output file will be the same

    example:
        .\brot -d 17 -n test.txt text.txt

author: {AUTHOR}
license: {LICENSE}
version: {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH_VERSION}
");
        }

        private static void PrintError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        private static bool CheckIfArgumentIsKey(string argument)
        {
            return 
                argument == "-h"
                || argument == "--help"
                || argument == "-d"
                || argument == "--degrees"
                || argument == "p"
                || argument == "--positive"
                || argument == "-n"
                || argument == "--negative";
        }

        private static void OneInputArgumentHandler(string argument)
        {
            if (argument == "-h" || argument == "--help")
            {
                PrintUsage();
                return;
            }

            if (CheckIfArgumentIsKey(argument))
            {
                PrintUsage();
                PrintError("When using one parameter it must be a filename!");

                return;
            }

            try
            {
                RotateFile(argument);
            }
            catch (PathTooLongException)
            {
                PrintError($"Specified path: {argument} too long!");
            }
            catch (DirectoryNotFoundException)
            {
                PrintError($"Directory: {argument} not found!");
            }
            catch (UnauthorizedAccessException)
            {
                PrintError($"You don't have permission to this file: {argument}!");
            }
            catch (FileNotFoundException)
            {
                PrintError($"File: {argument} not found!");
            }
        }

        private static void MultipleInputArgumentsHandler(string[] arguments)
        {
            var lastKeyPosition = -1;
            for (var i = 0; i < 3; i++)
            {
                if (CheckIfArgumentIsKey(arguments[i]))
                {
                    lastKeyPosition = i;

                    try
                    {
                        ParseSettings(new string[2] { arguments[i], arguments[i + 1] });
                    }
                    catch (FormatException)
                    {
                        PrintError($"{arguments[i+1]} is not a number or not in this range: 0-255!");
                        return;
                    }
                    catch (IncorrectPlaceForHelpException)
                    {
                        PrintUsage();
                        return;
                    }
                }
            }

            for (var i = lastKeyPosition + 1; i < arguments.Length; i++)
            {
                try
                {
                    RotateFile(arguments[i]);
                }
                catch (PathTooLongException)
                {
                    PrintError($"Specified path: {arguments[i]} too long!");
                }
                catch (DirectoryNotFoundException)
                {
                    PrintError($"Directory: {arguments[i]} not found!");
                }
                catch (UnauthorizedAccessException)
                {
                    PrintError($"You don't have permission to this file: {arguments[i]}!");
                }
                catch (FileNotFoundException)
                {
                    PrintError($"File: {arguments[i]} not found!");
                }
            }
        }

        private static void ParseSettings(string[] arguments)
        {
            switch (arguments[0])
            {
                case "-h":
                case "--help":
                    PrintUsage();
                    throw new IncorrectPlaceForHelpException();

                case "-d":
                case "--degrees":
                    rotationDegree = byte.Parse(arguments[1]);
                    break;

                case "-p":
                case "--positive":
                    isPositiveRotation = true;
                    break;

                case "-n":
                case "--negative":
                    isPositiveRotation = false;
                    break;
            }
        }

        private static void RotateFile(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                var buffer = new byte[file.Length];
                var bytesRead = file.Read(buffer);

                file.Position = 0;
                file.Write(BROT.Rotate(buffer, rotationDegree, isPositiveRotation));
            }
        }
    }
}