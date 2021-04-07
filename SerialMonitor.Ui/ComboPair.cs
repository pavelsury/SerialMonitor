namespace SerialMonitor.Ui
{
    public class ComboPair
    {
        public object Value { get; set; }
        public string Text { get; set; }
        public override string ToString() => Text;
    }

    public class ComboPairInt
    {
        public object Value
        {
            get => _value;
            set => _value = int.Parse((string)value);
        }

        public string Text => _value.ToString();
        public override string ToString() => Text;

        private int _value;
    }
}
