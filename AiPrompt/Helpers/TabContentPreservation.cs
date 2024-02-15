using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace AiPrompt.Helpers;

//public class TabContentPreservation
//{
//    public static bool GetIsContentPreserved(DependencyObject obj)
//    {
//        return (bool)obj.GetValue(IsContentPreservedProperty);
//    }

//    public static void SetIsContentPreserved(DependencyObject obj, bool value)
//    {
//        obj.SetValue(IsContentPreservedProperty, value);
//    }

//    public static readonly DependencyProperty IsContentPreservedProperty =
//        DependencyProperty.RegisterAttached("IsContentPreserved", typeof(bool), typeof(TabContentPreservation), new PropertyMetadata(false, OnIsContentPreservedChanged));


//    private static void OnIsContentPreservedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
//    {
//        var tabControl = obj as TabControl;
//        if ((bool)e.NewValue)
//        {
//            var manager = new TabContentManager(tabControl);
//            manager.Start();
//            SetTabContentManager(tabControl, manager);
//        }
//    }

//}


//public class TabViewCache : TabView
//{
//    private readonly List<object> cache = new();

//    public TabViewCache()
//    {
//        ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
//    }

//    private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
//    {
//        Panel
//    }



//    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
//    {
//        if (newValue is INotifyCollectionChanged notify)
//        {
//            notify.CollectionChanged += Notify_CollectionChanged;
//        }
//        base.OnItemsSourceChanged(oldValue, newValue);
//    }

//    private void Notify_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
//    {
//        if (e.Action is NotifyCollectionChangedAction.Reset)
//        {
//            cache.Clear();
//        }
//        else if (e.Action is NotifyCollectionChangedAction.Add)
//        {
//            if (e.NewItems is [var item and not null])
//            {
//                cache.Add(item);
//            }
//        }
//        else if (e.Action is NotifyCollectionChangedAction.Replace)
//        {

//        }
//        else if (e.Action is NotifyCollectionChangedAction.Remove)
//        {

//        }
//    }



//    private class CacheItem(object item)
//    {
//        public object Item { get; set; } = item;
//        public object? Cache { get; set; }
//    }
//}





//[TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
//public class TabViewCache : TabView
//{
//    // Holds all items, but only marks the current tab's item as visible
//    private Panel? _itemsHolder = null;

//    // Temporaily holds deleted item in case this was a drag/drop operation
//    private object? _deletedObject = null;

//    public TabViewCache()
//        : base()
//    {
//        // this is necessary so that we get the initial databound selected item
//        this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
//    }

//    /// <summary>
//    /// if containers are done, generate the selected item
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
//    {
//        if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
//        {
//            this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
//            UpdateSelectedItem();
//        }
//    }

//    /// <summary>
//    /// get the ItemsHolder and generate any children
//    /// </summary>
//    public override void OnApplyTemplate()
//    {
//        base.OnApplyTemplate();
//        _itemsHolder = GetTemplateChild("PART_ItemsHolder") as Panel;
//        UpdateSelectedItem();
//    }

//    /// <summary>
//    /// when the items change we remove any generated panel children and add any new ones as necessary
//    /// </summary>
//    /// <param name="e"></param>
//    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
//    {
//        base.OnItemsChanged(e);

//        if (_itemsHolder == null)
//        {
//            return;
//        }

//        switch (e.Action)
//        {
//            case NotifyCollectionChangedAction.Reset:
//                _itemsHolder.Children.Clear();

//                if (base.Items.Count > 0)
//                {
//                    base.SelectedItem = base.Items[0];
//                    UpdateSelectedItem();
//                }

//                break;

//            case NotifyCollectionChangedAction.Add:
//            case NotifyCollectionChangedAction.Remove:

//                // Search for recently deleted items caused by a Drag/Drop operation
//                if (e.NewItems != null && _deletedObject != null)
//                {
//                    foreach (var item in e.NewItems)
//                    {
//                        if (_deletedObject == item)
//                        {
//                            // If the new item is the same as the recently deleted one (i.e. a drag/drop event)
//                            // then cancel the deletion and reuse the ContentPresenter so it doesn't have to be
//                            // redrawn. We do need to link the presenter to the new item though (using the Tag)
//                            ContentPresenter cp = FindChildContentPresenter(_deletedObject);
//                            if (cp != null)
//                            {
//                                int index = _itemsHolder.Children.IndexOf(cp);

//                                (_itemsHolder.Children[index] as ContentPresenter).Tag =
//                                    (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
//                            }
//                            _deletedObject = null;
//                        }
//                    }
//                }

//                if (e.OldItems != null)
//                {
//                    foreach (var item in e.OldItems)
//                    {

//                        _deletedObject = item;

//                        // We want to run this at a slightly later priority in case this
//                        // is a drag/drop operation so that we can reuse the template
//                        this.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
//                            new Action(delegate ()
//                            {
//                                if (_deletedObject != null)
//                                {
//                                    ContentPresenter cp = FindChildContentPresenter(_deletedObject);
//                                    if (cp != null)
//                                    {
//                                        this._itemsHolder.Children.Remove(cp);
//                                    }
//                                }
//                            }
//                        ));
//                    }
//                }

//                UpdateSelectedItem();
//                break;

//            case NotifyCollectionChangedAction.Replace:
//                throw new NotImplementedException("Replace not implemented yet");
//        }
//    }

//    /// <summary>
//    /// update the visible child in the ItemsHolder
//    /// </summary>
//    /// <param name="e"></param>
//    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
//    {
//        base.OnSelectionChanged(e);
//        UpdateSelectedItem();
//    }

//    /// <summary>
//    /// generate a ContentPresenter for the selected item
//    /// </summary>
//    void UpdateSelectedItem()
//    {
//        if (_itemsHolder == null)
//        {
//            return;
//        }

//        // generate a ContentPresenter if necessary
//        TabItem item = GetSelectedTabItem();
//        if (item != null)
//        {
//            CreateChildContentPresenter(item);
//        }

//        // show the right child
//        foreach (ContentPresenter child in _itemsHolder.Children)
//        {
//            child.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
//        }
//    }

//    /// <summary>
//    /// create the child ContentPresenter for the given item (could be data or a TabItem)
//    /// </summary>
//    /// <param name="item"></param>
//    /// <returns></returns>
//    ContentPresenter CreateChildContentPresenter(object item)
//    {
//        if (item == null)
//        {
//            return null;
//        }

//        ContentPresenter cp = FindChildContentPresenter(item);

//        if (cp != null)
//        {
//            return cp;
//        }

//        // the actual child to be added.  cp.Tag is a reference to the TabItem
//        cp = new ContentPresenter();
//        cp.Content = (item is TabItem) ? (item as TabItem).Content : item;
//        cp.ContentTemplate = this.SelectedContentTemplate;
//        cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
//        cp.ContentStringFormat = this.SelectedContentStringFormat;
//        cp.Visibility = Visibility.Collapsed;
//        cp.Tag = (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
//        _itemsHolder.Children.Add(cp);
//        return cp;
//    }

//    /// <summary>
//    /// Find the CP for the given object.  data could be a TabItem or a piece of data
//    /// </summary>
//    /// <param name="data"></param>
//    /// <returns></returns>
//    ContentPresenter FindChildContentPresenter(object data)
//    {
//        if (data is TabItem)
//        {
//            data = (data as TabItem).Content;
//        }

//        if (data == null)
//        {
//            return null;
//        }

//        if (_itemsHolder == null)
//        {
//            return null;
//        }

//        foreach (ContentPresenter cp in _itemsHolder.Children)
//        {
//            if (cp.Content == data)
//            {
//                return cp;
//            }
//        }

//        return null;
//    }

//    /// <summary>
//    /// copied from TabControl; wish it were protected in that class instead of private
//    /// </summary>
//    /// <returns></returns>
//    protected TabItem GetSelectedTabItem()
//    {
//        object selectedItem = base.SelectedItem;
//        if (selectedItem == null)
//        {
//            return null;
//        }

//        if (_deletedObject == selectedItem)
//        {

//        }

//        TabItem item = selectedItem as TabItem;
//        if (item == null)
//        {
//            item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;
//        }
//        return item;
//    }
//}


