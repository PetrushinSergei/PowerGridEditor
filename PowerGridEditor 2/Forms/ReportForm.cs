using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            var engine = new ConsoleApplicationEngine(nodes, branches, shunts, CalculationOptions.Precision, CalculationOptions.MaxIterations);
            var result = engine.Run();

            txtOverview.Text = result.NetworkCdu;
            txtInput.Text = result.NetworkOut;
            txtResults.Text = result.NetworkRez;
            txtLoss.Text = result.NetworkRip;
            txtBreakdown.Text = result.LossesRez;
        }

        private List<ReportNodeSnapshot> ReadNodes()
        {
            var list = new List<ReportNodeSnapshot>();
            foreach (var el in _elements)
            {
                var node = el as GraphicNode;
                if (node != null)
                {
                    int type = node.Data.FixedVoltageModule > 0.1 ? 2 : 1;
                    list.Add(ReportNodeSnapshot.FromNode(node.Data.Number, type, node.Data));
                }

                var baseNode = el as GraphicBaseNode;
                if (baseNode != null)
                {
                    list.Add(ReportNodeSnapshot.FromNode(baseNode.Data.Number, 3, baseNode.Data));
                }
            }
            return list;
        }
    }
}
