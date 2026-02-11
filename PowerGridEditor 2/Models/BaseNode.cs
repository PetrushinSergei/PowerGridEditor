namespace PowerGridEditor
{
    public class BaseNode
    {
        public string Code { get; } = "0102 0";
        public int Number { get; set; }
        public double InitialVoltage { get; set; }
        public double NominalActivePower { get; set; }
        public double NominalReactivePower { get; set; }
        public double ActivePowerGeneration { get; set; }
        public double ReactivePowerGeneration { get; set; }
        public double FixedVoltageModule { get; set; }
        public double MinReactivePower { get; set; }
        public double MaxReactivePower { get; set; }

        public BaseNode(int number)
        {
            Number = number;
            InitialVoltage = 525.0;
        }

        public string ToFileString()
        {
            return $"{Code} {Number} {InitialVoltage} {NominalActivePower} {NominalReactivePower} {ActivePowerGeneration} {ReactivePowerGeneration} {FixedVoltageModule} {MinReactivePower} {MaxReactivePower}";
        }
    }
}