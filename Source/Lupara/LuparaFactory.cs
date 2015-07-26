namespace Lupara
{
    public class LuparaFactory
    {
        public static ILuparaClient CreateClient()
        {
            return new LuparaClient();
        }

        public static ILuparaRequest CreateRequest()
        {
            return new LuparaRequest();
        }
    }
}