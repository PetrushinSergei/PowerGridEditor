using System;
using System.Collections.Generic;

namespace PowerGridEditor
{
    public class Node
    {
        public int Number { get; set; }
        public double InitialVoltage { get; set; } = 500.0;
        public double NominalActivePower { get; set; }
        public double NominalReactivePower { get; set; }
        public double ActivePowerGeneration { get; set; }
        public double ReactivePowerGeneration { get; set; }
        public double FixedVoltageModule { get; set; }
        public double MinReactivePower { get; set; }
        public double MaxReactivePower { get; set; }

        public string IPAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "502";
        public string NodeID { get; set; } = "1";
        public string Protocol { get; set; } = "Modbus TCP";
        public bool IsRemote { get; set; } = false;

        public Dictionary<string, string> ParamRegisters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> ParamAutoModes { get; set; } = new Dictionary<string, bool>();

        // Сюда внешняя логика опроса (из вашего Program.cs) должна записывать результат
        public Dictionary<string, string> LiveValues { get; set; } = new Dictionary<string, string>();

        public Node(int number)
        {
            Number = number;
            string[] keys = { "Number", "U", "P", "Q", "Pg", "Qg", "Uf", "Qmin", "Qmax" };
            foreach (var key in keys)
            {
                ParamRegisters[key] = "0";
                ParamAutoModes[key] = false;
                LiveValues[key] = "---";
            }
        }
    }
}