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
        private readonly Notification notification = new Notification();
        private readonly Timer timer = new Timer();

        private Status status = Status.Null;
        private int previousOnline;

        private const string NumberFormat = "###,###";

        public MainWindow()
        {
            InitializeComponent();
            WindowTitle.Initialize(this, WindowTitleStyle.NormalWithPin);

            timer.Elapsed += new ElapsedEventHandler(Timer_ElapsedEvent);
            timer.Interval = 5000;

            UpdateOnline();
            timer.Enabled = true;
        }

        private void Timer_ElapsedEvent(object source, ElapsedEventArgs e)
        {
            UpdateOnline();
        }

        public async void UpdateOnline()
        {
            var serverStatus = await EAWebClient.GetServerStatus();

            string statusText = "";
            int? onlineCount = null;

            switch (serverStatus.Status)
            {
                case Status.Online:
                    statusText = string.Format("Online: {0} (Elyos: {1}%, Asmodian: {2}%)", serverStatus.Online.ToString(NumberFormat), serverStatus.ElyosPercentage, serverStatus.AsmoPercentage);
                    onlineCount = serverStatus.Online;
                    break;
                case Status.Offline:
                    statusText = "SERVER IS OFFLINE";
                    break;
                case Status.ZeroPlayer:
                    statusText = "ZERO ONLINE";
                    onlineCount = 0;
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

                if (status != Status.Null)
                {
                    if (status != serverStatus.Status)
                    {
                        switch (serverStatus.Status)
                        {
                            case Status.Online:
                                notification.ShowInfo("Server is online.", statusText);
                                break;
                            case Status.Offline:
                                notification.ShowWarning("Server is offline.");
                                break;
                            case Status.ZeroPlayer:
                                notification.ShowWarning("Server is online but with 0 player.");
                                break;
                        }
                    }
                    else if (serverStatus.Status == Status.Online && previousOnline > serverStatus.Online)
                    {
                        var difference = previousOnline - serverStatus.Online;
                        if (difference >= 50)
                        {
                            notification.ShowWarning("Mass disconnect detected.", string.Format("{0} players has been disconnected.", difference.ToString(NumberFormat)));
                        }
                    }
                }
            }));


            if (!serverStatus.IsError)
            {
                status = serverStatus.Status;
                previousOnline = serverStatus.Online;
            }
        }
    }
}
