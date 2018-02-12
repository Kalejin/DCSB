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

        public static readonly DependencyProperty ClickProperty = DependencyProperty.RegisterAttached("ClickCommand", typeof(ICommand), typeof(Commands),
              new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveClickEvent)));

        public static ICommand GetClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClickProperty);
        }

        public static void SetClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClickProperty, value);
        }

        public static readonly DependencyProperty ParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(Commands),
              new PropertyMetadata(null));

        public static ICommand GetCommandParameter(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ParameterProperty, value);
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

        public static void AttachOrRemoveClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is Control control)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                {
                    control.PreviewMouseDown += ExecuteClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    control.PreviewMouseDown -= ExecuteClick;
                }
            }
        }

        private static void ExecuteDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                DependencyObject obj = sender as DependencyObject;
                ICommand cmd = (ICommand)obj.GetValue(DoubleClickProperty);
                object parameter = obj.GetValue(ParameterProperty);
                if (cmd != null)
                {
                    if (cmd.CanExecute(parameter))
                    {
                        cmd.Execute(parameter);
                    }
                }
            }
        }

        private static void ExecuteClick(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                DependencyObject obj = sender as DependencyObject;
                ICommand cmd = (ICommand)obj.GetValue(ClickProperty);
                object parameter = obj.GetValue(ParameterProperty);
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
