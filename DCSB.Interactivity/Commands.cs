using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DCSB.Interactivity
{
    public static class Commands
    {
        public static readonly DependencyProperty DoubleClickProperty = DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(Commands),
                      new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDoubleClickEvent)));

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickProperty, value);
        }

        public static readonly DependencyProperty DoubleClickParameterProperty = DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(Commands),
              new PropertyMetadata(null));

        public static ICommand GetDoubleClickCommandParameter(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickParameterProperty);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickParameterProperty, value);
        }

        public static void AttachOrRemoveDoubleClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is Control control)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                {
                    control.MouseDoubleClick += ExecuteDoubleClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    control.MouseDoubleClick -= ExecuteDoubleClick;
                }
            }
        }

        private static void ExecuteDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                DependencyObject obj = sender as DependencyObject;
                ICommand cmd = (ICommand)obj.GetValue(DoubleClickProperty);
                object parameter = obj.GetValue(DoubleClickParameterProperty);
                if (cmd != null)
                {
                    if (cmd.CanExecute(parameter))
                    {
                        cmd.Execute(parameter);
                    }
                }
            }
        }
    }
}
