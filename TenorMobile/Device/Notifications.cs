using System;

using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Tenor.Mobile.Device
{
    /// <summary>
    /// Controls options for notification bubbles.
    /// </summary>
    public class Notification : IDisposable
    {

        RegistryKey key;

        /// <summary>
        /// Opens the given notification id.
        /// </summary>
        /// <param name="id"></param>
        public Notification(Guid id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("ControlPanel\\Notifications\\" + id.ToString("B"), true);
            if (key == null)
                throw new InvalidOperationException("Cannot find the given id");

        }

        /// <summary>
        /// Creates a new notification id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Notification Create(Guid id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (Exists(id))
                throw new InvalidOperationException("The given key already exists.");

            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("ControlPanel\\Notifications", true);
            try
            {
                key.CreateSubKey(id.ToString("B")).Close();
                return new Notification(id);
            }
            finally
            {
                key.Close();
            }
        }

        /// <summary>
        /// Checks if the given notification id already exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exists(Guid id)
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("ControlPanel\\Notifications\\" + id.ToString("B"));
            if (key == null)
                return false;
            else
            {
                key.Close();
                return true;
            }
        
        }

        /// <summary>
        /// A description text of this notification.
        /// </summary>
        public string Text
        {
            get { return (string)key.GetValue(null, string.Empty); }
            set { key.SetValue(null, value, RegistryValueKind.String); }
        }

        /// <summary>
        /// The wave file to play when sounds are enabled.
        /// </summary>
        public string Wave
        {
            get { return (string)key.GetValue("Wave", string.Empty); }
            set { key.SetValue("Wave", value, RegistryValueKind.String); }
        }

        /// <summary>
        /// The duration in minutes.
        /// </summary>
        public int Duration
        {
            get { return (int)key.GetValue("Duration", 0); }
            set { key.SetValue("Duration", value, RegistryValueKind.DWord); }
        }

        /// <summary>
        /// A bit-wise combination off <see cref="NotificationOptions"/>
        /// </summary>
        public NotificationOptions Options
        {
            get { return (NotificationOptions)(int)key.GetValue("Options", 0); }
            set { key.SetValue("Options", (int)value, RegistryValueKind.DWord); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            key.Close();
            key = null;
        }

        #endregion

        public override string ToString()
        {
            return Text;
        }

    }


    [Flags]
    public enum NotificationOptions
    {
        None = 0,
        Sound = 0x00000001,//	Sound notification.
        Vibrate = 0x00000002, // 	Vibrate notification.
        Flash = 0x00000004,// 	ROM notification.
        DisplayBubble = 0x00000008 //	Message notification.
    }

}
