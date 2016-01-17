using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Stundenplan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        public TestPage()
        {
            this.InitializeComponent();


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var control = new LandscapeWeekView() { Width = 300, Height = 300 };

            TestGrid.Children.Add(control);

            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(control);


            TestGrid.Children.Remove(control);

            TestGrid.Background = new ImageBrush() { ImageSource = bitmap };
        }
    }
}
