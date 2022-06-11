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
            // A B C
            MainForm.SendKeyPlusTab("a\tb\tc\n");
            //// B A D
            //MainForm.SendKeyPlusTab("b\ta\td\t");
            //// E D A
            //MainForm.SendKeyPlusTab("e\td\ta\t");
            //// D E A
            //MainForm.SendKeyPlusTab("d\te\ta\t");

            UIReadyAwaiter.Release();
        }
    }
}
