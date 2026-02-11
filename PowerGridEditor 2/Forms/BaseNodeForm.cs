using System;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class BaseNodeForm : Form
    {
        public BaseNode MyBaseNode { get; private set; }

        // Свойства для доступа к TextBox
        public TextBox NodeNumberTextBox => textBoxNodeNumber;
        public TextBox InitialVoltageTextBox => textBoxInitialVoltage;
        public TextBox NominalActivePowerTextBox => textBoxNominalActivePower;
        public TextBox NominalReactivePowerTextBox => textBoxNominalReactivePower;
        public TextBox ActivePowerGenerationTextBox => textBoxActivePowerGeneration;
        public TextBox ReactivePowerGenerationTextBox => textBoxReactivePowerGeneration;
        public TextBox FixedVoltageModuleTextBox => textBoxFixedVoltageModule;
        public TextBox MinReactivePowerTextBox => textBoxMinReactivePower;
        public TextBox MaxReactivePowerTextBox => textBoxMaxReactivePower;

        public BaseNodeForm()
        {
            InitializeComponent();
            MyBaseNode = new BaseNode(0); // Убедись что это есть
            InitializeFormData();
        }

        private void InitializeFormData()
        {
            labelCode.Text = "Код: " + MyBaseNode.Code;
            textBoxNodeNumber.Text = MyBaseNode.Number.ToString();
            textBoxInitialVoltage.Text = MyBaseNode.InitialVoltage.ToString("F2");
            textBoxNominalActivePower.Text = MyBaseNode.NominalActivePower.ToString("F2");
            textBoxNominalReactivePower.Text = MyBaseNode.NominalReactivePower.ToString("F2");
            textBoxActivePowerGeneration.Text = MyBaseNode.ActivePowerGeneration.ToString("F2");
            textBoxReactivePowerGeneration.Text = MyBaseNode.ReactivePowerGeneration.ToString("F2");
            textBoxFixedVoltageModule.Text = MyBaseNode.FixedVoltageModule.ToString("F2");
            textBoxMinReactivePower.Text = MyBaseNode.MinReactivePower.ToString("F2");
            textBoxMaxReactivePower.Text = MyBaseNode.MaxReactivePower.ToString("F2");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("=== СОХРАНЕНИЕ БАЗИСНОГО УЗЛА ===");
                Console.WriteLine($"Номер из TextBox: {textBoxNodeNumber.Text}");

                // СОХРАНЯЕМ ВСЕ ДАННЫЕ В MyBaseNode
                MyBaseNode.Number = int.Parse(textBoxNodeNumber.Text);
                MyBaseNode.InitialVoltage = double.Parse(textBoxInitialVoltage.Text);
                MyBaseNode.NominalActivePower = double.Parse(textBoxNominalActivePower.Text);
                MyBaseNode.NominalReactivePower = double.Parse(textBoxNominalReactivePower.Text);
                MyBaseNode.ActivePowerGeneration = double.Parse(textBoxActivePowerGeneration.Text);
                MyBaseNode.ReactivePowerGeneration = double.Parse(textBoxReactivePowerGeneration.Text);
                MyBaseNode.FixedVoltageModule = double.Parse(textBoxFixedVoltageModule.Text);
                MyBaseNode.MinReactivePower = double.Parse(textBoxMinReactivePower.Text);
                MyBaseNode.MaxReactivePower = double.Parse(textBoxMaxReactivePower.Text);

                Console.WriteLine($"Узел сохранен: №{MyBaseNode.Number}");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void labelCode_Click(object sender, EventArgs e)
        {
            // Пустой обработчик
        }

        private void buttonSave_Click_1(object sender, EventArgs e)
        {

        }
    }
}