using System.Threading.Tasks;

namespace SerialMonitor.Win.Business.Factories
{
    public static class BusinessFactoryBuilder
    {
        public static async Task InitializeAsync(string settingsFilename = null, string selectedPort = null) => await ModelFactory.InitializeAsync(settingsFilename, selectedPort);

        public static ModelFactory ModelFactory { get; } = new ModelFactory();
    }
}