using System.Threading.Tasks;
using SerialMonitor.Win.Business.Factories;

namespace SerialMonitor.Win.Ui.Factories
{
    public static class UiFactoryBuilder
    {
        public static Task InitializeAsync()
        {
            UiFactory = new UiFactory(BusinessFactoryBuilder.ModelFactory);
            return Task.CompletedTask;
        }

        public static UiFactory UiFactory { get; private set; }
    }
}