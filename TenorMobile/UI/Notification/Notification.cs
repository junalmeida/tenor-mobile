using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.WindowsCE.Forms;
using NotificationCE = Tenor.Mobile.UI.NotificationWithSoftKeys;
namespace Tenor.Mobile.UI
{
    /// <summary>
    /// Contain methods to show notification messages.
    /// </summary>
    public static class Notification
    {
        private static List<NotificationCE> notifications = new List<NotificationCE>();
        private static List<EventHandler> callbacks = new List<EventHandler>();

        private static void notification_BalloonChanged(object sender, BalloonChangedEventArgs e)
        {
        }


        private static void notification_Disposed(object sender, EventArgs e)
        {
            int index = notifications.IndexOf((NotificationCE)sender);
            callbacks.RemoveAt(index);
            notifications.RemoveAt(index);
        }

        private static void notification_ResponseSubmitted(object sender, ResponseSubmittedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Response) || e.Response.IndexOf("submit") > -1)
            {
                NotificationCE n = (NotificationCE)sender;
                Dismiss(n);
            }
        }

        private static void Dismiss(NotificationCE n)
        {
            int index = notifications.IndexOf(n);
            try
            {
                if (callbacks[index] != null)
                    callbacks[index].Invoke(n, new EventArgs());
            }
            catch
            {
                throw;
            }
            finally
            {
                n.Visible = false;
                n.Dispose();
            }
        }

        static void notification_LeftSoftKeyClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        static void notification_RightSoftKeyClick(object sender, EventArgs e)
        {
            NotificationCE n = (NotificationCE)sender;
            Dismiss(n);
        }

        /// <summary>
        /// Shows a notification to the user.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="critical"></param>
        /// <param name="icon"></param>
        public static void ShowMessage(string text, string caption, bool critical, Icon icon)
        {
            ShowMessage(text, caption, critical, icon, null);
        }
        /// <summary>
        /// Shows a notification to the user.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="critical"></param>
        /// <param name="icon"></param>
        public static void ShowMessage(string text, string caption, bool critical, Icon icon, EventHandler callback)
        {

            NotificationCE notification = new NotificationCE();
            notifications.Add(notification);
            callbacks.Add(callback);

            notification.ResponseSubmitted += new ResponseSubmittedEventHandler(notification_ResponseSubmitted);
            notification.BalloonChanged += new BalloonChangedEventHandler(notification_BalloonChanged);
            notification.Disposed += new EventHandler(notification_Disposed);

            notification.Caption = caption;
            notification.Critical = critical;
            notification.Icon = icon;

            notification.Text = text;

            //otification.Text = string.Format("<form>{0}<input name=submit type=submit value='Dismiss' style='float: right' /></form>", notification.Text);
            notification.LeftSoftKey = new NotificationSoftKey(SoftKeyType.Hide, "Hide");
            notification.RightSoftKey = new NotificationSoftKey(SoftKeyType.StayOpen, "Dismiss");
            //notification.LeftSoftKeyClick += new EventHandler(notification_LeftSoftKeyClick);
            notification.RightSoftKeyClick += new EventHandler(notification_RightSoftKeyClick);


            notification.Visible = true;

        }


    }
}
