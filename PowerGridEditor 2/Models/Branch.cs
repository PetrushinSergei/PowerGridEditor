namespace PowerGridEditor
{
    public class Branch
    {
        public string Code { get; } = "0301 0";
        public int StartNodeNumber { get; set; }
        public int EndNodeNumber { get; set; }
        public double ActiveResistance { get; set; }
        public double ReactiveResistance { get; set; }
        public double ReactiveConductivity { get; set; }
        public double TransformationRatio { get; set; }
        public double ActiveConductivity { get; set; }
        public int Zero1 { get; set; } = 0;
        public int Zero2 { get; set; } = 0;

        public Branch(int startNode, int endNode)
        {
            StartNodeNumber = startNode;
            EndNodeNumber = endNode;
        }

        public string ToFileString()
        {
            return $"{Code} {StartNodeNumber} {EndNodeNumber} {ActiveResistance:F1} {ReactiveResistance:F2} {ReactiveConductivity:F1} {TransformationRatio} {ActiveConductivity} {Zero1} {Zero2}";
        }
    }
}