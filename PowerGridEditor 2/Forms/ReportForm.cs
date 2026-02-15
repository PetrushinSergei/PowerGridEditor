using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

        private string _lastSignature;
        private string _cachedOverview;
        private string _cachedInput;
        private string _cachedResults;
        private string _cachedLoss;
        private string _cachedBreakdown;

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

            var signature = BuildSignature(nodes, branches, shunts);
            if (signature == _lastSignature)
            {
                txtOverview.Text = _cachedOverview ?? string.Empty;
                txtInput.Text = _cachedInput ?? string.Empty;
                txtResults.Text = _cachedResults ?? string.Empty;
                txtLoss.Text = _cachedLoss ?? string.Empty;
                txtBreakdown.Text = _cachedBreakdown ?? string.Empty;
                return;
            }

            if (TryGenerateExternalReports(nodes, branches, shunts, out var ov, out var input, out var rez, out var rip, out var loss))
            {
                _lastSignature = signature;
                _cachedOverview = ov;
                _cachedInput = input;
                _cachedResults = rez;
                _cachedLoss = rip;
                _cachedBreakdown = loss;
            }
            else
            {
                // fallback к предыдущему встроенному отображению
                _lastSignature = signature;
                _cachedOverview = BuildOverview(nodes, branches, shunts);
                _cachedInput = BuildInput(nodes, branches, shunts);
                _cachedResults = BuildResults(nodes, branches, shunts);
                _cachedLoss = BuildLoss(nodes, branches, shunts);
                _cachedBreakdown = BuildBreakdown(nodes, branches, shunts);
            }

            txtOverview.Text = _cachedOverview;
            txtInput.Text = _cachedInput;
            txtResults.Text = _cachedResults;
            txtLoss.Text = _cachedLoss;
            txtBreakdown.Text = _cachedBreakdown;
        }

        private static string BuildSignature(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            foreach (var n in nodes)
            {
                sb.Append(n.Number).Append('|').Append(n.Type).Append('|').Append(n.U.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(n.PLoad.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(n.QLoad.ToString("G17", CultureInfo.InvariantCulture)).Append(';');
            }
            foreach (var b in branches)
            {
                sb.Append(b.Data.StartNodeNumber).Append('-').Append(b.Data.EndNodeNumber).Append('|')
                    .Append(b.Data.ActiveResistance.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(b.Data.ReactiveResistance.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(b.Data.ReactiveConductivity.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(b.Data.TransformationRatio.ToString("G17", CultureInfo.InvariantCulture)).Append(';');
            }
            foreach (var s in shunts)
            {
                sb.Append("S").Append(s.Data.StartNodeNumber).Append('|')
                    .Append(s.Data.ActiveResistance.ToString("G17", CultureInfo.InvariantCulture)).Append('|')
                    .Append(s.Data.ReactiveResistance.ToString("G17", CultureInfo.InvariantCulture)).Append(';');
            }
            return sb.ToString();
        }

        private bool TryGenerateExternalReports(
            List<NodeSnapshot> nodes,
            List<GraphicBranch> branches,
            List<GraphicShunt> shunts,
            out string overview,
            out string input,
            out string results,
            out string loss,
            out string breakdown)
        {
            overview = input = results = loss = breakdown = string.Empty;

            var runtimeDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConsoleApplicationCS_Runtime");
            Directory.CreateDirectory(runtimeDir);
            var cduPath = Path.Combine(runtimeDir, "network_input.cdu");

            WriteCdu(cduPath, nodes, branches, shunts);

            if (!TryRunConsoleApplication(runtimeDir, cduPath, out var runError))
            {
                overview = "network.cdu\r\n" + runError;
                input = runError;
                results = runError;
                loss = runError;
                breakdown = runError;
                return false;
            }

            overview = ReadOrMessage(Path.Combine(runtimeDir, "network.cdu"));
            input = ReadOrMessage(Path.Combine(runtimeDir, "network.out"));
            results = ReadOrMessage(Path.Combine(runtimeDir, "network.rez"));
            loss = ReadOrMessage(Path.Combine(runtimeDir, "network.rip"));
            breakdown = ReadOrMessage(Path.Combine(runtimeDir, "losses.rez"));
            return true;
        }

        private static string ReadOrMessage(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : ("Файл не найден: " + path);
        }

        private static void WriteCdu(string filePath, List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var c = CultureInfo.InvariantCulture;
            var slack = nodes.FirstOrDefault(x => x.Type == 3);
            var pqpv = nodes.Where(x => x.Type != 3).ToList();

            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                if (slack.Number != 0)
                {
                    sw.WriteLine(string.Format(c,
                        "0102 0 {0,4} {1,7:0.00} {2,7:0.00} {3,7:0.00} 0 0 0 0 0",
                        slack.Number, slack.U, slack.PLoad, slack.QLoad));
                }

                foreach (var n in pqpv)
                {
                    // По текущим данным считаем PQ-узлами (0201), как было для ConsoleApplication1
                    sw.WriteLine(string.Format(c,
                        "0201 0 {0,4} {1,7:0.00} {2,7:0.00} {3,7:0.00} 0 0 0 0 0",
                        n.Number, n.U, n.PLoad, n.QLoad));
                }

                foreach (var s in shunts)
                {
                    sw.WriteLine(string.Format(c,
                        "0301 0 {0,4} 0 {1,7:0.###} {2,7:0.###}",
                        s.Data.StartNodeNumber,
                        s.Data.ActiveResistance,
                        s.Data.ReactiveResistance));
                }

                foreach (var b in branches)
                {
                    sw.WriteLine(string.Format(c,
                        "0301 0 {0,4} {1,4} {2,7:0.###} {3,7:0.###} {4,9:0.###} {5,4:0.###} 0 0 0",
                        b.Data.StartNodeNumber,
                        b.Data.EndNodeNumber,
                        b.Data.ActiveResistance,
                        b.Data.ReactiveResistance,
                        b.Data.ReactiveConductivity,
                        b.Data.TransformationRatio <= 0 ? 1 : b.Data.TransformationRatio));
                }
            }
        }

        private static bool TryRunConsoleApplication(string runtimeDir, string cduPath, out string error)
        {
            error = string.Empty;
            string runnerPath = FindRunner();
            if (string.IsNullOrEmpty(runnerPath))
            {
                error = "Не найден ConsoleApplicationCS.exe (или .dll). Сначала соберите ConsoleApplicationCS.";
                return false;
            }

            var isDll = runnerPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
            var psi = new ProcessStartInfo
            {
                FileName = isDll ? "dotnet" : runnerPath,
                Arguments = isDll ? ('"' + runnerPath + '"') : string.Empty,
                WorkingDirectory = runtimeDir,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                // порядок ввода в ConsoleApplicationCS: путь, точность, итерации
                process.StandardInput.WriteLine(cduPath);
                process.StandardInput.WriteLine(CalculationOptions.Precision.ToString(CultureInfo.InvariantCulture));
                process.StandardInput.WriteLine(CalculationOptions.MaxIterations.ToString(CultureInfo.InvariantCulture));
                process.StandardInput.Flush();
                process.StandardInput.Close();

                var output = process.StandardOutput.ReadToEnd();
                var err = process.StandardError.ReadToEnd();
                process.WaitForExit(60000);

                if (process.ExitCode != 0)
                {
                    error = "Ошибка запуска ConsoleApplicationCS.\r\n" + output + "\r\n" + err;
                    return false;
                }
            }

            return true;
        }

        private static string FindRunner()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "ConsoleApplicationCS", "ConsoleApplicationCS.exe"),
                Path.Combine(baseDir, "ConsoleApplicationCS.exe"),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\ConsoleApplicationCS\bin\Debug\net8.0\ConsoleApplicationCS.exe")),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\ConsoleApplicationCS\bin\Release\net8.0\ConsoleApplicationCS.exe")),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\ConsoleApplicationCS\bin\Debug\net8.0\ConsoleApplicationCS.dll")),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\ConsoleApplicationCS\bin\Release\net8.0\ConsoleApplicationCS.dll"))
            };

            return candidates.FirstOrDefault(File.Exists);
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

        // fallback отображение, если внешний расчёт не запустился
        private static string BuildOverview(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("network.cdu");
            sb.AppendLine($"Обновлено: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine(new string('-', 70));
            sb.AppendLine($"Число узлов: {nodes.Count}");
            sb.AppendLine($"Число ветвей: {branches.Count}");
            sb.AppendLine($"Число шунтов: {shunts.Count}");
            return sb.ToString();
        }

        private static string BuildInput(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            return "Внешний расчёт недоступен.";
        }

        private static string BuildResults(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            return "Внешний расчёт недоступен.";
        }

        private static string BuildLoss(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            return "Внешний расчёт недоступен.";
        }

        private static string BuildBreakdown(List<NodeSnapshot> nodes, List<GraphicBranch> branches, List<GraphicShunt> shunts)
        {
            return "Внешний расчёт недоступен.";
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
