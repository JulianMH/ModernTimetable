using System;
using System.Windows;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Stundenplan.Localization;
using Windows.UI.Xaml;
using System.Windows.Input;

namespace Stundenplan
{
    public sealed class ExtendedListView : ListView, INotifyPropertyChanged
    {
        public static readonly DependencyProperty EmptyStringProperty =
             DependencyProperty.Register("EmptyString", typeof(string),
             typeof(ExtendedListView), new PropertyMetadata(null));

        public string EmptyString
        {
            get { return (string)GetValue(EmptyStringProperty); }
            set { SetValue(EmptyStringProperty, value); }
        }

        public static readonly DependencyProperty ItemSelectedCommandProperty =
             DependencyProperty.Register("ItemSelectedCommand", typeof(ICommand),
             typeof(ExtendedListView), new PropertyMetadata(null));

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        /// <summary>
        /// Das Template für die leere Listbox. Kann für Anspruchsvollere Szenarios verändert werden.
        /// </summary>
        public ControlTemplate EmptyTemplate { set; get; }

        /// <summary>
        /// Event, tritt auf wenn eine Eigenschaft dieses Objektes sich ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Feuere das Event PropertyChanged, sofern dieses gültig ist.
        /// </summary>
        /// <param name="property">Die Eigenschaft, die sich geändert hat</param>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        ControlTemplate defaultTemplate;

        public ExtendedListView()
        {
            this.EmptyString = Strings.ListBoxEmptyStringDefault;
            this.EmptyTemplate = (ControlTemplate)App.Current.Resources["ExtendedListViewEmptyTemplate"];

            this.IsItemClickEnabled = true;
            this.ItemClick += ExtendedListView_ItemClick;

            ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Visible);
        }

        void ExtendedListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ItemSelectedCommand != null)
                ItemSelectedCommand.Execute(e.ClickedItem);
        }

        protected override void OnItemsChanged(object e)
        {
            if (defaultTemplate == null)
                defaultTemplate = this.Template;

            this.Template = this.Items.Count > 0 ? defaultTemplate : this.EmptyTemplate;

            base.OnItemsChanged(e);
        }
    }
}
