using System.Threading.Tasks;

namespace SerialMonitor.Win.Business.Factories
{
    public static class FactoryBuilder
    {
        public static async Task InitializeAsync(string settingsFilename = null) => await ModelFactory.InitializeAsync(settingsFilename);

        public static ModelFactory ModelFactory { get; } = new ModelFactory();
    }
}