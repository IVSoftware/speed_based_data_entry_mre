using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
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
            Application.Run(MainForm);
            #region L o c a l F x
            void onFormClosed(object sender, FormClosedEventArgs e)
            {
                UIClosedAwaiter.Release();
                ((MainForm)sender).FormClosed -= onFormClosed;
            };
            #endregion L o c a l F x
        }


        [AssemblyCleanup]
        public static async Task WaitForUIClose()
        {
            await UIClosedAwaiter.WaitAsync();
            UIClosedAwaiter.Release();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
