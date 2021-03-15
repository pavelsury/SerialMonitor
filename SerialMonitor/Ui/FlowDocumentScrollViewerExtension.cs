using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SerialMonitor.Ui
{
    public static class FlowDocumentScrollViewerExtension
    {
        public static void Clear(this FlowDocumentScrollViewer flowDocumentScrollViewer)
        {
            flowDocumentScrollViewer.Document.Blocks.Clear();
        }

        public static void AppendText(this FlowDocumentScrollViewer flowDocumentScrollViewer, string data, int fontSize, FontStyle fontStyle)
        {
            AppendText(flowDocumentScrollViewer, data, flowDocumentScrollViewer.FindResource(Microsoft.VisualStudio.PlatformUI.CommonControlsColors.TextBoxTextBrushKey) as SolidColorBrush, fontSize, fontStyle);
        }

        public static void AppendText(this FlowDocumentScrollViewer flowDocumentScrollViewer, string data, SolidColorBrush brush, int fontSize, FontStyle fontStyle)
        {
            var range = new TextRange(flowDocumentScrollViewer.Document.ContentEnd.DocumentEnd, flowDocumentScrollViewer.Document.ContentEnd.DocumentEnd);
            range.Text = data.Replace(Environment.NewLine, "\r");
            range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
            range.ApplyPropertyValue(TextElement.FontSizeProperty, (double)fontSize);
        }

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