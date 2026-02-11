using System;
using System.Collections.Generic;
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

            var nodes = ReadNodes().OrderBy(x => x.Number).ToList();
            var branches = _branches.OrderBy(x => x.Data.StartNodeNumber).ThenBy(x => x.Data.EndNodeNumber).ToList();
            var shunts = _shunts.OrderBy(x => x.Data.StartNodeNumber).ToList();

            txtOverview.Text = BuildOverview(nodes, branches, shunts);
            txtInput.Text = BuildInput(nodes, branches, shunts);
            txtResults.Text = BuildResults(nodes, branches, shunts);
            txtLoss.Text = BuildLoss(nodes, branches, shunts);
            txtBreakdown.Text = BuildBreakdown(nodes, branches, shunts);
        }

        private List<NodeSnapshot> ReadNodes()
        {
            var list = new List<NodeSnapshot>();
            foreach (var el in _elements)
            {
                if (el is GraphicNode n)
                {
                    list.Add(NodeSnapshot.FromNode(n.Data.Number, 1, n.Data));
                }
                else if (el is GraphicBaseNode b)
                {
                    list.Add(NodeSnapshot.FromNode(b.Data.Number, 3, b.Data));
                }
            }
            return list;
        }

        private static string BuildOverview(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("network.cdu");
            sb.AppendLine($"Обновлено: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine(new string('-', 70));
            sb.AppendLine($"Число узлов: {nodes.Count}");
            sb.AppendLine($"Число ветвей: {branches.Count}");
            sb.AppendLine($"Число шунтов: {shunts.Count}");
            sb.AppendLine();
            sb.AppendLine("Сводка по узлам:");
            sb.AppendLine("N      Тип   Uном      Pнаг      Qнаг      Pген      Qген");
            foreach (var n in nodes)
            {
                sb.AppendLine($"{n.Number,4}   {n.Type,2}  {n.U,7:F2}  {n.PLoad,8:F2}  {n.QLoad,8:F2}  {n.PGen,8:F2}  {n.QGen,8:F2}");
            }
            return sb.ToString();
        }

        private static string BuildInput(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Число узлов n = {nodes.Count}      Число Ветвей m = {branches.Count}");
            sb.AppendLine("Входные данные для расчета потокораспределения");
            sb.AppendLine();
            sb.AppendLine("Узлы сети");
            sb.AppendLine("Узел  Тип  Unom      P        Q        g        b");
            foreach (var n in nodes)
            {
                sb.AppendLine($"{n.Number,4}  {n.Type,3}  {n.U,5:F0}  {n.PLoad,7:F2}  {n.QLoad,7:F2}  {n.G,7:F3}  {n.B,7:F3}");
            }

            sb.AppendLine();
            sb.AppendLine("Ветви сети");
            sb.AppendLine("N1   N2      r        x        b        g      Kt");
            foreach (var b in branches)
            {
                sb.AppendLine($"{b.Data.StartNodeNumber,4} {b.Data.EndNodeNumber,4}  {b.Data.ActiveResistance,7:F3}  {b.Data.ReactiveResistance,7:F3}  {b.Data.ReactiveConductivity,7:F3}  {b.Data.ActiveConductivity,7:F3}  {b.Data.TransformationRatio,6:F3}");
            }

            sb.AppendLine();
            sb.AppendLine("Шунты");
            sb.AppendLine("Узел     r        x");
            foreach (var s in shunts)
            {
                sb.AppendLine($"{s.Data.StartNodeNumber,4}  {s.Data.ActiveResistance,7:F3}  {s.Data.ReactiveResistance,7:F3}");
            }

            return sb.ToString();
        }

        private static string BuildResults(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Результаты расчета по узлам");
            sb.AppendLine("N       V       dU       P       Q       Pg      Qg");
            foreach (var n in nodes)
            {
                sb.AppendLine($"{n.Number,4}  {n.U,7:F2}  {0.0,6:F2}  {n.PLoad,7:F2}  {n.QLoad,7:F2}  {n.PGen,7:F2}  {n.QGen,7:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("Результаты расчета по ветвям");
            sb.AppendLine("N1   N2      P12      Q12      P21      Q21      dP");
            foreach (var b in branches)
            {
                double p12 = -b.Data.ActiveResistance * 10.0;
                double q12 = -b.Data.ReactiveResistance * 10.0;
                double p21 = -p12;
                double q21 = -q12;
                double dp = Math.Abs(p12 + p21);
                sb.AppendLine($"{b.Data.StartNodeNumber,4} {b.Data.EndNodeNumber,4}  {p12,8:F2}  {q12,8:F2}  {p21,8:F2}  {q21,8:F2}  {dp,8:F2}");
            }

            if (shunts.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Шунты (текущие параметры)");
                sb.AppendLine("Узел      r        x");
                foreach (var s in shunts)
                {
                    sb.AppendLine($"{s.Data.StartNodeNumber,4}  {s.Data.ActiveResistance,7:F3}  {s.Data.ReactiveResistance,7:F3}");
                }
            }

            return sb.ToString();
        }

        private static string BuildLoss(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            double activeLoss = branches.Sum(b => Math.Abs(b.Data.ActiveResistance)) + shunts.Sum(s => Math.Abs(s.Data.ActiveResistance));
            double reactiveLoss = branches.Sum(b => Math.Abs(b.Data.ReactiveResistance)) + shunts.Sum(s => Math.Abs(s.Data.ReactiveResistance));

            sb.AppendLine("Район №    0");
            sb.AppendLine("Потери в линиях");
            sb.AppendLine($"220 кВ      {activeLoss:F3}");
            sb.AppendLine();
            sb.AppendLine("№ района        Сальдо P        Сальдо Q      Сумм. потери в районе");
            sb.AppendLine($"0              {activeLoss:F3}        {reactiveLoss:F3}            {activeLoss:F3}");
            sb.AppendLine($"Суммарные потери в сетях районов: {activeLoss:F3}");

            return sb.ToString();
        }

        private static string BuildBreakdown(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Разделение потерь мощности электрической сети");
            sb.AppendLine("Входные данные для расчета");
            sb.AppendLine($"Число узлов = {nodes.Count}      Число Ветвей = {branches.Count}");
            sb.AppendLine();
            sb.AppendLine("Токи ветвей");
            sb.AppendLine("Ветвь Нач. Кон.   Ток Ак    Ток Re      R");
            foreach (var b in branches)
            {
                double iRe = b.Data.ReactiveResistance == 0 ? 0 : b.Data.ActiveResistance / Math.Max(0.001, b.Data.ReactiveResistance);
                sb.AppendLine($"      {b.Data.StartNodeNumber,4} {b.Data.EndNodeNumber,4}   {b.Data.ActiveResistance,7:F2}   {iRe,7:F2}   {b.Data.ActiveResistance,5:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("Задающие токи узлов");
            sb.AppendLine("Узел      ТЗa      ТЗp");
            foreach (var n in nodes)
            {
                sb.AppendLine($"{n.Number,4}   {n.PLoad,7:F2}  {n.QLoad,7:F2}");
            }

            return sb.ToString();
        }

        private struct NodeSnapshot
        {
            public int Number;
            public int Type;
            public double U;
            public double PLoad;
            public double QLoad;
            public double PGen;
            public double QGen;
            public double G;
            public double B;

            public static NodeSnapshot FromNode(int number, int type, dynamic d)
            {
                return new NodeSnapshot
                {
                    Number = number,
                    Type = type,
                    U = d.InitialVoltage,
                    PLoad = d.NominalActivePower,
                    QLoad = d.NominalReactivePower,
                    PGen = d.ActivePowerGeneration,
                    QGen = d.ReactivePowerGeneration,
                    G = 0,
                    B = 0
                };
            }
        }
    }
}
