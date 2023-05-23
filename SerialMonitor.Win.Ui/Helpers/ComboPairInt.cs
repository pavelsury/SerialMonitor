namespace SerialMonitor.Win.Ui.Helpers
{
    public class ComboPairInt : ComboPair
    {
        protected override void OnTextChanged()
        {
            Value = int.Parse(Text);
        }
    }
}
