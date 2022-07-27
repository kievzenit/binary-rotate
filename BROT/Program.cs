using BROT.Utils;

namespace BROT
{
    internal class Program
    {
        const string AUTHOR = "kievzenit";
        const string LICENSE = "MIT";
        const byte MAJOR_VERSION = 1;
        const byte MINOR_VERSION = 0;
        const byte PATCH_VERSION = 0;

        static void Main(string[] args)
        {
            var cmd = new CMD(
                args,
                MAJOR_VERSION,
                MINOR_VERSION,
                PATCH_VERSION,
                AUTHOR,
                LICENSE);

            cmd.Run();
        }
    }
}