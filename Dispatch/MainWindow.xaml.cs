using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //BindingOperations.EnableCollectionSynchronization(LstMessages, _personCollectionLock);

            LstDisplay.ItemsSource = LstMessages;

            //LstMessages.Add(new DispatchItem { Date = DateTime.Now, Description = "Hey" });

            _parser = new Parser();
        }

        public FastCollection<Call> LstMessages=new FastCollection<Call>();

        private Unit _unit;

        private Parser _parser;
        

        private void LstDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstDisplay.SelectedIndex< 0)
                return;
            Call item = (Call) LstDisplay.SelectedItem;
            GrdDocumnet.DataContext = item;
            //TxtDisplay.Text=item.ToString();
        }
        

        private void Window_Closed(object sender, EventArgs e)
        {
            //Application.Current.Shutdown();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var unitName = System.IO.File.ReadAllText("unit.txt");
                _unit = await _parser.GetUnitAsync(unitName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

                Application.Current.Shutdown();
            }

            if (_unit != null)
                RefreshMessage();
        }


        private void PrintDocument()
        {
            //PrintDialog printDialog = new PrintDialog();

            //if(printDialog.ShowDialog()==true)
            //{
            //    printDialog.PrintDocument(((IDocumentPaginatorSource)FDR).DocumentPaginator, "Printing Report");
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PrintDocument();

            if(LstDisplay.SelectedIndex<0)
            {
                MessageBox.Show("Please select a message for report","Info",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }

            Call item = (Call)LstDisplay.SelectedItem;

            var report = new Report_Viewer() { Cal = item };

            report.Owner = this;

            report.ShowDialog();
        }



        private async void RefreshMessage()
        {
            int count = 0;
            for(; ; )
            {
                if (count == 3)
                {
                    await GetCalls();
                    count = 0;
                    continue;
                }
                await CheckMessage();
                await Task.Delay(1000 * 10);
                count++;
            }
        }


        private async Task GetCalls()
        {

            var list = await _parser.GetCallsAsync(_unit.Name);
            
            
            if (list != null && list.Count > 0)
            {
                LstMessages.NotificationOff();

                foreach (var item in list)
                {
                    if (!LstMessages.Any(x=>x.Id==item.Id))
                        LstMessages.Insert(0,item);
                }

                LstMessages.NotificationOn();
            }
        }

        private async Task CheckMessage()
        {
            var unit = await _parser.GetUnitAsync(_unit.Id);

            if(unit.IsRequestOn)
            {
                var msgRes = MessageBox.Show(unit.Message + "\r\nDo you wish to accept this task?", "New Task Request",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRes == MessageBoxResult.Yes)
                    SendRequest(true);
                else
                    SendRequest(false);
            }
        }

        private async void SendRequest(bool res)
        {
            try
            {
                await _parser.AcceptRejectRequest(new UnitAcceptRejectRequestModel { Id = _unit.Id, AcceptRequest = res });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

    }
}
