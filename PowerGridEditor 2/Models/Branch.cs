using System.Collections.Generic;

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

        public string IPAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "502";
        public string DeviceID { get; set; } = "1";
        public string Protocol { get; set; } = "Modbus TCP";
        public Dictionary<string, string> ParamRegisters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> ParamAutoModes { get; set; } = new Dictionary<string, bool>();

        public Branch(int startNode, int endNode)
        {
            StartNodeNumber = startNode;
            EndNodeNumber = endNode;

            string[] keys = { "R", "X", "B", "Ktr", "G" };
            foreach (var key in keys)
            {
                ParamRegisters[key] = "0";
                ParamAutoModes[key] = false;
            }
        }

        public string ToFileString()
        {
            return $"{Code} {StartNodeNumber} {EndNodeNumber} {ActiveResistance:F1} {ReactiveResistance:F2} {ReactiveConductivity:F1} {TransformationRatio} {ActiveConductivity} {Zero1} {Zero2}";
        }
    }
}
