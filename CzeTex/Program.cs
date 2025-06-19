namespace CzeTex
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                //Only input file with CzeTex text is provided
                Commander commander = new Commander(args[0]);
            }
            else if (args.Length == 2)
            {
                //Input file with CzeTex text and custom setup json file are provided
                Commander commander = new Commander(args[0], args[1]);
            }
            else
            {
                //User provided incorrect number of arguments
                throw new InvalidArgumentsException(); 
            }
        }
    }
}