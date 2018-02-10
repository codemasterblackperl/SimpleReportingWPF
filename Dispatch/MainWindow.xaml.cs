using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
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

            _lstCalls = new List<Call>();
        }

        public FastCollection<CallDisplay> LstMessages=new FastCollection<CallDisplay>();

        private List<Call> _lstCalls;

        private bool _callInsertlock = false;

        private Parser _parser;

        private int _lastMessageCount;

        private System.Media.SoundPlayer _wavPlayer;

        public static List<string> _SubUnits = new List<string>();

        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;

        private object _lockObject = new object();
        private bool _getCallOn = false;



        private void LstDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstDisplay.SelectedIndex< 0)
                return;
            CallDisplay item = (CallDisplay) LstDisplay.SelectedItem;
            try
            {
                var call = _lstCalls.Single(x => x.CallId == item.CallId);
                GrdDocumnet.DataContext = call;
            }
            catch
            {
                GrdDocumnet.DataContext = null;
            }
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

            //Call item = (Call)LstDisplay.SelectedItem;
            var call = _lstCalls[LstDisplay.SelectedIndex];

            var report = new Report_Viewer() { Cal = call };

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

            //var callD = LstMessages[LstDisplay.SelectedIndex];

            //var call = _lstCalls.Single(x => x.CallId == callD.CallId);
            var call = _lstCalls[LstDisplay.SelectedIndex];

            var team = call.UnitsAssigned.Single(x => x.UnitName == Shared._Unit.Name);

            if (team.Dispatched != null)
            {
                MessageBox.Show("Unit already dispatched for this call.\r\nDispatched Time: " + team.Dispatched);
                return;
            }
            try
            {

                var subUnit = new SubUnits() { Owner = this };
                var dlgRes = subUnit.ShowDialog();

                if (dlgRes != true)
                    return;

                Logger.Log.Info("Dispatching the subunit " + subUnit.SubUnitSelected + " for callId: " + call.CallId);


                var upcall = await Shared._Parser.UpdateDispatchTime(new UpdateDispacthTime
                {
                    CallId = call.CallId,
                    TeamId = team.TeamId,
                    SubUnitAssigned = subUnit.SubUnitSelected
                });

                _lstCalls[LstDisplay.SelectedIndex] = upcall;

                var tt = upcall.UnitsAssigned.Single(x => x.TeamId == team.TeamId);

                LstMessages[LstDisplay.SelectedIndex].DispatchedTime = tt.Dispatched;
                LstDisplay.Items.Refresh();

                Logger.Log.Info("Subunit dispatched successfully");

                MessageBox.Show("Unit successfully dispatched");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error when dispatching sub unit\r\nMessage: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_getCallOn)
                return;

            GetCalls();
        }


        //private async void RefreshMessage()
        //{
        //    int count = 0;
        //    for(; ; )
        //    {
        //        if (count == 3)
        //        {

        //            await GetCalls();
        //            count = 0;
        //            continue;
        //        }

        //        await Task.Delay(1000 * 10);
        //        count++;
        //    }
        //}


        //private async Task LoadUnit()
        //{
        //    string unitName=null;
        //    if(!File.Exists(Logger._AppDir+"\\unit.txt"))
        //    {
        //        unitName = await GetUnit();
        //    }
        //    else
        //    {
        //        unitName = File.ReadAllText(Logger._AppDir + "\\unit.txt");
        //    }

        //    if(string.IsNullOrEmpty(unitName))
        //    {
        //        File.Delete(Logger._AppDir + "\\unit.txt");
        //        throw new Exception("unit file is corrupted. Null error");
        //    }

        //    Logger.Log.Info("Reading units");

        //    try
        //    {

        //        _unit = await _parser.GetUnitAsync(unitName);

        //        if (_unit==null)
        //        {
        //            //MessageBox.Show("Error when retreaving unit information", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            throw new Exception("Error when retreaving unit information");
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        Logger.Log.Error(ex);

        //        throw new Exception(ex.Message);

        //        //MessageBox.Show(ex.Message);
        //    }

        //}

        //private async Task<string> GetUnit()
        //{
        //    var units = await _parser.GetAllUnitNames();

        //    if (units == null)
        //    {
        //        //MessageBox.Show("Server doesnt have any units yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //        throw new Exception("Server doesnt have any units to display");
        //    }

        //    var su = new SelectUnit(units) { Owner = this };
        //    var res = su.ShowDialog();
        //    if (res != true)
        //    {
        //        throw new Exception("Null unit error");
        //    }

        //    File.WriteAllText(Logger._AppDir + "\\unit.txt", su.SelectedUnit);

        //    return su.SelectedUnit;
        //}

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
            try
            {
                Logger.Log.Info("Getting Calls for unit: " + Shared._Unit.Name);
                _getCallOn = true;

                var list = await Shared._Parser.GetCallsAsync(Shared._Unit.Name);

                if (list != null && list.Count > 0)
                {
                    LstMessages.NotificationOff();

                    var turnAlarmOn = false;

                    foreach (var item in list)
                    {
                        if (!LstMessages.Any(x => x.CallId == item.CallId))
                        {
                            var team = item.UnitsAssigned.Single(x => x.UnitName == Shared._Unit.Name);
                            if (team == null)
                                continue;
                            turnAlarmOn = team.Dispatched == null ? true : false;
                            if (_callInsertlock)
                                break;
                            lock (_lockObject)
                            {
                                _lstCalls.Insert(0, item);
                                LstMessages.Insert(0, new CallDisplay
                                {
                                    CallId = item.CallId,
                                    CallReceivedTime = item.CallReceivedTime,
                                    EmergencyType = item.EmergencyType,
                                    IncidentType = item.IncidentType,
                                    DispatchedTime = team.Dispatched
                                });
                            }
                        }
                    }

                    if (LstMessages.Count > _lastMessageCount)
                    {
                        if (turnAlarmOn)
                            _wavPlayer.PlayLooping();
                        _lastMessageCount = LstMessages.Count;
                    }

                    
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex);
            }
            finally
            {
                _getCallOn = false;
                LstMessages.NotificationOn();
            }
        }


        private async void LoadLoginScreen()
        {
            try
            {
                GrdMain.Visibility = Visibility.Hidden;
                var login = new Login() { Owner = this };
                if (login.ShowDialog() != true)
                    Application.Current.Shutdown();
                login.Close();
                GrdMain.Visibility = Visibility.Visible;
                await LoadUnitDetail();
                await GetCalls();
                LoadSubUnits();
                InitHub();
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex);
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

        private async Task LoadUnitDetail()
        {
            try
            {
                Logger.Log.Info("Getting unit details for userName: " + Shared.UserName);
                Shared._Unit = await Shared._Parser.GetUnitAsync(Shared.UserName);
                LblSelectedUnit.Content = Shared._Unit.Name;
                //this.Dispatcher.Invoke(() => LblSelectedUnit.Content = Shared._Unit.Name);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error when getting unit details for userName: " + Shared.UserName);
                Logger.Log.Error("Error Message: " + ex);
                MessageBox.Show(ex.Message);
            }
        }

        private async void InitHub()
        {
            Logger.Log.Info("Intialzing signalr connection");
            _hubConnection = new HubConnection(Shared._ApiUrl);
            _hubConnection.Headers.Add("Authorization", Shared._Parser.AuthString);
            _hubProxy = _hubConnection.CreateHubProxy("NotificationHub");
            _hubProxy.Subscribe("Notify").Received += HubNotificationReceiver;
            await _hubConnection.Start();
            Logger.Log.Info("signalr connection started");
        }

        private void HubNotificationReceiver(IList<Newtonsoft.Json.Linq.JToken> obj)
        {
            _callInsertlock = true;
            try
            {
                foreach (var item in obj)
                {
                    Call call = JsonConvert.DeserializeObject<Call>(item.ToString()); //item.ToObject<Call>();
                    Dispatcher.Invoke(() =>
                    {
                        lock (_lockObject)
                        {
                            _lstCalls.Insert(0, call);
                            LstMessages.Insert(0, new CallDisplay
                            {
                                CallId = call.CallId,
                                CallReceivedTime = call.CallReceivedTime,
                                EmergencyType = call.EmergencyType,
                                IncidentType = call.IncidentType,
                            });
                        }
                        _wavPlayer.PlayLooping();
                    });
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error("Error when inserting an item into list\r\n"+obj);
                Logger.Log.Error(ex);
            }
            finally
            {
                _callInsertlock = false;
            }
        }

       
    }
}
