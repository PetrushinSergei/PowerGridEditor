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

            var nodes = ReadNodes().OrderBy(x => x.Number).ToList();
            var branches = _branches.OrderBy(x => x.Data.StartNodeNumber).ThenBy(x => x.Data.EndNodeNumber).ToList();
            var shunts = _shunts.OrderBy(x => x.Data.StartNodeNumber).ToList();
            var report = LoadFlowSolver.BuildReport(nodes, branches, shunts);

            txtOverview.Text = BuildOverview(nodes, branches, shunts, report);
            txtInput.Text = report.InputText;
            txtResults.Text = report.ResultsText;
            txtLoss.Text = report.LossAnalysisText;
            txtBreakdown.Text = report.LossComponentsText;
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

        private static string BuildOverview(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts, LoadFlowReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("network.cdu");
            sb.AppendLine($"Обновлено: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine(new string('-', 70));
            sb.AppendLine($"Число узлов: {nodes.Count}");
            sb.AppendLine($"Число ветвей: {branches.Count}");
            sb.AppendLine($"Число шунтов: {shunts.Count}");
            sb.AppendLine($"Итераций: {report.Iterations}");
            sb.AppendLine($"Невязка: {report.Mismatch:G6}");
            sb.AppendLine($"Сходимость: {(report.Converged ? "Да" : "Нет")}");
            return sb.ToString();
        }

        private sealed class LoadFlowSolver
        {
            private const int kkk = 100;
            private const int kk = 10;
            private static readonly CultureInfo C = CultureInfo.InvariantCulture;

            public static LoadFlowReport BuildReport(List<NodeSnapshot> sourceNodes, List<GraphicBranch> sourceBranches, List<GraphicShunt> sourceShunts)
            {
                var report = new LoadFlowReport();
                if (sourceNodes.Count == 0)
                {
                    report.InputText = "Нет данных для расчета.";
                    report.ResultsText = "Нет данных для расчета.";
                    report.LossAnalysisText = "Нет данных для расчета.";
                    report.LossComponentsText = "Нет данных для расчета.";
                    return report;
                }

                var nodes = ReorderNodes(sourceNodes);
                int n = nodes.Count - 1;
                int m = sourceBranches.Count;
                int[] nn = new int[nodes.Count];
                int[] nk = new int[nodes.Count];
                double[] unom = new double[nodes.Count];
                double[] p0 = new double[nodes.Count];
                double[] q0 = new double[nodes.Count];
                double[] g = new double[nodes.Count];
                double[] b = new double[nodes.Count];
                int[] nus = new int[10];

                var idxByNumber = new Dictionary<int, int>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    idxByNumber[node.Number] = i;
                    nn[i] = node.Number;
                    nk[i] = node.Type;
                    unom[i] = node.U;
                    p0[i] = node.PLoad - node.PGen;
                    q0[i] = node.QLoad - node.QGen;
                    g[i] = node.G;
                    b[i] = node.B;
                    int rk = nn[i] / kkk;
                    while (rk > 9) rk /= kk;
                    nus[rk]++;
                }

                foreach (var s in sourceShunts)
                {
                    int idx;
                    if (!idxByNumber.TryGetValue(s.Data.StartNodeNumber, out idx)) continue;
                    g[idx] += s.Data.ActiveResistance;
                    b[idx] -= s.Data.ReactiveResistance;
                }

                int[,] nm = new int[3, m + 1];
                int[,] nm1 = new int[3, m + 1];
                double[] r = new double[m + 1];
                double[] x = new double[m + 1];
                double[] gy = new double[m + 1];
                double[] by = new double[m + 1];
                double[] kt = new double[m + 1];
                for (int j = 1; j <= m; j++)
                {
                    var br = sourceBranches[j - 1].Data;
                    nm[1, j] = br.StartNodeNumber;
                    nm[2, j] = br.EndNodeNumber;
                    r[j] = br.ActiveResistance;
                    x[j] = Math.Abs(br.ReactiveResistance) < 1.001 ? 1.01 : br.ReactiveResistance;
                    gy[j] = br.ActiveConductivity * 1e-6;
                    by[j] = -br.ReactiveConductivity * 1e-6;
                    kt[j] = br.TransformationRatio < 0.001 ? 1 : br.TransformationRatio;
                    nm1[1, j] = idxByNumber.ContainsKey(nm[1, j]) ? idxByNumber[nm[1, j]] : 0;
                    nm1[2, j] = idxByNumber.ContainsKey(nm[2, j]) ? idxByNumber[nm[2, j]] : 0;
                }

                double[] gr = new double[m + 1];
                double[] bx = new double[m + 1];
                double[] gg = new double[nodes.Count];
                double[] bb = new double[nodes.Count];
                double[] va = new double[nodes.Count];
                double[] vr = new double[nodes.Count];
                double[] p = new double[nodes.Count];
                double[] q = new double[nodes.Count];
                double[] ja = new double[nodes.Count];
                double[] jr = new double[nodes.Count];
                double[] ds = new double[2 * nodes.Count + 2];
                double[,] A = new double[2 * nodes.Count + 2, 2 * nodes.Count + 2];

                for (int i = 0; i < nodes.Count; i++)
                {
                    va[i] = unom[i];
                    vr[i] = 0;
                    gg[i] = g[i] * 1e-6;
                    bb[i] = b[i] * 1e-6;
                }

                for (int j = 1; j <= m; j++)
                {
                    double c = r[j] * r[j] + x[j] * x[j];
                    gr[j] = r[j] / c;
                    bx[j] = -x[j] / c;

                    int i1 = nm1[1, j];
                    int i2 = nm1[2, j];
                    gg[i1] += gr[j] / (kt[j] * kt[j]) + gy[j] / 2.0;
                    bb[i1] += bx[j] / (kt[j] * kt[j]) + by[j] / 2.0;
                    gg[i2] += gr[j] + gy[j] / 2.0;
                    bb[i2] += bx[j] + by[j] / 2.0;
                }

                double mismatch = Func(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, p0, q0, unom, ja, jr, p, q, ds);
                int count = 0;
                while (mismatch > CalculationOptions.Precision && count < CalculationOptions.MaxIterations)
                {
                    Jacoby(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, ja, jr, unom, A);
                    Gauss(n, A, ds, va, vr);
                    count++;
                    mismatch = Func(n, m, nk, nm1, kt, gr, bx, gg, bb, va, vr, p0, q0, unom, ja, jr, p, q, ds);
                }

                report.Converged = mismatch <= CalculationOptions.Precision;
                report.Iterations = count;
                report.Mismatch = mismatch;

                report.InputText = BuildInputText(nn, nk, unom, p0, q0, g, b, nm, m, r, x, by, gy, kt, n);

                var loadResult = BuildLoadAndLossTexts(nn, n, m, nus, nm1, unom, va, vr, g, b, gy, by, kt, gr, bx, p, q, r);
                report.ResultsText = loadResult.Results;
                report.LossAnalysisText = loadResult.LossAnalysis;
                report.LossComponentsText = BuildLossComponentsText(n, m, nn, loadResult.TokRows);
                return report;
            }

            private static List<NodeSnapshot> ReorderNodes(List<NodeSnapshot> source)
            {
                var result = new List<NodeSnapshot>(source.Count);
                int slackIndex = source.FindIndex(x => x.Type == 3);
                if (slackIndex >= 0)
                {
                    result.Add(source[slackIndex]);
                }
                result.AddRange(source.Where((x, i) => i != slackIndex));
                if (result[0].Type != 3)
                {
                    var s = result[0];
                    s.Type = 3;
                    result[0] = s;
                }
                return result;
            }

            private static string BuildInputText(int[] nn, int[] nk, double[] unom, double[] p0, double[] q0, double[] g, double[] b, int[,] nm, int m, double[] r, double[] x, double[] by, double[] gy, double[] kt, int n)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Число узлов n = {n}\tЧисло Ветвей m = {m}");
                sb.AppendLine("     Входные данные для расчета потокораспределения");
                sb.AppendLine("           У з л ы    с е т и ");
                sb.AppendLine(" Узел   Тип  Uном        P        Q        g           b ");
                for (int i = 0; i <= n; i++)
                {
                    sb.AppendLine($"{nn[i],5}{nk[i],5}{Flex(unom[i], 0),7}{Flex(p0[i], 0),9}{Flex(q0[i], 0),9}{Flex(g[i], 5),9}{Flex(b[i], 5),12}");
                }

                sb.AppendLine();
                sb.AppendLine("               В е т в и    с е т и");
                sb.AppendLine("   N1   N2        r         x            b           g       Kt ");
                for (int j = 1; j <= m; j++)
                {
                    sb.AppendLine($"{nm[1, j],5}{nm[2, j],5}{Flex(r[j], 3),10}{Flex(x[j], 2),10}{Flex(Math.Abs(by[j] * 1e6), 7),14}{Flex(gy[j] * 1e6, 0),10}{Flex(kt[j], 0),9}");
                }
                return sb.ToString();
            }

            private static LoadOutput BuildLoadAndLossTexts(int[] nn, int n, int m, int[] nus, int[,] nm1, double[] unom, double[] va, double[] vr, double[] g, double[] b, double[] gy, double[] by, double[] kt, double[] gr, double[] bx, double[] p, double[] q, double[] r)
            {
                var net2 = new StringBuilder();
                var raipot = new StringBuilder();
                var tokRows = new List<TokRow>();

                net2.AppendLine("          Результаты расчета по узлам");
                net2.AppendLine("    N        V         dV         P         Q        Pg       Qb");

                double sp = 0, sq = 0, spg = 0, sqb = 0, dPsum = 0;
                var saldo = new double[2, 10];
                var sumpot = new double[10];
                var sv = new int[10];
                var line = new double[10, 10];
                var linc = new int[10, 10];

                for (int i = 0; i <= n; i++)
                {
                    double mv = va[i] * va[i] + vr[i] * vr[i];
                    double dv = Math.Atan2(vr[i], va[i]) * 57.295779515;
                    double pg = mv * (g[i] * 1e-6);
                    double qb = -mv * (b[i] * 1e-6);
                    sp += p[i]; sq += q[i]; spg += pg; sqb += qb;
                    mv = Math.Sqrt(mv);

                    net2.AppendLine($"{nn[i],5}{F2(mv),10}{F2(dv),10}{F2(-p[i]),10}{F2(-q[i]),10}{F2(pg),10}{F2(qb),10}");
                }

                net2.AppendLine("---------------------------------------------------");
                net2.AppendLine($"Баланс пассивных элементов {F2(sp),10}{F2(sq),10}{F2(spg),10}{F2(sqb),10}");
                net2.AppendLine("                         + потребление, - генерация ");
                net2.AppendLine();
                net2.AppendLine("                   Результаты расчета по ветвям");
                net2.AppendLine("   N1   N1       P12       Q12       P21       Q21       dP");

                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[1, j], i2 = nm1[2, j];

                    double i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / (kt[j] * kt[j]) - (va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
                    double i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / (kt[j] * kt[j]) - (va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
                    double i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]);
                    double i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]);

                    double mv = va[i1] * va[i1] + vr[i1] * vr[i1];
                    double p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv / 2.0;
                    double q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv / 2.0;

                    mv = va[i2] * va[i2] + vr[i2] * vr[i2];
                    double p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv / 2.0;
                    double q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv / 2.0;

                    double dpl = Math.Abs(p21 - p12);
                    dPsum += dpl;

                    tokRows.Add(new TokRow { Start = i1, End = i2, Ia = i1a, Ir = i1r, R = Math.Abs(r[j]) });
                    net2.AppendLine($"{nn[i1],5}{nn[i2],5}{F2(-p12),10}{F2(-q12),10}{F2(p21),10}{F2(q21),10}{F2(dpl),10}");

                    int rk = nn[i1] / kkk; while (rk > 9) rk /= kk;
                    int r2 = nn[i2] / kkk; while (r2 > 9) r2 /= kk;
                    if (rk == r2)
                    {
                        sv[rk]++;
                        sumpot[rk] += dpl;
                        if (Math.Abs(kt[j] - 1) < 0.001)
                        {
                            int cls = VoltageClass(unom[i1]);
                            line[rk, cls] += dpl;
                            linc[rk, cls]++;
                        }
                    }
                    else
                    {
                        saldo[0, rk] += -p12;
                        saldo[1, rk] += -q12;
                        saldo[0, r2] += p21;
                        saldo[1, r2] += q21;
                    }
                }

                net2.AppendLine($"{F2(dPsum),60}");

                int[] ul = { 6, 10, 20, 35, 110, 220, 330, 500, 750, 1150 };
                for (int i = 0; i < 10; i++)
                {
                    if (sv[i] == 0) continue;
                    raipot.AppendLine($"Район  №{i,6}");
                    for (int g1 = 0; g1 < 10; g1++)
                    {
                        if (linc[i, g1] != 0)
                        {
                            raipot.AppendLine(" Потери в линиях");
                            raipot.AppendLine($"{ul[g1],5} кВ{Flex(line[i, g1], 3),15}");
                        }
                    }
                }

                raipot.AppendLine();
                raipot.AppendLine("№ района          Сальдо P           Сальдо Q       Сумм. потери в районе");
                double sumoll = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (nus[i] == 0) continue;
                    raipot.AppendLine($"{i,5}{F2(saldo[0, i]),20}{F2(saldo[1, i]),20}{F2(sumpot[i]),20}");
                    sumoll += sumpot[i];
                }
                raipot.AppendLine($" Суммарные потери в сетях районов {F2(sumoll),25}");

                return new LoadOutput
                {
                    Results = net2.ToString(),
                    LossAnalysis = raipot.ToString(),
                    TokRows = tokRows
                };
            }

            private static string BuildLossComponentsText(int n, int m, int[] nn, List<TokRow> tokRows)
            {
                int L = m;
                int[] na = new int[L + 1], ka = new int[L + 1], nr = new int[L + 1], kr = new int[L + 1], N = new int[L + 1], k = new int[L + 1];
                double[] ta = new double[L + 1], tr = new double[L + 1], rd = new double[L + 1], dP = new double[L + 1], tza = new double[n + 1], tzr = new double[n + 1];
                double[,] aa = new double[L + 1, n + 1], a = new double[L + 1, n + 1], ar = new double[L + 1, n + 1];

                for (int i = 1; i <= L; i++)
                {
                    var row = tokRows[i - 1];
                    na[i] = row.Start;
                    ka[i] = row.End;
                    ta[i] = row.Ia;
                    tr[i] = row.Ir;
                    rd[i] = row.R;
                    nr[i] = na[i];
                    kr[i] = ka[i];
                }

                for (int i = 1; i <= L; i++)
                {
                    tza[na[i]] -= ta[i];
                    tzr[na[i]] -= tr[i];
                    tza[ka[i]] += ta[i];
                    tzr[ka[i]] += tr[i];
                }

                var sb = new StringBuilder();
                sb.AppendLine("  Разделение потерь мощности электрической сети  ");
                sb.AppendLine("  Входные данные для расчета ");
                sb.AppendLine($"  Число узлов  = {n + 1}\tЧисло Ветвей  = {L}");
                sb.AppendLine("         Токи ветвей ");
                sb.AppendLine(" Ветвь Нач.   Кон.    Ток Ak   Ток Re    R  ");
                for (int i = 1; i <= L; i++) sb.AppendLine($"{nn[na[i]],8}{nn[ka[i]],8}{F2(ta[i]),10}{F2(tr[i]),10}{F2(Math.Abs(rd[i])),10}");

                sb.AppendLine("       Задающие токи узлов  ");
                sb.AppendLine("     Узел            ТЗа     ТЗр  ");
                for (int j = 1; j <= n; j++) sb.AppendLine($"{nn[j],8}{F2(tza[j]),10}{F2(tzr[j]),10}");
                sb.AppendLine($"{nn[0],8}{F2(tza[0]),10}{F2(tzr[0]),10}");

                for (int j = 0; j <= n; j++)
                {
                    double sia = tza[j];
                    for (int i = 1; i <= L; i++) if (na[i] == j) sia += ta[i];
                    if (Math.Abs(sia) > 1e-10)
                        for (int i = 1; i <= L; i++) if (ka[i] == j) a[i, j] = ta[i] / sia;
                }

                for (int j = 0; j <= n; j++)
                {
                    double sia = tzr[j];
                    for (int i = 1; i <= L; i++) if (nr[i] == j) sia += tr[i];
                    if (Math.Abs(sia) > 1e-10)
                        for (int i = 1; i <= L; i++) if (kr[i] == j) ar[i, j] = tr[i] / sia;
                }

                for (int i = 1; i <= L; i++)
                {
                    N[i] = na[i];
                    k[i] = ka[i];
                    for (int j = 0; j <= n; j++) aa[i, j] = a[i, j];
                }
                Alpha(L, n, aa, N, k);
                for (int i = 1; i <= L; i++) for (int j = 0; j <= n; j++) a[i, j] = aa[i, j];

                for (int i = 1; i <= L; i++)
                {
                    N[i] = nr[i];
                    k[i] = kr[i];
                    for (int j = 0; j <= n; j++) aa[i, j] = ar[i, j];
                }
                Alpha(L, n, aa, N, k);
                for (int i = 1; i <= L; i++) for (int j = 0; j <= n; j++) ar[i, j] = aa[i, j];

                for (int i = 1; i <= L; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        a[i, j] = tza[j] * a[i, j];
                        ar[i, j] = tzr[j] * ar[i, j];
                    }
                    dP[i] = Math.Abs(rd[i]) * (ta[i] * ta[i] + tr[i] * tr[i]);
                }

                sb.AppendLine();
                sb.AppendLine("              Результаты расчетов  ");
                sb.AppendLine();
                sb.AppendLine("          Составляющие потерь от нагрузок узлов   ");
                sb.Append("Ветвь    ");
                for (int j = 1; j <= n; j++) sb.Append($"{nn[j],6}");
                sb.AppendLine($"{nn[0],6}   dP");

                for (int i = 1; i <= L; i++)
                {
                    sb.Append($"{nn[na[i]],4} {nn[ka[i]],4}");
                    double total = 0;
                    for (int j = 1; j <= n; j++)
                    {
                        double comp = 0;
                        for (int kk1 = 0; kk1 <= n; kk1++) comp += a[i, j] * a[i, kk1] + ar[i, j] * ar[i, kk1];
                        aa[i, j] = Math.Abs(rd[i]) * comp;
                        total += aa[i, j];
                        sb.Append($"{F2(aa[i, j]),6}");
                    }
                    double bal = 0;
                    for (int kk1 = 0; kk1 <= n; kk1++) bal += a[i, 0] * a[i, kk1] + ar[i, 0] * ar[i, kk1];
                    aa[i, 0] = Math.Abs(rd[i]) * bal;
                    total += aa[i, 0];
                    sb.AppendLine($"{F2(aa[i, 0]),6}{F2(total),7}");
                }

                sb.AppendLine();
                for (int i = 1; i <= L; i++)
                {
                    sb.AppendLine($"Ветвь   {nn[na[i]]}-{nn[ka[i]]}  Потери  {F2(dP[i])}");
                    sb.Append("Составляющие ");
                    for (int j = 1; j <= n; j++) if (Math.Abs(aa[i, j]) > 1e-6) sb.Append($" dP{nn[j]}={F2(aa[i, j])}");
                    if (Math.Abs(aa[i, 0]) > 1e-6) sb.Append($" dP{nn[0]}={F2(aa[i, 0])}");
                    sb.AppendLine();
                }

                sb.AppendLine();
                sb.AppendLine("          Доля транзитных потерь от нагрузок узлов   ");
                sb.Append("Узлы    ");
                for (int j = 1; j <= n; j++) sb.Append($"{nn[j],6}");
                sb.AppendLine($"{nn[0],6}");
                sb.Append("Потери ");
                for (int j = 1; j <= n; j++)
                {
                    double nodeLoss = 0;
                    for (int i = 1; i <= L; i++) nodeLoss += aa[i, j];
                    sb.Append($"{F2(nodeLoss),6}");
                }
                double balLoss = 0;
                for (int i = 1; i <= L; i++) balLoss += aa[i, 0];
                sb.AppendLine($"{F2(balLoss),6}");

                return sb.ToString();
            }

            private static void Alpha(int L, int n, double[,] aa, int[] N, int[] k)
            {
                const int maxDepth = 32;
                const double minCoef = 1e-10;
                var src = new double[L + 1, n + 1];
                var result = new double[L + 1, n + 1];

                for (int i = 1; i <= L; i++)
                for (int j = 0; j <= n; j++)
                {
                    src[i, j] = aa[i, j];
                    result[i, j] = src[i, j];
                }

                for (int startBranch = 1; startBranch <= L; startBranch++)
                {
                    var stack = new Stack<AlphaState>();
                    for (int j = 0; j <= n; j++)
                        if (Math.Abs(src[startBranch, j]) > minCoef)
                            stack.Push(new AlphaState { Node = j, Coef = src[startBranch, j], Depth = 0 });

                    while (stack.Count > 0)
                    {
                        var st = stack.Pop();
                        if (st.Depth >= maxDepth || Math.Abs(st.Coef) < minCoef) continue;

                        for (int br = 1; br <= L; br++)
                        {
                            if (N[br] != st.Node) continue;
                            int nextNode = k[br];
                            double step = src[br, nextNode];
                            if (Math.Abs(step) < minCoef) continue;
                            double nextCoef = st.Coef * step;
                            if (Math.Abs(nextCoef) < minCoef) continue;
                            result[startBranch, nextNode] += nextCoef;
                            stack.Push(new AlphaState { Node = nextNode, Coef = nextCoef, Depth = st.Depth + 1 });
                        }
                    }
                }

                for (int i = 1; i <= L; i++)
                for (int j = 0; j <= n; j++) aa[i, j] = result[i, j];
            }

            private static double Func(int n, int m, int[] nk, int[,] nm1, double[] kt, double[] gr, double[] bx, double[] gg, double[] bb, double[] va, double[] vr, double[] p0, double[] q0, double[] unom, double[] ja, double[] jr, double[] p, double[] q, double[] ds)
            {
                double w = 0;
                for (int i = 0; i <= n; i++)
                {
                    ja[i] = gg[i] * va[i] - bb[i] * vr[i];
                    jr[i] = bb[i] * va[i] + gg[i] * vr[i];
                }
                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[1, j], i2 = nm1[2, j];
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
                return Math.Sqrt(w / (2 * Math.Max(n, 1)));
            }

            private static void Jacoby(int n, int m, int[] nk, int[,] nm1, double[] kt, double[] gr, double[] bx, double[] gg, double[] bb, double[] va, double[] vr, double[] ja, double[] jr, double[] unom, double[,] A)
            {
                Array.Clear(A, 0, A.Length);
                for (int i = 1; i <= n; i++)
                {
                    if (nk[i] == 1)
                    {
                        A[2 * i - 1, 2 * i - 1] = -bb[i] * va[i] + gg[i] * vr[i] - jr[i];
                        A[2 * i - 1, 2 * i] = -gg[i] * va[i] - bb[i] * vr[i] + ja[i];
                    }
                    else
                    {
                        A[2 * i - 1, 2 * i - 1] = 2 * va[i] / unom[i];
                        A[2 * i - 1, 2 * i] = 2 * vr[i] / unom[i];
                    }
                    A[2 * i, 2 * i - 1] = gg[i] * va[i] + bb[i] * vr[i] + ja[i];
                    A[2 * i, 2 * i] = -bb[i] * va[i] + gg[i] * vr[i] + jr[i];
                }
                for (int j = 1; j <= m; j++)
                {
                    int i1 = nm1[1, j], i2 = nm1[2, j];
                    if (i1 == 0 || i2 == 0) continue;
                    int j1 = 2 * i1 - 1, j2 = 2 * i1, j3 = 2 * i2 - 1, j4 = 2 * i2;
                    if (nk[i1] == 1)
                    {
                        A[j1, j3] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                        A[j1, j4] = -(-gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
                    }
                    A[j2, j3] = -(gr[j] * va[i1] + bx[j] * vr[i1]) / kt[j];
                    A[j2, j4] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                    if (nk[i2] == 1)
                    {
                        A[j3, j1] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                        A[j3, j2] = -(-gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
                    }
                    A[j4, j1] = -(gr[j] * va[i2] + bx[j] * vr[i2]) / kt[j];
                    A[j4, j2] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                }
            }

            private static void Gauss(int n, double[,] A, double[] ds, double[] va, double[] vr)
            {
                var u = new double[2 * n + 1];
                for (int i = 1; i <= 2 * n - 1; i++)
                {
                    double c = Math.Abs(A[i, i]) < 1e-7 ? 1e-7 : A[i, i];
                    for (int j = i + 1; j <= 2 * n; j++) if (Math.Abs(A[i, j]) > 1e-7) A[i, j] /= c;
                    ds[i] /= c;
                    for (int k1 = i + 1; k1 <= 2 * n; k1++)
                    {
                        double d = A[k1, i];
                        if (Math.Abs(d) <= 1e-7) continue;
                        for (int l = i + 1; l <= 2 * n; l++) if (Math.Abs(A[i, l]) > 1e-7) A[k1, l] -= A[i, l] * d;
                        ds[k1] -= ds[i] * d;
                    }
                }
                u[2 * n] = ds[2 * n] / A[2 * n, 2 * n];
                for (int k1 = 2 * n - 1; k1 >= 1; k1--)
                {
                    double s = 0;
                    for (int j = k1 + 1; j <= 2 * n; j++) if (Math.Abs(A[k1, j]) > 1e-7) s += A[k1, j] * u[j];
                    u[k1] = ds[k1] - s;
                }
                for (int i = 1; i <= n; i++)
                {
                    va[i] += u[2 * i - 1];
                    vr[i] += u[2 * i];
                }
            }

            private static int VoltageClass(double u)
            {
                if (u <= 8) return 0;
                if (u <= 15) return 1;
                if (u <= 28) return 2;
                if (u <= 70) return 3;
                if (u <= 140) return 4;
                if (u <= 270) return 5;
                if (u <= 430) return 6;
                if (u <= 600) return 7;
                if (u <= 900) return 8;
                return 9;
            }

            private static string F2(double v) => Math.Round(v, 2, MidpointRounding.AwayFromZero).ToString("0.00", C);

            private static string Flex(double v, int maxDecimals = 7)
            {
                double rv = Math.Round(v, maxDecimals, MidpointRounding.AwayFromZero);
                if (Math.Abs(rv) < 5 * Math.Pow(10, -maxDecimals)) rv = 0;
                return rv.ToString("0." + new string('#', maxDecimals), C);
            }

            private struct AlphaState
            {
                public int Node;
                public double Coef;
                public int Depth;
            }

            private sealed class LoadOutput
            {
                public string Results;
                public string LossAnalysis;
                public List<TokRow> TokRows;
            }

            private sealed class TokRow
            {
                public int Start;
                public int End;
                public double Ia;
                public double Ir;
                public double R;
            }
        }

        private sealed class LoadFlowReport
        {
            public bool Converged;
            public int Iterations;
            public double Mismatch;
            public string InputText;
            public string ResultsText;
            public string LossAnalysisText;
            public string LossComponentsText;
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
