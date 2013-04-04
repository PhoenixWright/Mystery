namespace Mystery
{
#if WINDOWS || XBOX
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      using(MysteryGame game = new MysteryGame()) {
        game.Run();
      }
    }
  }
#endif
}
