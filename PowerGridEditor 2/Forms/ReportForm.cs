using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
            txtLoadCurrent.Text = BuildCurrentLoadingReport(result.LossesRez);
        }

        private string BuildCurrentLoadingReport(string lossesRez)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Расчет загрузки ветвей по току");
            sb.AppendLine("Формулы:");
            sb.AppendLine("Iр = sqrt(Iак^2 + Iре^2)");
            sb.AppendLine("Kзагр = Iр / Iдоп * 100%");
            sb.AppendLine();
            sb.AppendLine("Ветвь      Iак       Iре       Iр        Iдоп      Kзагр     Запас    Цвет      Смысл");

            var currents = ParseBranchCurrents(lossesRez);
            foreach (var branch in _branches.OrderBy(b => b.Data.StartNodeNumber).ThenBy(b => b.Data.EndNodeNumber))
            {
                var key = (branch.Data.StartNodeNumber, branch.Data.EndNodeNumber);
                if (!currents.TryGetValue(key, out var c) && !currents.TryGetValue((key.Item2, key.Item1), out c))
                {
                    sb.AppendLine($"{key.Item1,4}-{key.Item2,-4}  нет данных тока в losses.rez");
                    continue;
                }
            }

                double id = branch.Data.PermissibleCurrent <= 0 ? 600 : branch.Data.PermissibleCurrent;
                double ir = Math.Sqrt(c.Active * c.Active + c.Reactive * c.Reactive);
                double load = id > 0 ? ir / id * 100.0 : 0;
                double reserve = 100.0 - load;

                var meaning = GetLoadMeaning(load);
                sb.AppendLine(
                    $"{key.Item1,4}-{key.Item2,-4}  {c.Active,7:F3}  {c.Reactive,8:F3}  {ir,8:F3}  {id,8:F2}  {load,7:F2}%  {reserve,7:F2}%  {meaning.Color,-9} {meaning.Description}");
            }

            sb.AppendLine();
            sb.AppendLine("Шкала интерпретации:");
            sb.AppendLine("0% - 50%   : Синий    - Линия недогружена (холодная)");
            sb.AppendLine("50% - 80%  : Зеленый  - Оптимальный режим");
            sb.AppendLine("80% - 95%  : Желтый   - Внимание! Близко к пределу");
            sb.AppendLine("95% - 100% : Оранжевый- Предупреждение (МУН должен реагировать)");
            sb.AppendLine(">100%      : Красный  - ПЕРЕГРУЗКА! Риск повреждения провода");

            return sb.ToString();
        }

        private Dictionary<(int Start, int End), (double Active, double Reactive)> ParseBranchCurrents(string lossesRez)
        {
            var result = new Dictionary<(int Start, int End), (double Active, double Reactive)>();
            if (string.IsNullOrWhiteSpace(lossesRez))
            {
                return result;
            }

            bool inTable = false;
            var lines = lossesRez.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.StartsWith("Ветвь Нач.", StringComparison.OrdinalIgnoreCase))
                {
                    inTable = true;
                    continue;
                }

                if (!inTable || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!char.IsDigit(line[0]))
                {
                    if (line.StartsWith("Задающие токи узлов", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    continue;
                }

                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                {
                    continue;
                }

                if (!int.TryParse(parts[0], out int start) || !int.TryParse(parts[1], out int end))
                {
                    continue;
                }

                if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double ia))
                {
                    continue;
                }

                if (!double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double ir))
                {
                    continue;
                }

                result[(start, end)] = (ia, ir);
            }

            return result;
        }

        private (string Color, string Description) GetLoadMeaning(double load)
        {
            if (load <= 50) return ("Синий", "Линия недогружена (холодная)");
            if (load <= 80) return ("Зеленый", "Оптимальный режим");
            if (load <= 95) return ("Желтый", "Внимание! Близко к пределу");
            if (load <= 100) return ("Оранжевый", "Предупреждение (МУН должен реагировать)");
            return ("Красный", "ПЕРЕГРУЗКА! Риск повреждения провода");
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
