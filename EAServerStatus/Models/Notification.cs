using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace EAServerStatus.Models
{
    public class Notification
    {
        private readonly NotifyIcon _notifyIcon;

        public Notification()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
        }

        private void Show(string title, string message, ToolTipIcon icon)
        {
            if (string.IsNullOrEmpty(title)) title = " ";
            if (string.IsNullOrEmpty(message)) message = " ";

            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }

        public void ShowInfo(string title, string message = "")
        {
            Show(title, message, ToolTipIcon.Info);
        }

        public void ShowWarning(string title, string message = "")
        {
            Show(title, message, ToolTipIcon.Warning);
        }
    }
}
