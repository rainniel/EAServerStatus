using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EAServerStatus.Controls
{
    public partial class TitleBar : UserControl
    {
        private const string PinMessage = "Pin window on top of the screen";
        private const string UnpinMessage = "Un-pin window on top of the screen";
        private const string MaximizeMessage = "Maximize";
        private const string RestoreMessage = "Restore";

        private Window _parentWindow;
        private bool _canMaximize;

        public TitleBar()
        {
            InitializeComponent();
        }

        #region Control Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Movable && e.ChangedButton == MouseButton.Left)
            {
                try { _parentWindow.DragMove(); }
                catch { }
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) BtnMaximize_Click(sender, e);
        }

        private void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            PinWindow(!_parentWindow.Topmost);
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (!_canMaximize) return;

            if (_parentWindow.WindowState == WindowState.Normal)
            {
                TbkMaximize.Text = RestoreMessage;
                _parentWindow.ResizeMode = ResizeMode.NoResize;
                _parentWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                TbkMaximize.Text = MaximizeMessage;
                _parentWindow.ResizeMode = ResizeMode.CanResize;
                _parentWindow.WindowState = WindowState.Normal;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.Close();
        }

        #endregion

        #region Public Properties

        public string Title
        {
            get => LblTitle.Content.ToString();
            set => LblTitle.Content = value;
        }

        public bool Pinned
        {
            get => _parentWindow.Topmost = true;
            set => PinWindow(value);
        }

        public void TogglePin()
        {
            PinWindow(!_parentWindow.Topmost);
        }

        public bool Movable { get; set; } = true;

        #endregion

        #region Misc

        public void Initialize(Window window, WindowTitleStyle style)
        {
            _parentWindow = window;
            _parentWindow.MouseDown += Window_MouseDown;
            SetStyle(style);
        }

        public void SetStyle(WindowTitleStyle style)
        {
            switch (style)
            {
                case WindowTitleStyle.Normal:
                    BtnPin.Visibility = Visibility.Collapsed;
                    BtnMinimize.Visibility = Visibility.Visible;
                    BtnMaximize.Visibility = Visibility.Visible;
                    BtnClose.Visibility = Visibility.Visible;
                    _canMaximize = true;
                    break;

                case WindowTitleStyle.NormalWithPin:
                    BtnPin.Visibility = Visibility.Visible;
                    BtnMinimize.Visibility = Visibility.Visible;
                    BtnMaximize.Visibility = Visibility.Visible;
                    BtnClose.Visibility = Visibility.Visible;
                    _canMaximize = true;
                    break;

                case WindowTitleStyle.Dialog:
                    BtnPin.Visibility = Visibility.Collapsed;
                    BtnMinimize.Visibility = Visibility.Collapsed;
                    BtnMaximize.Visibility = Visibility.Collapsed;
                    BtnClose.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void PinWindow(bool pin)
        {
            if (pin)
            {
                TbkPin.Text = UnpinMessage;
                PthPin.Data = (Geometry)FindResource("UnpinGeometry");
                PthPin.Margin = new Thickness(2, 3, 3, 3);
                _parentWindow.Topmost = true;
            }
            else
            {
                TbkPin.Text = PinMessage;
                PthPin.Data = (Geometry)FindResource("PinGeometry");
                PthPin.Margin = new Thickness(4, 3, 4, 3);
                _parentWindow.Topmost = false;
            }
        }

        #endregion
    }

    public enum WindowTitleStyle
    {
        Normal,
        NormalWithPin,
        Dialog
    }
}
