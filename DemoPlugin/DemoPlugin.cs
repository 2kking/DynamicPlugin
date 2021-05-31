using Plugin;

namespace DemoPlugin
{
    public class DemoPlugin : IPlugin
    {
        public string Hello(string data)
        {
            return $"Hello:{data}";
        }
    }
}