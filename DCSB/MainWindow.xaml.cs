using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DCSB.ViewModels;
using DCSB.Views.SettingsWindow;
using MaterialDesignThemes.Wpf;

namespace DCSB
{
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            System.Drawing.Icon icon;
            using (Stream stream = System.Windows.Application.GetResourceStream(new Uri("icon.ico", UriKind.Relative)).Stream)
            {
                icon = new System.Drawing.Icon(stream);
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

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (splitterGrid.ColumnDefinitions[0].Width.Value != 1)
            {
                splitterGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                splitterGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            DragMove();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is System.Windows.Controls.Primitives.ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private async void HotkeysSetting_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new KeyboardView(), "RootDialog");
        }

        private async void SoundSetting_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new DCSB.Views.SettingsWindow.SoundView(), "RootDialog");
        }

        private async void PresetsSetting_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new PresetConfigurationView(), "RootDialog");
        }

        private async void VisualSetting_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new VisualView(), "RootDialog");
        }

        private async void OtherSetting_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new OtherView(), "RootDialog");
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            //await DialogHost.Show(new (), "RootDialog");
        }
    }
}
