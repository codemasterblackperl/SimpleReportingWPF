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
    /// Interaction Logger.Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            
            InitializeComponent();

            //BindingOperations.EnableCollectionSynchronization(LstMessages, _personCollectionLock);

            Logger.Log.Info("Application Started");

            LstDisplay.ItemsSource = LstMessages;

            //LstMessages.Add(new DispatchItem { Date = DateTime.Now, Description = "Hey" });
            
            _wavPlayer = new System.Media.SoundPlayer("alarm.wav");

            Logger.Log.Info("Sound alarm initialized");

            Shared.InitApiSet();

            //LoadLoginScreen();


        }

        public FastCollection<Call> LstMessages=new FastCollection<Call>();

        private Unit _unit;

        private Parser _parser;

        private int _lastMessageCount;

        private System.Media.SoundPlayer _wavPlayer;

        public static List<string> _SubUnits = new List<string>();

        private string _authToken = "";

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
            //try
            //{
            //    //LoadApiSet();

            //    _lastMessageCount = 0;

            //    //await LoadUnit();

            //    LoadSubUnits();

            //    LblSelectedUnit.Content = "Unit Name: " + _unit.Name;
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Application.Current.Shutdown();
            //}

            //    if (_unit != null)
            //        RefreshMessage();
            LoadLoginScreen();
            
        }


        private void BtnTurnOffAlarm(object sender, RoutedEventArgs e)
        {
            //new SubUnits().ShowDiaLogger.Log();
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

            if (call.Dispatched!=null)
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

                Logger.Log.Info("Dispatching the subunit " + subUnit.SubUnitSelected + " for callId: " + call.Id);


                var upcall = await _parser.UpdateDispatchTime(new UpdateDispacthTime
                {
                    Id = call.Id,
                    SubUnitAssigned = subUnit.SubUnitSelected
                });

                LstMessages[LstDisplay.SelectedIndex] = upcall;
                LstDisplay.Items.Refresh();

                Logger.Log.Info("Subunit dispatched successfully");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error when dispatching sub unit\r\nMessage: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
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
        

        private async Task LoadUnit()
        {
            string unitName=null;
            if(!File.Exists(Logger._AppDir+"\\unit.txt"))
            {
                unitName = await GetUnit();
            }
            else
            {
                unitName = File.ReadAllText(Logger._AppDir + "\\unit.txt");
            }

            if(string.IsNullOrEmpty(unitName))
            {
                File.Delete(Logger._AppDir + "\\unit.txt");
                throw new Exception("unit file is corrupted. Null error");
            }

            Logger.Log.Info("Reading units");

            try
            {

                _unit = await _parser.GetUnitAsync(unitName);

                if (_unit==null)
                {
                    //MessageBox.Show("Error when retreaving unit information", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception("Error when retreaving unit information");
                }

            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex);

                throw new Exception(ex.Message);

                //MessageBox.Show(ex.Message);
            }

        }

        private async Task<string> GetUnit()
        {
            var units = await _parser.GetAllUnitNames();

            if (units == null)
            {
                //MessageBox.Show("Server doesnt have any units yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                throw new Exception("Server doesnt have any units to display");
            }

            var su = new SelectUnit(units) { Owner = this };
            var res = su.ShowDialog();
            if (res != true)
            {
                throw new Exception("Null unit error");
            }

            File.WriteAllText(Logger._AppDir + "\\unit.txt", su.SelectedUnit);

            return su.SelectedUnit;
        }

        private void LoadSubUnits()
        {

            if(!File.Exists("subunits.txt"))
            {
                //MessageBox.Show("Subunits file is missing or corrupted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("Subunit file is missing or corrupted");
            }

            Logger.Log.Info("Reading subunits");

            var subUnits = File.ReadAllLines("subunits.txt");
            _SubUnits.AddRange(subUnits);

            if (_SubUnits.Count == 0)
            {
                Logger.Log.Error("There is no subunits to dispatch.\r\nPlease add subunits before using this software.");
                //MessageBox.Show("There is no subunits to dispatch.\r\nPlease add subunits before using this software.",
                //    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("Subunit file is missing or corrupted");
            }
        }


        private async Task GetCalls()
        {
            Logger.Log.Info("Getting Calls for unit: "+_unit.Name);

            var list = await _parser.GetCallsAsync(_unit.Name);
            
            
            
            if (list != null && list.Count > 0)
            {
                LstMessages.NotificationOff();

                var turnAlarmOn = false;

                foreach (var item in list)
                {
                    if (!LstMessages.Any(x => x.Id == item.Id))
                    {
                        turnAlarmOn = item.Dispatched==null?true:false;
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
            Logger.Log.Info("Checking messages for unit: "+_unit.Name);

            var unit = await _parser.GetUnitAsync(_unit.Id);

            if(unit.IsRequestOn)
            {
                _wavPlayer.PlayLooping();

                Logger.Log.Info("New message received: \r\n" + unit.Message);

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

                Logger.Log.Info(resp + " the request");

                await _parser.AcceptRejectRequest(new UnitAcceptRejectRequestModel { Id = _unit.Id, AcceptRequest = res });

                Logger.Log.Info("Response sent successfully");
            }
            catch(Exception ex)
            {
                Logger.Log.Error("Error when accepting the request\r\nMessage: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }


        private void LoadLoginScreen()
        {
            try
            {
                GrdMain.Visibility = Visibility.Hidden;
                var login = new Login() { Owner = this };
                if (login.ShowDialog() != true)
                    Application.Current.Shutdown();
                login.Close();
                GrdMain.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {

            }
        }

        
    }
}
