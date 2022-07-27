using BROT.Exceptions;

namespace BROT.Utils
{
    public class CMD
    {
        private readonly string[] Args;
        private readonly int MajorVersion;
        private readonly int MinorVersion;
        private readonly int PatchVersion;
        private readonly string Author;
        private readonly string License;

        private byte RotationDegree { get; set; }
        private bool IsPositiveRotation { get; set; }

        const byte DEFAULT_ROTATION_DEGREE = 13;


        public CMD(
            string[] args,
            int majorVersion,
            int minorVersion,
            int patchVersion,
            string author,
            string license)
        {
            this.Args = args;
            this.MajorVersion = majorVersion;
            this.MinorVersion = minorVersion;
            this.PatchVersion = patchVersion;
            this.Author = author;
            this.License = license;

            this.RotationDegree = DEFAULT_ROTATION_DEGREE;
            this.IsPositiveRotation = true;
        }

        public void Run()
        {
            switch (this.Args.Length)
            {
                case 0: this.PrintUsage(); break;
                case 1: this.OneInputArgumentHandler(); break;
                default: this.MultipleInputArgumentsHandler(); break;
            }
        }

        private void PrintUsage()
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

author: {this.Author}
license: {this.License}
version: {this.MajorVersion}.{this.MinorVersion}.{this.PatchVersion}
");
        }

        private void PrintError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        private bool CheckIfArgumentIsKey(string argument)
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

        private void OneInputArgumentHandler()
        {
            var argument = this.Args[0];

            if (argument == "-h" || argument == "--help")
            {
                this.PrintUsage();
                return;
            }

            if (this.CheckIfArgumentIsKey(argument))
            {
                this.PrintUsage();
                this.PrintError("When using one parameter it must be a filename!");

                return;
            }

            try
            {
                this.RotatePath(argument);
            }
            catch (PathTooLongException)
            {
                this.PrintError($"Specified path: {argument} too long!");
            }
            catch (DirectoryNotFoundException)
            {
                this.PrintError($"Directory: {argument} not found!");
            }
            catch (UnauthorizedAccessException)
            {
                this.PrintError($"You don't have permission to this file: {argument}!");
            }
            catch (FileNotFoundException)
            {
                this.PrintError($"File: {argument} not found!");
            }
        }

        private void MultipleInputArgumentsHandler()
        {
            var files = new List<string>();

            try
            {
                files.AddRange(this.ParseSettings());
            }
            catch (IncorrectPlaceForHelpException)
            {
                return;
            }
            catch (IndexOutOfRangeException)
            {
                this.PrintError("Degrees key was specifies, but degrees number wasn't!");
                return;
            }
            catch (FormatException)
            {
                this.PrintError("Degrees must be a number in range from 0 to 255!");
                return;
            }


            for (var i = 0; i < files.Count; i++)
            {
                try
                {
                    this.RotatePath(files[i]);
                }
                catch (PathTooLongException)
                {
                    this.PrintError($"Specified path: {files[i]} too long!");
                }
                catch (DirectoryNotFoundException)
                {
                    this.PrintError($"Directory: {files[i]} not found!");
                }
                catch (UnauthorizedAccessException)
                {
                    this.PrintError($"You don't have permission to this file: {files[i]}!");
                }
                catch (FileNotFoundException)
                {
                    this.PrintError($"File: {files[i]} not found!");
                }
            }
        }

        private IEnumerable<string> ParseSettings()
        {
            var files = new List<string>();
            for (var i = 0; i < this.Args.Length; i++)
            {
                switch (this.Args[i])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        throw new IncorrectPlaceForHelpException();

                    case "-d":
                    case "--degrees":
                        if (++i > this.Args.Length)
                            throw new IndexOutOfRangeException();

                        this.RotationDegree = byte.Parse(this.Args[i]);
                        continue;

                    case "-p":
                    case "--positive":
                        this.IsPositiveRotation = true;
                        continue;

                    case "-n":
                    case "--negative":
                        this.IsPositiveRotation = false;
                        continue;

                    default:
                        files.Add(this.Args[i]);
                        continue;
                }
            }

            return files;
        }

        private void RotatePath(string path)
        {
            if (!this.IsDirectory(path))
            {
                this.RotateFile(path);
                return;
            }

            var directories = Directory.EnumerateDirectories(path);
            var files = Directory.EnumerateFiles(path);

            foreach (var directory in directories)
            {
                this.RotatePath(directory);
            }

            foreach (var file in files)
            {
                this.RotateFile(file);
            }
        }

        private void RotateFile(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                var buffer = new byte[file.Length];
                var bytesRead = file.Read(buffer);

                file.Position = 0;
                file.Write(BROT.Rotate(buffer, this.RotationDegree, this.IsPositiveRotation));
            }
        }

        private bool IsDirectory(string path)
        {
            var attributes = File.GetAttributes(path);

            return (attributes & FileAttributes.Directory) != 0;
        }
    }
}
