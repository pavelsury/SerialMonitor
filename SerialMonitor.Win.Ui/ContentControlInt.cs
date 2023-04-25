using System.Windows;
using System.Windows.Controls;

namespace SerialMonitor.Win.Ui
{
    public class ContentControlInt : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            Tag = int.Parse((string)Content);
        }
    }
}
