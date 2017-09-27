using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dispatch
{
    /// <summary>
    /// Interaction logic for SelectUnit.xaml
    /// </summary>
    public partial class SelectUnit : Window
    {
        public SelectUnit(List<string> units)
        {
            InitializeComponent();

            _lstUnits.AddRange(units);
        }

        private List<string> _lstUnits = new List<string>();

        public string SelectedUnit { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var unit in _lstUnits)
            {
                var btn = new Button
                {
                    Content = unit,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = 50,
                    Width = 180,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                btn.Click += Btn_Click;
                StckUnits.Children.Add(btn);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var res = MessageBox.Show("Do you wish to select the unit: " + btn.Content, "Question",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes)
                return;

            SelectedUnit = btn.Content.ToString();
            DialogResult = true;
        }
    }
}
