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
            var solution = LoadFlowSolver.Solve(nodes, branches, shunts);

            txtOverview.Text = BuildOverview(nodes, branches, shunts);
            txtInput.Text = BuildInput(nodes, branches, shunts);
            txtResults.Text = BuildResults(solution);
            txtLoss.Text = BuildLoss(solution);
            txtBreakdown.Text = BuildBreakdown(solution);
        }

        private List<NodeSnapshot> ReadNodes()
        {
            var list = new List<NodeSnapshot>();
            foreach (var el in _elements)
            {
                if (el is GraphicNode n)
                {
                    int type = n.Data.FixedVoltageModule > 0.1 ? 2 : 1;
                    list.Add(NodeSnapshot.FromNode(n.Data.Number, type, n.Data));
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

        private static string BuildResults(LoadFlowSolution solution)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Параметры расчёта: eps={CalculationOptions.Precision:G6}, maxIter={CalculationOptions.MaxIterations}");
            sb.AppendLine($"Итог итераций: {solution.Iterations}, невязка={solution.Mismatch:G6}, сходимость={(solution.Converged ? "Да" : "Нет")}");
            sb.AppendLine();
            sb.AppendLine("Результаты расчета по узлам");
            sb.AppendLine("N       V       dU       P       Q       Pg      Qg");
            foreach (var n in solution.Nodes)
            {
                sb.AppendLine($"{n.Number,4}  {n.Voltage,7:F2}  {n.Angle,6:F2}  {-n.P,7:F2}  {-n.Q,7:F2}  {n.Pg,7:F2}  {n.Qb,7:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("Результаты расчета по ветвям");
            sb.AppendLine("N1   N2      P12      Q12      P21      Q21      dP");
            foreach (var b in solution.Branches)
            {
                sb.AppendLine($"{b.StartNode,4} {b.EndNode,4}  {-b.P12,8:F2}  {-b.Q12,8:F2}  {b.P21,8:F2}  {b.Q21,8:F2}  {b.DeltaP,8:F2}");
            }

            return sb.ToString();
        }

        private static string BuildLoss(LoadFlowSolution solution)
        {
            var sb = new StringBuilder();
            double activeLoss = solution.Branches.Sum(x => x.DeltaP);
            double reactiveLoss = solution.Branches.Sum(x => Math.Abs(x.Q12 + x.Q21));

            sb.AppendLine("Район №    0");
            sb.AppendLine("Потери в линиях");
            sb.AppendLine($"220 кВ      {activeLoss:F3}");
            sb.AppendLine();
            sb.AppendLine("№ района        Сальдо P        Сальдо Q      Сумм. потери в районе");
            sb.AppendLine($"0              {activeLoss:F3}        {reactiveLoss:F3}            {activeLoss:F3}");
            sb.AppendLine($"Суммарные потери в сетях районов: {activeLoss:F3}");

            return sb.ToString();
        }

        private static string BuildBreakdown(LoadFlowSolution solution)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Разделение потерь мощности электрической сети");
            sb.AppendLine("Входные данные для расчета");
            sb.AppendLine($"Число узлов = {solution.Nodes.Count}      Число Ветвей = {solution.Branches.Count}");
            sb.AppendLine();
            sb.AppendLine("Токи ветвей");
            sb.AppendLine("Ветвь Нач. Кон.   Ток Ак    Ток Re      R");
            foreach (var b in solution.Branches)
            {
                sb.AppendLine($"      {b.StartNode,4} {b.EndNode,4}   {b.Ia,7:F2}   {b.Ir,7:F2}   {b.R,5:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("Задающие токи узлов");
            sb.AppendLine("Узел      ТЗa      ТЗp");
            foreach (var n in solution.Nodes)
            {
                sb.AppendLine($"{n.Number,4}   {n.P,7:F2}  {n.Q,7:F2}");
            }

            return sb.ToString();
        }

        private sealed class LoadFlowSolver
        {
            public static LoadFlowSolution Solve(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
            {
                var solution = new LoadFlowSolution();
                if (nodes.Count == 0)
                {
                    return solution;
                }

                var orderedNodes = new List<NodeSnapshot>(nodes.Count);
                var slack = nodes.FirstOrDefault(x => x.Type == 3);
                if (slack.Number != 0) orderedNodes.Add(slack);
                orderedNodes.AddRange(nodes.Where(x => x.Number != slack.Number));
                if (orderedNodes.Count == 0)
                {
                    orderedNodes.Add(nodes[0]);
                    orderedNodes[0] = new NodeSnapshot { Number = nodes[0].Number, Type = 3, U = nodes[0].U, PLoad = nodes[0].PLoad, QLoad = nodes[0].QLoad, PGen = nodes[0].PGen, QGen = nodes[0].QGen, G = nodes[0].G, B = nodes[0].B };
                }
                if (orderedNodes[0].Type != 3)
                {
                    orderedNodes[0] = new NodeSnapshot { Number = orderedNodes[0].Number, Type = 3, U = orderedNodes[0].U, PLoad = orderedNodes[0].PLoad, QLoad = orderedNodes[0].QLoad, PGen = orderedNodes[0].PGen, QGen = orderedNodes[0].QGen, G = orderedNodes[0].G, B = orderedNodes[0].B };
                }

                int nodeCount = orderedNodes.Count;
                int n = nodeCount - 1;
                int m = branches.Count;
                int dim = 2 * n + 1;

                var indexByNumber = new Dictionary<int, int>();
                for (int i = 0; i < nodeCount; i++)
                {
                    indexByNumber[orderedNodes[i].Number] = i;
                }

                var nn = new int[nodeCount];
                var nk = new int[nodeCount];
                var unom = new double[nodeCount];
                var p0 = new double[nodeCount];
                var q0 = new double[nodeCount];
                var g = new double[nodeCount];
                var b = new double[nodeCount];
                for (int i = 0; i < nodeCount; i++)
                {
                    var node = orderedNodes[i];
                    nn[i] = node.Number;
                    nk[i] = node.Type;
                    unom[i] = Math.Abs(node.U) > 0.001 ? node.U : 1.0;
                    p0[i] = node.PLoad - node.PGen;
                    q0[i] = node.QLoad - node.QGen;
                    g[i] = node.G;
                    b[i] = node.B;
                }

                foreach (var shunt in shunts)
                {
                    int idx;
                    if (!indexByNumber.TryGetValue(shunt.Data.StartNodeNumber, out idx))
                    {
                        continue;
                    }

                    double r = shunt.Data.ActiveResistance;
                    double x = shunt.Data.ReactiveResistance;
                    double c = r * r + x * x;
                    if (c < 1e-12)
                    {
                        continue;
                    }

                    g[idx] += r / c;
                    b[idx] += -x / c;
                }

                var nm1 = new int[2, m + 1];
                var gr = new double[m + 1];
                var bx = new double[m + 1];
                var gy = new double[m + 1];
                var by = new double[m + 1];
                var kt = new double[m + 1];
                var rBranch = new double[m + 1];

                for (int j = 1; j <= m; j++)
                {
                    var br = branches[j - 1].Data;
                    int i1;
                    int i2;
                    if (!indexByNumber.TryGetValue(br.StartNodeNumber, out i1) || !indexByNumber.TryGetValue(br.EndNodeNumber, out i2))
                    {
                        continue;
                    }

                    nm1[0, j] = i1;
                    nm1[1, j] = i2;
                    rBranch[j] = br.ActiveResistance;
                    double xBranch = Math.Abs(br.ReactiveResistance) < 1.001 ? 1.01 : br.ReactiveResistance;
                    double c = rBranch[j] * rBranch[j] + xBranch * xBranch;
                    gr[j] = rBranch[j] / c;
                    bx[j] = -xBranch / c;
                    gy[j] = br.ActiveConductivity * 1e-6;
                    by[j] = -br.ReactiveConductivity * 1e-6;
                    kt[j] = br.TransformationRatio < 0.001 ? 1.0 : br.TransformationRatio;
                }

                var va = new double[nodeCount];
                var vr = new double[nodeCount];
                var gg = new double[nodeCount];
                var bb = new double[nodeCount];
                var ja = new double[nodeCount];
                var jr = new double[nodeCount];
                var p = new double[nodeCount];
                var q = new double[nodeCount];
                var ds = new double[dim + 1];
                var a = new double[dim + 1, dim + 1];

                for (int i = 0; i < nodeCount; i++)
                {
                    va[i] = unom[i];
                    vr[i] = 0;
                    gg[i] = g[i] * 1e-6;
                    bb[i] = b[i] * 1e-6;
                }

                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[0, j];
                    int i2 = nm1[1, j];
                    gg[i1] += gr[j] / (kt[j] * kt[j]) + gy[j] / 2.0;
                    bb[i1] += bx[j] / (kt[j] * kt[j]) + by[j] / 2.0;
                    gg[i2] += gr[j] + gy[j] / 2.0;
                    bb[i2] += bx[j] + by[j] / 2.0;
                }

                Func(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, ja, jr, p, q, ds, p0, q0, unom, ref solution.Mismatch);

                int count = 0;
                while (solution.Mismatch > CalculationOptions.Precision && count < CalculationOptions.MaxIterations)
                {
                    Jacoby(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, ja, jr, a, unom);
                    Gauss(n, a, ds, va, vr);
                    count++;
                    Func(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, ja, jr, p, q, ds, p0, q0, unom, ref solution.Mismatch);
                }

                solution.Iterations = count;
                solution.Converged = solution.Mismatch <= CalculationOptions.Precision;

                for (int i = 0; i < nodeCount; i++)
                {
                    double mv = va[i] * va[i] + vr[i] * vr[i];
                    solution.Nodes.Add(new SolvedNode
                    {
                        Number = nn[i],
                        Voltage = Math.Sqrt(mv),
                        Angle = Math.Atan2(vr[i], va[i]) * 57.295779515,
                        P = p[i],
                        Q = q[i],
                        Pg = mv * (g[i] * 1e-6),
                        Qb = -mv * (b[i] * 1e-6)
                    });
                }

                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[0, j];
                    int i2 = nm1[1, j];

                    double i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / (kt[j] * kt[j]) - (va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
                    double i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / (kt[j] * kt[j]) - (va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
                    double i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]);
                    double i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]);

                    double mv1 = va[i1] * va[i1] + vr[i1] * vr[i1];
                    double p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv1 / 2.0;
                    double q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv1 / 2.0;

                    double mv2 = va[i2] * va[i2] + vr[i2] * vr[i2];
                    double p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv2 / 2.0;
                    double q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv2 / 2.0;

                    solution.Branches.Add(new SolvedBranch
                    {
                        StartNode = nn[i1],
                        EndNode = nn[i2],
                        P12 = p12,
                        Q12 = q12,
                        P21 = p21,
                        Q21 = q21,
                        DeltaP = Math.Abs(p21 - p12),
                        Ia = i1a,
                        Ir = i1r,
                        R = Math.Abs(rBranch[j])
                    });
                }

                return solution;
            }

            private static void Func(int n, int m, int[] nk, int[,] nm1, double[] kt, double[] gr, double[] bx, double[] gg, double[] bb, double[] va, double[] vr, double[] ja, double[] jr, double[] p, double[] q, double[] ds, double[] p0, double[] q0, double[] unom, ref double mismatch)
            {
                double w = 0;

                for (int i = 0; i <= n; i++)
                {
                    ja[i] = gg[i] * va[i] - bb[i] * vr[i];
                    jr[i] = bb[i] * va[i] + gg[i] * vr[i];
                }

                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[0, j];
                    int i2 = nm1[1, j];
                    ja[i1] -= (gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
                    jr[i1] -= (bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                    ja[i2] -= (gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
                    jr[i2] -= (bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                }

                for (int i = 0; i <= n; i++)
                {
                    p[i] = va[i] * ja[i] + vr[i] * jr[i];
                    q[i] = vr[i] * ja[i] - va[i] * jr[i];
                }

                for (int i = 1; i <= n; i++)
                {
                    ds[2 * i] = -(p[i] + p0[i]);
                    double h;
                    if (nk[i] == 1)
                    {
                        ds[2 * i - 1] = -(q[i] + q0[i]);
                        h = Math.Abs(q0[i]) < 1 ? ds[2 * i - 1] : ds[2 * i - 1] / q0[i];
                    }
                    else
                    {
                        ds[2 * i - 1] = -(va[i] * va[i] + vr[i] * vr[i] - unom[i] * unom[i]) / unom[i];
                        h = ds[2 * i - 1] / unom[i];
                    }

                    double pp = Math.Abs(p0[i]) < 1 ? ds[2 * i] : ds[2 * i] / p0[i];
                    w += pp * pp + h * h;
                }

                mismatch = Math.Sqrt(w / (2 * Math.Max(n, 1)));
            }

            private static void Jacoby(int n, int m, int[] nk, int[,] nm1, double[] kt, double[] gr, double[] bx, double[] gg, double[] bb, double[] va, double[] vr, double[] ja, double[] jr, double[,] a, double[] unom)
            {
                Array.Clear(a, 0, a.Length);

                for (int i = 1; i <= n; i++)
                {
                    if (nk[i] == 1)
                    {
                        a[2 * i - 1, 2 * i - 1] = -bb[i] * va[i] + gg[i] * vr[i] - jr[i];
                        a[2 * i - 1, 2 * i] = -gg[i] * va[i] - bb[i] * vr[i] + ja[i];
                    }
                    else
                    {
                        a[2 * i - 1, 2 * i - 1] = 2 * va[i] / unom[i];
                        a[2 * i - 1, 2 * i] = 2 * vr[i] / unom[i];
                    }
                    a[2 * i, 2 * i - 1] = gg[i] * va[i] + bb[i] * vr[i] + ja[i];
                    a[2 * i, 2 * i] = -bb[i] * va[i] + gg[i] * vr[i] + jr[i];
                }

                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[0, j];
                    int i2 = nm1[1, j];
                    if (i1 == 0 || i2 == 0) continue;

                    int j1 = 2 * i1 - 1;
                    int j2 = 2 * i1;
                    int j3 = 2 * i2 - 1;
                    int j4 = 2 * i2;
                    if (nk[i1] == 1)
                    {
                        a[j1, j3] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                        a[j1, j4] = -(-gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
                    }
                    a[j2, j3] = -(gr[j] * va[i1] + bx[j] * vr[i1]) / kt[j];
                    a[j2, j4] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];

                    if (nk[i2] == 1)
                    {
                        a[j3, j1] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                        a[j3, j2] = -(-gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
                    }
                    a[j4, j1] = -(gr[j] * va[i2] + bx[j] * vr[i2]) / kt[j];
                    a[j4, j2] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                }
            }

            private static void Gauss(int n, double[,] a, double[] ds, double[] va, double[] vr)
            {
                var u = new double[2 * n + 1];

                for (int i = 1; i <= 2 * n - 1; i++)
                {
                    double c = Math.Abs(a[i, i]) < 1e-7 ? 1e-7 : a[i, i];
                    for (int j = i + 1; j <= 2 * n; j++)
                    {
                        if (Math.Abs(a[i, j]) > 1e-7) a[i, j] /= c;
                    }

                    ds[i] /= c;

                    for (int k1 = i + 1; k1 <= 2 * n; k1++)
                    {
                        double d = a[k1, i];
                        if (Math.Abs(d) <= 1e-7) continue;
                        for (int l = i + 1; l <= 2 * n; l++)
                        {
                            if (Math.Abs(a[i, l]) > 1e-7) a[k1, l] -= a[i, l] * d;
                        }
                        ds[k1] -= ds[i] * d;
                    }
                }

                u[2 * n] = ds[2 * n] / a[2 * n, 2 * n];
                for (int k1 = 2 * n - 1; k1 >= 1; k1--)
                {
                    double s = 0;
                    for (int j = k1 + 1; j <= 2 * n; j++)
                    {
                        if (Math.Abs(a[k1, j]) > 1e-7) s += a[k1, j] * u[j];
                    }
                    u[k1] = ds[k1] - s;
                }

                for (int i = 1; i <= n; i++)
                {
                    va[i] += u[2 * i - 1];
                    vr[i] += u[2 * i];
                }
            }
        }

        private sealed class LoadFlowSolution
        {
            public bool Converged;
            public int Iterations;
            public double Mismatch;
            public readonly List<SolvedNode> Nodes = new List<SolvedNode>();
            public readonly List<SolvedBranch> Branches = new List<SolvedBranch>();
        }

        private sealed class SolvedNode
        {
            public int Number;
            public double Voltage;
            public double Angle;
            public double P;
            public double Q;
            public double Pg;
            public double Qb;
        }

        private sealed class SolvedBranch
        {
            public int StartNode;
            public int EndNode;
            public double P12;
            public double Q12;
            public double P21;
            public double Q21;
            public double DeltaP;
            public double Ia;
            public double Ir;
            public double R;
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
