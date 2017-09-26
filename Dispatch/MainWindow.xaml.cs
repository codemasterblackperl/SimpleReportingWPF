using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public static readonly log4net.ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            if (System.Diagnostics.Process.GetProcessesByName(
                System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location))
                .Length > 1)
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            InitializeComponent();

            //BindingOperations.EnableCollectionSynchronization(LstMessages, _personCollectionLock);

            Log.Info("Application Started");

            LstDisplay.ItemsSource = LstMessages;

            //LstMessages.Add(new DispatchItem { Date = DateTime.Now, Description = "Hey" });
            
            _wavPlayer = new System.Media.SoundPlayer("alarm.wav");

            Log.Info("Sound alarm initialized");
        }

        public FastCollection<Call> LstMessages=new FastCollection<Call>();

        private Unit _unit;

        private Parser _parser;

        private int _lastMessageCount;

        private System.Media.SoundPlayer _wavPlayer;

        public static List<string> _SubUnits = new List<string>();
        

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
            Log.Info("Reading apiset");
            if (!File.Exists("apiset"))
            {
                Log.Error("apiset is missing");
                MessageBox.Show("apiset file is missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            var data = File.ReadAllText("apiset");
            if (string.IsNullOrEmpty(data))
            {
                Log.Error("apiset is corrupted");
                MessageBox.Show("apiset file is currpted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            _parser = new Parser(data);

            _lastMessageCount = 0;
            
            try
            {
                Log.Info("Reading units");

                var unitName = System.IO.File.ReadAllText("unit.txt");

                _unit = await _parser.GetUnitAsync(unitName);

                Log.Info("Reading subunits");

                var subUnits = File.ReadAllLines("subunits.txt");
                _SubUnits.AddRange(subUnits);

                if(_SubUnits.Count==0)
                {
                    Log.Error("There is no subunits to dispatch.\r\nPlease add subunits before using this software.");
                    MessageBox.Show("There is no subunits to dispatch.\r\nPlease add subunits before using this software.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);

                MessageBox.Show(ex.Message);

                Application.Current.Shutdown();
            }

            if (_unit != null)
                RefreshMessage();
        }


        private void BtnTurnOffAlarm(object sender, RoutedEventArgs e)
        {
            _wavPlayer.Stop();
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
            Log.Info("Getting Calls for unit: "+_unit.Name);

            var list = await _parser.GetCallsAsync(_unit.Name);
            
            
            
            if (list != null && list.Count > 0)
            {
                LstMessages.NotificationOff();

                var turnAlarmOn = false;

                foreach (var item in list)
                {
                    if (!LstMessages.Any(x => x.Id == item.Id))
                    {
                        turnAlarmOn |= string.IsNullOrEmpty(item.Dispatched);
                        LstMessages.Insert(0, item);
                    }
                }

                if (LstMessages.Count > _lastMessageCount)
                {
                    if(turnAlarmOn)
                        _wavPlayer.PlayLooping();
                    _lastMessageCount = LstMessages.Count;
                }

                LstMessages.NotificationOn();
            }
        }

        private async Task CheckMessage()
        {
            Log.Info("Checking messages for unit: "+_unit.Name);

            var unit = await _parser.GetUnitAsync(_unit.Id);

            if(unit.IsRequestOn)
            {
                _wavPlayer.PlayLooping();

                Log.Info("New message received: \r\n" + unit.Message);

                var msgRes = MessageBox.Show(this,unit.Message + "\r\nDo you wish to accept this task?", "New Task Request",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRes == MessageBoxResult.Yes)
                    SendRequest(true);
                else
                    SendRequest(false);

                _wavPlayer.Stop();

                await Task.Delay(1000 * 15);
            }
        }

        private async void SendRequest(bool res)
        {
            try
            {
                string resp = res ? "Accepted" : "Rejected";

                Log.Info(resp + " the request");

                await _parser.AcceptRejectRequest(new UnitAcceptRejectRequestModel { Id = _unit.Id, AcceptRequest = res });

                Log.Info("Response sent successfully");
            }
            catch(Exception ex)
            {
                Log.Error("Error when accepting the request\r\nMessage: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void LstDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstDisplay.SelectedIndex < 0)
                return;


        }

        private async void BtnDispatchUnit_Click(object sender, RoutedEventArgs e)
        {
            if (LstDisplay.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a call from list");
                return;
            }

            var call = LstMessages[LstDisplay.SelectedIndex];

            if(!string.IsNullOrEmpty(call.Dispatched))
            {
                MessageBox.Show("Unit already dispatched for this call.\r\nDispatched Time: " + call.Dispatched);
                return;
            }
            try
            {
                
                var subUnit = new SubUnits() { Owner = this };
                var dlgRes = subUnit.ShowDialog();

                if (dlgRes != true)
                    return;

                Log.Info("Dispatching the subunit " + subUnit.SubUnitSelected + " for callId: " + call.Id);


                var upcall = await _parser.UpdateDispatchTime(new UpdateDispacthTime
                {
                    Id = call.Id,
                    SubUnitAssigned = subUnit.SubUnitSelected
                });

                LstMessages[LstDisplay.SelectedIndex] = upcall;
                LstDisplay.Items.Refresh();

                Log.Info("Subunit dispatched successfully");
            }
            catch(Exception ex)
            {
                Log.Error("Error when dispatching sub unit\r\nMessage: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
