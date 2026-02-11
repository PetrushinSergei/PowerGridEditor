using System;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class ShuntForm : Form
    {
        public Shunt MyShunt { get; private set; }

        // Свойства для доступа к элементам
        public TextBox StartNodeTextBox => textBoxStartNode;
        public TextBox ActiveResistanceTextBox => textBoxActiveResistance;
        public TextBox ReactiveResistanceTextBox => textBoxReactiveResistance;

        public ShuntForm()
        {
            InitializeComponent();
            MyShunt = new Shunt(0);
            InitializeFormData();
        }

        private void InitializeFormData()
        {
            labelCode.Text = "Код: " + MyShunt.Code;
            textBoxStartNode.Text = MyShunt.StartNodeNumber.ToString();
            textBoxActiveResistance.Text = MyShunt.ActiveResistance.ToString("F1");
            textBoxReactiveResistance.Text = MyShunt.ReactiveResistance.ToString();

            // Конечный узел всегда 0 и недоступен для редактирования
            textBoxEndNode.Text = MyShunt.EndNodeNumber.ToString();
            textBoxEndNode.Enabled = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("=== СОХРАНЕНИЕ ШУНТА ===");
                Console.WriteLine($"Начальный узел из TextBox: {textBoxStartNode.Text}");
                Console.WriteLine($"Активное сопротивление: {textBoxActiveResistance.Text}");
                Console.WriteLine($"Реактивное сопротивление: {textBoxReactiveResistance.Text}");

                // Сохраняем параметры шунта
                MyShunt.StartNodeNumber = int.Parse(textBoxStartNode.Text);
                MyShunt.ActiveResistance = double.Parse(textBoxActiveResistance.Text);
                MyShunt.ReactiveResistance = double.Parse(textBoxReactiveResistance.Text);

                Console.WriteLine($"Шунт сохранен: узел №{MyShunt.StartNodeNumber}");

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

        }
    }
}