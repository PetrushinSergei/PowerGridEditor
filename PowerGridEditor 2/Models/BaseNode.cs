using System.Collections.Generic;

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

        public string IPAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "502";
        public string DeviceID { get; set; } = "1";
        public string Protocol { get; set; } = "Modbus TCP";
        public int MeasurementIntervalSeconds { get; set; } = 2;
        public double IncrementStep { get; set; } = 1;
        public int IncrementIntervalSeconds { get; set; } = 2;
        public Dictionary<string, string> ParamRegisters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> ParamAutoModes { get; set; } = new Dictionary<string, bool>();

        public BaseNode(int number)
        {
            Number = number;
            InitialVoltage = 525.0;

            string[] keys = { "U", "P", "Q", "Pg", "Qg", "Uf", "Qmin", "Qmax" };
            foreach (var key in keys)
            {
                ParamRegisters[key] = "0";
                ParamAutoModes[key] = false;
            }
        }

        public string ToFileString()
        {
            return $"{Code} {Number} {InitialVoltage} {NominalActivePower} {NominalReactivePower} {ActivePowerGeneration} {ReactivePowerGeneration} {FixedVoltageModule} {MinReactivePower} {MaxReactivePower}";
        }
    }
}
