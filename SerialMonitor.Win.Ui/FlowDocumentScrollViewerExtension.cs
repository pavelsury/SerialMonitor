using System.Windows.Controls;
using System.Windows.Media;

namespace SerialMonitor.Win.Ui
{
    public static class FlowDocumentScrollViewerExtension
    {
        public static void ScrollToEnd(this FlowDocumentScrollViewer flowDocumentScrollViewer)
        {
            if (VisualTreeHelper.GetChildrenCount(flowDocumentScrollViewer) == 0)
            {
                return;
            }

            var firstChild = VisualTreeHelper.GetChild(flowDocumentScrollViewer, 0);
            if (!(VisualTreeHelper.GetChild(firstChild, 0) is Decorator border))
            {
                return;
            }

            (border.Child as ScrollViewer)?.ScrollToEnd();
        }
    }
}