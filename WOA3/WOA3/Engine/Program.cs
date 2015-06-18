
namespace WOA3.Engine {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Renderer game = new Renderer())
            {
                game.Run();
            }
        }
    }
#endif
}

