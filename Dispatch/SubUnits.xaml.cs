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
    /// Interaction logic for SubUnits.xaml
    /// </summary>
    public partial class SubUnits : Window
    {
        public SubUnits()
        {
            InitializeComponent();
        }

        public string SubUnitSelected { get; set; }

        

        private void BtnDispatch_Click(object sender, RoutedEventArgs e)
        {
            //if(string.IsNullOrEmpty(TxtDispatchUnit.Text))
            //{
            //    MessageBox.Show("Please select a subunit before dispatching", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            SubUnitSelected = "";

            foreach(CheckBox chk in WrapSubUnits.Children)
            {
                if (chk.IsChecked == true)
                    SubUnitSelected += chk.Content + ",";
            }

            SubUnitSelected.TrimEnd(',');

            if (SubUnitSelected=="")
            {
                MessageBox.Show("Please select a subunit before dispatching", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var s in MainWindow._SubUnits)
            {
                //var button = new Button
                //{
                //    Content = s,
                //    Width = 120,
                //    Height = 30,
                //    Margin = new Thickness(5, 5, 5, 5)
                //};

                //button.Click += ButtonSubUnitClick;

                var checkBox = new CheckBox
                {
                    Content = s,
                    Width = 120,
                    Height = 30,
                    Margin = new Thickness(5, 5, 5, 5)
                };

                WrapSubUnits.Children.Add(checkBox);
            }

        }

        //private void ButtonSubUnitClick(object sender, RoutedEventArgs e)
        //{
        //    TxtDispatchUnit.Text = ((Button)sender).Content.ToString();
        //}
    }
}
