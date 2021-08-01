using System.Threading.Tasks;

namespace SerialMonitor.Win.Business.Factories
{
    public static class FactoryBuilder
    {
        public static async Task InitializeAsync() => await ModelFactory.InitializeAsync();

        public static ModelFactory ModelFactory { get; } = new ModelFactory();
    }
}