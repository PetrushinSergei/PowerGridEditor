namespace PowerGridEditor
{
    public class Shunt
    {
        public string Code { get; } = "0301 0";
        public int StartNodeNumber { get; set; }
        public int EndNodeNumber { get; set; } = 0; // Всегда 0 для шунта
        public double ActiveResistance { get; set; }
        public double ReactiveResistance { get; set; }

        public Shunt(int startNode)
        {
            StartNodeNumber = startNode;
            EndNodeNumber = 0; // Фиксированное значение
        }

        public string ToFileString()
        {
            return $"{Code} {StartNodeNumber} {EndNodeNumber} {ActiveResistance:F1} {ReactiveResistance}";
        }
    }
}