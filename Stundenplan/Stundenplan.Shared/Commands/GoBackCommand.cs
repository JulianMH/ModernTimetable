using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Stundenplan.Commands
{
    public class GoBackCommand : DependencyObject, ICommand
    {
        public Frame NavigationFrame
        {
            get { return (Frame)this.GetValue(NavigationFrameProperty); }
            set { this.SetValue(NavigationFrameProperty, value); }
        }

        public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register("NavigationFrame",
            typeof(Frame), typeof(GoBackCommand), new PropertyMetadata(null));

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public virtual void Execute(object parameter)
        {
            this.NavigationFrame.GoBack();
        }
    }
}
