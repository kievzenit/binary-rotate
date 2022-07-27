using BROT.Exceptions;

namespace BROT
{
    internal class Program
    {
        const string AUTHOR = "kievzenit";
        const string LICENSE = "MIT";
        const byte MAJOR_VERSION = 0;
        const byte MINOR_VERSION = 2;
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
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
@$"BROT usage:
    -h or --help: prints this message
    -d or --degrees number: to specify rotation degrees (default: 13)
    -p or --positive: to specify rotation type as positive (default)
    -n or --negative: to specify rotation type as negative
    [files... directories...] filenames or directories to be rotated, note: output file will be the same

    example:
        .\brot -d 17 -n test.txt text.txt directory

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
                || argument == "-p"
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
                RotatePath(argument);
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
            var files = new List<string>();

            try
            {
                files.AddRange(ParseSettings(arguments));
            }
            catch (IncorrectPlaceForHelpException)
            {
                return;
            }
            catch (IndexOutOfRangeException ex)
            {
                PrintError("Degrees key was specifies, but degrees number wasn't!");
                return;
            }
            catch (FormatException)
            {
                PrintError("Degrees must be a number in range from 0 to 255!");
                return;
            }
            

            for (var i = 0; i < files.Count; i++)
            {
                try
                {
                    RotatePath(files[i]);
                }
                catch (PathTooLongException)
                {
                    PrintError($"Specified path: {files[i]} too long!");
                }
                catch (DirectoryNotFoundException)
                {
                    PrintError($"Directory: {files[i]} not found!");
                }
                catch (UnauthorizedAccessException)
                {
                    PrintError($"You don't have permission to this file: {files[i]}!");
                }
                catch (FileNotFoundException)
                {
                    PrintError($"File: {files[i]} not found!");
                }
            }
        }

        private static IEnumerable<string> ParseSettings(string[] arguments)
        {
            var files = new List<string>();
            for (var i = 0; i < arguments.Length; i++)
            {
                switch (arguments[i])
                {
                    case "-h":
                    case "--help":
                        PrintUsage();
                        throw new IncorrectPlaceForHelpException();

                    case "-d":
                    case "--degrees":
                        if (++i > arguments.Length)
                            throw new IndexOutOfRangeException();

                        rotationDegree = byte.Parse(arguments[i]);
                        continue;

                    case "-p":
                    case "--positive":
                        isPositiveRotation = true;
                        continue;

                    case "-n":
                    case "--negative":
                        isPositiveRotation = false;
                        continue;

                    default:
                        files.Add(arguments[i]);
                        continue;
                }
            }

            return files;
        }

        private static void RotatePath(string path)
        {
            if (!IsDirectory(path))
            {
                RotateFile(path);
                return;
            }

            var directories = Directory.EnumerateDirectories(path);
            var files = Directory.EnumerateFiles(path);

            foreach (var directory in directories)
            {
                RotatePath(directory);
            }

            foreach (var file in files)
            {
                RotateFile(file);
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

        private static bool IsDirectory(string path)
        {
            var attributes = File.GetAttributes(path);

            return (attributes & FileAttributes.Directory) != 0;
        }
    }
}