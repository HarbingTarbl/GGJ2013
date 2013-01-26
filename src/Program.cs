using System;

namespace GGJ2013
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LiterallyHitler game = new LiterallyHitler())
            {
                game.Run();
            }
        }
    }
#endif
}

