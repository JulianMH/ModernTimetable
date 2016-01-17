using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Stundenplan
{
    public class ItemsHub : Hub
    {
        public ItemsHub()
        {
            this.SectionsInViewChanged += ItemsHub_SectionsInViewChanged;
        }

        void ItemsHub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            this.SelectedItem = this.SectionsInView.Any() ? this.SectionsInView.First().DataContext : null;
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsHub), new PropertyMetadata(null, ItemTemplateChanged));

        private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsHub hub = d as ItemsHub;
            if (hub != null)
            {
                DataTemplate template = e.NewValue as DataTemplate;
                if (template != null)
                {
                    // Apply template
                    foreach (var section in hub.Sections)
                    {
                        section.ContentTemplate = template;
                    }
                }
            }
        }
        
        public object DefaultItem
        {
            get { return (object)GetValue(DefaultItemProperty); }
            set { SetValue(DefaultItemProperty, value); }
        }

        public static readonly DependencyProperty DefaultItemProperty =
            DependencyProperty.Register("DefaultItem", typeof(object), typeof(ItemsHub), new PropertyMetadata(null, DefaultItemChanged));

        private static void DefaultItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsHub hub = d as ItemsHub;
            if (hub != null)
            {
                if (e.NewValue != null)
                {
                    hub.ScrollToSection(hub.Sections.FirstOrDefault(p => p.DataContext == e.NewValue));
                }
            }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ItemsHub), new PropertyMetadata(null, SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public DataTemplate ItemHeaderTemplate
        {
            get { return (DataTemplate)GetValue(ItemHeaderTemplateProperty); }
            set { SetValue(ItemHeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemHeaderTemplateProperty =
            DependencyProperty.Register("ItemHeaderTemplate", typeof(DataTemplate), typeof(ItemsHub), new PropertyMetadata(null, ItemHeaderTemplateChanged));

        private static void ItemHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsHub hub = d as ItemsHub;
            if (hub != null)
            {
                DataTemplate template = e.NewValue as DataTemplate;
                if (template != null)
                {
                    // Apply template
                    foreach (var section in hub.Sections)
                    {
                        section.HeaderTemplate = template;
                    }
                }
            }
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(ItemsHub), new PropertyMetadata(null, ItemsSourceChanged));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsHub hub = d as ItemsHub;
            if (hub != null)
            {
                IList items = e.NewValue as IList;
                if (items != null)
                {
                    hub.Sections.Clear();
                    foreach (var item in items)
                    {
                        HubSection section = new HubSection();
                        section.DataContext = item;
                        section.Header = item;

                        section.ContentTemplate = hub.ItemTemplate;
                        section.HeaderTemplate = hub.ItemHeaderTemplate;
                        hub.Sections.Add(section);
                    }

                    if (hub.DefaultItem != null && hub.Sections.Any(p => p.DataContext == hub.DefaultItem))
                        hub.ScrollToSection(hub.Sections.FirstOrDefault(p => p.DataContext == hub.DefaultItem));
                }
            }
        }

    }
}
