using EAServerStatus.Controls;
using EAServerStatus.Models;
using EAServerStatus.Utils;
using System;
using System.Timers;
using System.Windows;

namespace EAServerStatus
{
    public partial class MainWindow : Window
    {
        private const string NumberFormat = "###,###";

        private readonly Notification _notification = new Notification();
        private readonly Timer _timer = new Timer();

        private Status _status = Status.Null;
        private int _previousOnline;

        public MainWindow()
        {
            InitializeComponent();
            WindowTitle.Initialize(this, WindowTitleStyle.NormalWithPin);

            _timer.Elapsed += new ElapsedEventHandler(Timer_ElapsedEvent);
            _timer.Interval = 5000;

            UpdateOnline();
            _timer.Enabled = true;
        }

        private void Timer_ElapsedEvent(object source, ElapsedEventArgs e)
        {
            UpdateOnline();
        }

        public async void UpdateOnline()
        {
            var serverStatus = await EAWebClient.GetServerStatus();

            var statusText = "";
            int? onlineCount = null;

            switch (serverStatus.Status)
            {
                case Status.Online:
                    statusText = string.Format("Online: {0} (Elyos: {1}%, Asmodian: {2}%)", serverStatus.Online.ToString(NumberFormat), serverStatus.ElyosPercentage, serverStatus.AsmoPercentage);
                    onlineCount = serverStatus.Online;
                    break;
                case Status.ZeroPlayer:
                    statusText = "ZERO ONLINE";
                    break;
                case Status.Maintenance:
                    statusText = "SERVER MAINTENANCE";
                    break;
                case Status.DataError:
                    statusText = "DATA ERROR";
                    break;
                case Status.ServerError:
                    statusText = "SERVER ERROR";
                    break;
                case Status.RequestError:
                    statusText = "REQUEST ERROR";
                    break;
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TxtStatus.Text = statusText;
                ChartOnline.AddOnline(onlineCount);

                if (_status != Status.Null)
                {
                    if (_status != serverStatus.Status)
                    {
                        switch (serverStatus.Status)
                        {
                            case Status.Online:
                                _notification.ShowInfo("Server is online.", statusText);
                                break;
                            case Status.ZeroPlayer:
                                _notification.ShowWarning("Server is online but with 0 player.");
                                break;
                            case Status.Maintenance:
                                _notification.ShowWarning("Server is under maintenance.");
                                break;
                        }
                    }
                    else if (serverStatus.Status == Status.Online && _previousOnline > serverStatus.Online)
                    {
                        var difference = _previousOnline - serverStatus.Online;
                        if (difference >= 50)
                        {
                            _notification.ShowWarning("Mass disconnect detected.", string.Format("{0} players has been disconnected.", difference.ToString(NumberFormat)));
                        }
                    }
                }
            }));

            if (!serverStatus.IsError)
            {
                _status = serverStatus.Status;
                _previousOnline = serverStatus.Online;
            }
        }
    }
}
