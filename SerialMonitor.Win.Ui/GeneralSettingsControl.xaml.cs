using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using SerialMonitor.Business.Data;
using SerialMonitor.Win.Ui.Helpers;

namespace SerialMonitor.Win.Ui
{
    public partial class GeneralSettingsControl : UserControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
        }

        public List<ComboPair> FontSizes { get; } = Enumerable
            .Range(AppSettings.DefaultFontSizeMin, AppSettings.DefaultFontSizeMax - AppSettings.DefaultFontSizeMin + 1)
            .Select(i => new ComboPair
            {
                Value = i,
                Text = i.ToString()
            })
            .ToList();
    }
}
