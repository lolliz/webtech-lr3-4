namespace Lab_3_4.Events
{
    public class CarEvent
    {
        public string EventType { get; set; } = string.Empty;
        public CarData Car { get; set; } = new();
    }

    public class CarData
    {
        public string Firm { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Power { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}