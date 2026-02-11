using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        public void SetNetworkSummary(List<object> elements, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sbInput = new StringBuilder();
            var sbResults = new StringBuilder();
            var sbLoss = new StringBuilder();
            var sbBreakdown = new StringBuilder();

            var nodes = new List<(int Number, string Type, double U, double P, double Q)>();
            foreach (var el in elements)
            {
                if (el is GraphicNode n)
                {
                    nodes.Add((n.Data.Number, "PQ", n.Data.InitialVoltage, n.Data.NominalActivePower, n.Data.NominalReactivePower));
                }
                else if (el is GraphicBaseNode bn)
                {
                    nodes.Add((bn.Data.Number, "Slack", bn.Data.InitialVoltage, bn.Data.NominalActivePower, bn.Data.NominalReactivePower));
                }
            }
            nodes = nodes.OrderBy(n => n.Number).ToList();

            sbInput.AppendLine($"Число узлов n = {nodes.Count}      Число ветвей m = {branches.Count}");
            sbInput.AppendLine("Входные данные для расчета потокораспределения");
            sbInput.AppendLine();
            sbInput.AppendLine("Узел   Тип   Unom      P        Q");
            foreach (var n in nodes)
            {
                sbInput.AppendLine($"{n.Number,4}  {n.Type,-5} {n.U,6:F2} {n.P,8:F2} {n.Q,8:F2}");
            }
            sbInput.AppendLine();
            sbInput.AppendLine("Ветви сети");
            sbInput.AppendLine("N1    N2      r        x");
            foreach (var b in branches.OrderBy(x => x.Data.StartNodeNumber).ThenBy(x => x.Data.EndNodeNumber))
            {
                sbInput.AppendLine($"{b.Data.StartNodeNumber,4}  {b.Data.EndNodeNumber,4}  {b.Data.ActiveResistance,7:F3}  {b.Data.ReactiveResistance,7:F3}");
            }
            txtInput.Text = sbInput.ToString();

            sbResults.AppendLine("Результаты расчета по узлам (черновой просмотр)");
            sbResults.AppendLine("N       V       P       Q");
            foreach (var n in nodes)
            {
                sbResults.AppendLine($"{n.Number,4}  {n.U,7:F2}  {n.P,7:F2}  {n.Q,7:F2}");
            }
            sbResults.AppendLine();
            sbResults.AppendLine("Результаты расчета по ветвям");
            sbResults.AppendLine("N1    N2      r        x");
            foreach (var b in branches)
            {
                sbResults.AppendLine($"{b.Data.StartNodeNumber,4}  {b.Data.EndNodeNumber,4}  {b.Data.ActiveResistance,7:F3}  {b.Data.ReactiveResistance,7:F3}");
            }
            txtResults.Text = sbResults.ToString();

            double totalLoss = branches.Sum(b => Math.Abs(b.Data.ActiveResistance));
            sbLoss.AppendLine("Анализ потерь (предварительный) ");
            sbLoss.AppendLine($"Суммарный показатель потерь по r: {totalLoss:F3}");
            sbLoss.AppendLine($"Число шунтов: {shunts.Count}");
            txtLoss.Text = sbLoss.ToString();

            sbBreakdown.AppendLine("Составляющие потерь (предварительный просмотр)");
            sbBreakdown.AppendLine("Ветвь    r       x");
            foreach (var b in branches)
            {
                sbBreakdown.AppendLine($"{b.Data.StartNodeNumber,3}-{b.Data.EndNodeNumber,-3} {b.Data.ActiveResistance,7:F3} {b.Data.ReactiveResistance,7:F3}");
            }
            txtBreakdown.Text = sbBreakdown.ToString();
        }
    }
}
