using System.Windows;

namespace MainGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculateCOPandP calculateCoPandP = new CalculateCOPandP();
        private GUIChecks guiChecks = new GUIChecks();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (!guiChecks.TryGetValue(PheTextBox, out double Phe) || !guiChecks.TryGetValue(PeTextBox, out double Pe))
                return;

            ResultTextBox.Text = calculateCoPandP.CalculateCOP(Phe, Pe).ToString();
        }
    }
}