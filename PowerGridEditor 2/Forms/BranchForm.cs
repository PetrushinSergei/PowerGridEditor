using System;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class BranchForm : Form
    {
        public Branch MyBranch { get; private set; }

        // Свойства для доступа к элементам
        public TextBox StartNodeTextBox => textBoxStartNode;
        public TextBox EndNodeTextBox => textBoxEndNode;
        public TextBox ActiveResistanceTextBox => textBoxActiveResistance;
        public TextBox ReactiveResistanceTextBox => textBoxReactiveResistance;
        public TextBox ReactiveConductivityTextBox => textBoxReactiveConductivity;
        public TextBox TransformationRatioTextBox => textBoxTransformationRatio;
        public TextBox ActiveConductivityTextBox => textBoxActiveConductivity;

        public BranchForm()
        {
            InitializeComponent();
            MyBranch = new Branch(0, 0);
            InitializeFormData();
        }

        private void InitializeFormData()
        {
            labelCode.Text = "Код: " + MyBranch.Code;
            textBoxStartNode.Text = MyBranch.StartNodeNumber.ToString();
            textBoxEndNode.Text = MyBranch.EndNodeNumber.ToString();
            textBoxActiveResistance.Text = MyBranch.ActiveResistance.ToString("F1");
            textBoxReactiveResistance.Text = MyBranch.ReactiveResistance.ToString("F2");
            textBoxReactiveConductivity.Text = MyBranch.ReactiveConductivity.ToString("F1");
            textBoxTransformationRatio.Text = MyBranch.TransformationRatio.ToString();
            textBoxActiveConductivity.Text = MyBranch.ActiveConductivity.ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Сохраняем все параметры ветви
                MyBranch.StartNodeNumber = int.Parse(textBoxStartNode.Text);
                MyBranch.EndNodeNumber = int.Parse(textBoxEndNode.Text);
                MyBranch.ActiveResistance = double.Parse(textBoxActiveResistance.Text);
                MyBranch.ReactiveResistance = double.Parse(textBoxReactiveResistance.Text);
                MyBranch.ReactiveConductivity = double.Parse(textBoxReactiveConductivity.Text);
                MyBranch.TransformationRatio = double.Parse(textBoxTransformationRatio.Text);
                MyBranch.ActiveConductivity = double.Parse(textBoxActiveConductivity.Text);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Удали эти лишние методы или оставь пустыми:
        private void labelCode_Click(object sender, EventArgs e)
        {
            // Пустой обработчик, можно удалить
        }

        private void buttonSave_Click_1(object sender, EventArgs e)
        {
            // УДАЛИ этот метод - он дублирует buttonSave_Click
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            // УДАЛИ этот метод - он дублирует buttonCancel_Click
        }
    }
}