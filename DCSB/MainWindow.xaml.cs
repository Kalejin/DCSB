using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DCSB.ViewModels;

namespace DCSB
{
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DataContext = new ViewModel();

            Icon icon;
            using (Stream stream = System.Windows.Application.GetResourceStream(new Uri("icon.ico", UriKind.Relative)).Stream)
            {
                icon = new Icon(stream);
            }

            MenuItem open = new MenuItem("Open", (sender, e) => Open());
            MenuItem exit = new MenuItem("Exit", (sender, e) => Close());

            notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "Deathcounter and Soundboard",
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[] { open, exit })
            };

            notifyIcon.Click += (sender, args) => Open();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ExceptionObject.ToString(), "Unhandled exception");
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (DataContext is ViewModel viewModel && viewModel.ConfigurationModel.MinimizeToTray && WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (notifyIcon != null)
                notifyIcon.Dispose();

            base.OnClosed(e);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel viewModel)
            {
                viewModel.WindowHandle = new WindowInteropHelper(this).Handle;
            }
        }

        private void Open()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }
    }
}
