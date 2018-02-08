using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DCSB.Interactivity
{
    public static class AttachedProperties
    {
        public static IList GetSelectedItems(ListBox obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(ListBox obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty
            SelectedItemsProperty =
                DependencyProperty.RegisterAttached(
                    "SelectedItems",
                    typeof(IList),
                    typeof(AttachedProperties),
                    new PropertyMetadata(null,
                        SelectedItems_PropertyChanged));

        private static void SelectedItems_PropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is ListBox listBox && e.NewValue is IList newCollection)
            {
                if (newCollection.Count > 0)
                {
                    listBox.SelectedItems.Clear();
                    foreach (object item in newCollection)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                }

                listBox.SelectionChanged += (s, e2) =>
                {
                    if (e2.RemovedItems != null)
                    {
                        foreach (object item in e2.RemovedItems)
                        {
                            newCollection.Remove(item);
                        }
                    }
                    if (e2.AddedItems != null)
                    {
                        foreach (object item in e2.AddedItems)
                        {
                            newCollection.Add(item);
                        }
                    }
                };
            }           
        }
    }
}
