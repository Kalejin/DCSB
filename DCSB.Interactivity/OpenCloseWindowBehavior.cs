using System;
using System.Windows;
using System.Windows.Interactivity;

namespace DCSB.Interactivity
{
    public class OpenCloseWindowBehavior : Behavior<Window>
    {
        private Window _windowInstance;

        public Type WindowType
        {
            get { return (Type)GetValue(WindowTypeProperty); }
            set { SetValue(WindowTypeProperty, value); }
        }
        public static readonly DependencyProperty WindowTypeProperty = DependencyProperty.Register("WindowType", typeof(Type), typeof(OpenCloseWindowBehavior), new PropertyMetadata(null));

        public bool Open
        {
            get { return (bool)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }
        public static readonly DependencyProperty OpenProperty = DependencyProperty.Register("Open", typeof(bool), typeof(OpenCloseWindowBehavior), new PropertyMetadata(false, OnOpenChanged));

        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), typeof(OpenCloseWindowBehavior), new PropertyMetadata(null));

        private static void OnOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OpenCloseWindowBehavior me = (OpenCloseWindowBehavior)d;
            if ((bool)e.NewValue)
            {
                object instance = Activator.CreateInstance(me.WindowType);
                if (instance is Window window)
                {
                    window.Owner = me.AssociatedObject;
                    window.DataContext = me.DataContext;
                    window.Closing += (s, ev) =>
                    {
                        if (me.Open)
                        {
                            me._windowInstance = null;
                            me.Open = false;
                        }
                    };
                    me._windowInstance = window;
                    window.ShowDialog();
                }
                else
                {
                    throw new ArgumentException(string.Format("Type '{0}' does not derive from System.Windows.Window.", me.WindowType));
                }
            }
            else
            {
                if (me._windowInstance != null)
                    me._windowInstance.Close();
            }
        }
    }
}
