using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stundenplan
{
    public class Pivot : Windows.UI.Xaml.Controls.Pivot
    {
        //Designer fix
        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

    }
    public class PivotItem : Windows.UI.Xaml.Controls.PivotItem
    {
        //Designer fix
        public new string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
