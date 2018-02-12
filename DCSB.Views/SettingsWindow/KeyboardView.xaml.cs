using DCSB.Models;
using DCSB.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DCSB.Views.SettingsWindow
{
    public partial class KeyboardView : UserControl
    {
        public KeyboardView()
        {
            InitializeComponent();
        }

        private async void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ViewModel viewModel && 
                sender is FrameworkElement frameworkElement && 
                frameworkElement.DataContext is IBindable bindable)
            {
                viewModel.BindKeysViewModel.Bindable = bindable;
                BindKeysView view = new BindKeysView
                {
                    DataContext = viewModel.BindKeysViewModel
                };

                await DialogHost.Show(view, "KeyboardViewDialog");
            }
        }
    }
}
