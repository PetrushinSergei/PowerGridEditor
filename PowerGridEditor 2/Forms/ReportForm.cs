using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class ReportForm : Form
    {
        private List<object> _elements;
        private List<GraphicBranch> _branches;
        private List<GraphicShunt> _shunts;
        private readonly Timer refreshTimer;

        public ReportForm()
        {
            InitializeComponent();
            refreshTimer = new Timer { Interval = 1000 };
            refreshTimer.Tick += (s, e) => RefreshReport();
            refreshTimer.Start();
            FormClosing += (s, e) => refreshTimer.Stop();
        }

        public void SetNetworkSummary(List<object> elements, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            _elements = elements;
            _branches = branches;
            _shunts = shunts;
            RefreshReport();
        }

        private void RefreshReport()
        {
            if (_elements == null || _branches == null || _shunts == null)
            {
                return;
            }

            var cduLines = BuildCduLines();
            var engine = new ConsoleApplicationEngine(cduLines, CalculationOptions.Precision, CalculationOptions.MaxIterations);
            var result = engine.Run();

            txtOverview.Text = result.NetworkCdu;
            txtErr.Text = result.NetworkErr;
            txtInput.Text = result.NetworkOut;
            txtResults.Text = result.NetworkRez;
            txtLoss.Text = result.NetworkRip;
            txtBreakdown.Text = result.LossesRez;
        }

        private List<string> BuildCduLines()
        {
            var lines = new List<string>();

            var baseNodeNumbers = new HashSet<int>();
            foreach (var element in _elements)
            {
                var baseNode = element as GraphicBaseNode;
                if (baseNode != null)
                {
                    baseNodeNumbers.Add(baseNode.Data.Number);
                }
            }

            foreach (var element in _elements)
            {
                var node = element as GraphicNode;
                if (node == null || baseNodeNumbers.Contains(node.Data.Number))
                {
                    continue;
                }

                string line = $"0201 0   {node.Data.Number,3}  {node.Data.InitialVoltage,3}     " +
                              $"{FormatInt(node.Data.NominalActivePower),4}  {FormatInt(node.Data.NominalReactivePower),3}  " +
                              $"{FormatInt(node.Data.ActivePowerGeneration),1} {FormatInt(node.Data.ReactivePowerGeneration),1}  " +
                              $"{FormatInt(node.Data.FixedVoltageModule),3} {FormatInt(node.Data.MinReactivePower),1} {FormatInt(node.Data.MaxReactivePower),1}";
                lines.Add(line);
            }

            foreach (var element in _elements)
            {
                var baseNode = element as GraphicBaseNode;
                if (baseNode == null)
                {
                    continue;
                }

                string line = $"0102 0   {baseNode.Data.Number,3}  {baseNode.Data.InitialVoltage,3}       " +
                              $"{FormatInt(baseNode.Data.NominalActivePower),1}    " +
                              $"{FormatInt(baseNode.Data.NominalReactivePower),1}  " +
                              $"{FormatInt(baseNode.Data.ActivePowerGeneration),1} {FormatInt(baseNode.Data.ReactivePowerGeneration),1}   " +
                              $"{FormatInt(baseNode.Data.FixedVoltageModule),1} " +
                              $"{FormatInt(baseNode.Data.MinReactivePower),1} " +
                              $"{FormatInt(baseNode.Data.MaxReactivePower),1}";
                lines.Add(line);
            }

            foreach (var shunt in _shunts)
            {
                string shuntLine = $"0301 0   {shunt.Data.StartNodeNumber,3}      {shunt.Data.EndNodeNumber,2}    " +
                                   $"{FormatDouble(shunt.Data.ActiveResistance),4}   " +
                                   $"{FormatDouble(shunt.Data.ReactiveResistance),5}";
                lines.Add(shuntLine);
            }

            foreach (var branch in _branches)
            {
                string branchLine = $"0301 0   {branch.Data.StartNodeNumber,3}      {branch.Data.EndNodeNumber,2}    " +
                                    $"{FormatDouble(branch.Data.ActiveResistance),4}   " +
                                    $"{FormatDouble(branch.Data.ReactiveResistance),5}   " +
                                    $"{FormatDouble(branch.Data.ReactiveConductivity, true),6}     " +
                                    $"{FormatDouble(branch.Data.TransformationRatio),5} " +
                                    $"{FormatInt(branch.Data.ActiveConductivity),1} 0 0";
                lines.Add(branchLine);
            }

            return lines;
        }

        private string FormatInt(double number)
        {
            if (number == 0)
            {
                return "0";
            }

            return ((int)number).ToString(CultureInfo.InvariantCulture);
        }

        private string FormatDouble(double number, bool isConductivity = false)
        {
            if (number == 0)
            {
                return "0";
            }

            if (isConductivity && number < 0)
            {
                return $"-{Math.Abs(number).ToString("F1", CultureInfo.InvariantCulture)}";
            }

            string formatted = number.ToString("F2", CultureInfo.InvariantCulture);
            if (formatted.StartsWith("0."))
            {
                formatted = "." + formatted.Substring(2);
            }

            return formatted;
        }
    }
}
