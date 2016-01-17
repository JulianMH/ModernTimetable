using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Stundenplan
{
    public sealed partial class LessonControl : UserControl
    {
        public ICommand EditLessonCommand
        {
            get { return (ICommand)this.GetValue(EditLessonCommandProperty); }
            set { this.SetValue(EditLessonCommandProperty, value); }
        }

        public static readonly DependencyProperty EditLessonCommandProperty = DependencyProperty.Register("EditLessonCommand",
            typeof(ICommand), typeof(LessonControl), new PropertyMetadata(null, (s, e) => ((LessonControl)s).EditLessonMenuFlyoutItem.Command = (ICommand)e.NewValue));
        
        public ICommand DeleteLessonCommand
        {
            get { return (ICommand)this.GetValue(DeleteLessonCommandProperty); }
            set { this.SetValue(DeleteLessonCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteLessonCommandProperty = DependencyProperty.Register("DeleteLessonCommand",
            typeof(ICommand), typeof(LessonControl), new PropertyMetadata(null, (s, e) => ((LessonControl)s).DeleteLessonMenuFlyoutItem.Command = (ICommand)e.NewValue));

        public ICommand NavigateToHomeworkOverviewPageCommand
        {
            get { return (ICommand)this.GetValue(NavigateToHomeworkOverviewPageCommandProperty); }
            set { this.SetValue(NavigateToHomeworkOverviewPageCommandProperty, value); }
        }

        public static readonly DependencyProperty NavigateToHomeworkOverviewPageCommandProperty = DependencyProperty.Register("NavigateToHomeworkOverviewPageCommand",
            typeof(ICommand), typeof(LessonControl), new PropertyMetadata(null, (s, e) => ((LessonControl)s).HomeworkMenuFlyoutItem.Command = (ICommand)e.NewValue));

        public LessonControl()
        {
            this.InitializeComponent();
        }
    }
}
