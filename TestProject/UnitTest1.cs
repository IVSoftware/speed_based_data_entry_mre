using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        // https://stackoverflow.com/a/9085150/5438626
        public static void ManagedSendKeys(string keys)
        {
            SendKeys.SendWait(keys);
            SendKeys.Flush();
        }
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void KeyboardEvent(Keys key, IntPtr windowHandler, int delay)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            Thread.Sleep(delay);
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }

        public static SemaphoreSlim UIReadyAwaiter = new SemaphoreSlim(0, 1);
        public static SemaphoreSlim UIClosedAwaiter = new SemaphoreSlim(0, 1);
        public UnitTest1()
        {
            Boot();
        }
        static MainForm MainForm = null;
        internal static void Boot()
        {
            var uiThread = new Thread(() =>
            {
                Assert.IsTrue(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);

                try
                {
                    createUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Exception{ex.Message}");
                    Debug.Assert(false, ex.Message);
                }
            });
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }

        private static void createUI()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Debug.Assert(false, e.GetType().Name);
            };
            Application.ThreadException += (sender, e) =>
            {
                Debug.Assert(false, e.Exception.Message);
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
            MainForm.FormClosed += onFormClosed;
            MainForm.HandleCreated += onHandleCreated;
            Application.Run(MainForm);
            #region L o c a l F x
            void onFormClosed(object sender, FormClosedEventArgs e)
            {
                UIClosedAwaiter.Release();
                ((MainForm)sender).FormClosed -= onFormClosed;
            };
            void onHandleCreated(object sender, EventArgs e)
            {
                UIReadyAwaiter.Release();
            }
            #endregion L o c a l F x
        }


        [AssemblyCleanup]
        public static async Task WaitForUIClose()
        {
            await UIClosedAwaiter.WaitAsync();
            UIClosedAwaiter.Release();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            await UIReadyAwaiter.WaitAsync();
            MainForm.SendKeyPlusTab("a\t");

            UIReadyAwaiter.Release();
        }
    }
}
