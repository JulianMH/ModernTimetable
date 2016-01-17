using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.StartScreen;
using Windows.Foundation;
using Windows.Storage;
using Stundenplan.Data;
using Stundenplan.Localization;
using Windows.UI.Xaml;
using System.Linq;

namespace Stundenplan.LiveTile
{
    public sealed class AppTileUpdater : XamlRenderingBackgroundTask
    {
        protected override void OnRun(Windows.ApplicationModel.Background.IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            System.Diagnostics.Debug.WriteLine("OnRun called!");
            UpdateTileAsync(_deferral);
        }

        public static IAsyncOperation<Grid> ReadTileControl()
        {
            return ReadTileControlPrivate().AsAsyncOperation();
        }

        private static async Task<Grid> ReadTileControlPrivate()
        {
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            var file = await folder.GetFileAsync("NormalTile.xml");
            return XamlReader.Load(await Windows.Storage.FileIO.ReadTextAsync(file)) as Grid;
        }
        public static IAsyncAction UpdateTileImage(Grid tileControl, SecondaryTile secondaryTile)
        {
            return UpdateTileImagePrivate(tileControl, secondaryTile).AsAsyncAction();
        }

        private class LessonData
        {
            public string LessonTime { get; set; }
            public string Subject { get; set; }
            public string Room { get; set; }
        }

        private static async Task UpdateTileImagePrivate(Grid tileControl, SecondaryTile secondaryTile)
        {
            Action<string, string> assignText = (name, text) => ((TextBlock)tileControl.FindName(name)).Text = text;
            Action<string> hide = (name) => ((TextBlock)tileControl.FindName(name)).Visibility = Visibility.Collapsed;

            assignText("Timestamp", "Last update " + DateTime.Now.ToString("hh:mm:ss"));

            Timetable timetable = null;

            try
            {
                timetable = Timetable.ParseXml(await FileIO.ReadTextAsync(await
                    (await ApplicationData.Current.LocalFolder.GetFolderAsync("Timetables")).GetFileAsync(secondaryTile.Arguments)),
                    secondaryTile.Arguments, "");

                var day = timetable.GetListedLessons(DateTime.Now, DateTime.Now.AddDays(7), ConvertHelpers.IsOddWeek(DateTime.Now))
                    .First(p => p.Value.Any(q => q.Value.Subject != Subject.None));

                var lessonList = day.Value.Where(q => q.Value.Subject != Subject.None).Take(4).Select(p => new LessonData()
                {
                    LessonTime = string.Format(Strings.LessonTimeMultiLineFormat, p.Key.Start, p.Key.End),
                    Subject = p.Value.Subject.Name,
                    Room = p.Value.Room
                }).ToList();

                if (day.Key.Date != DateTime.Today)
                {
                    lessonList.Insert(0, new LessonData() { LessonTime = day.Key.Date.ToString("dddd") });
                    while (lessonList.Count > 4)
                        lessonList.RemoveAt(4);
                }

                ((ItemsControl)tileControl.FindName("LessonsControl")).ItemsSource = lessonList;

                var imageFileName = secondaryTile.Arguments + ".png";
                var wideImageFileName = secondaryTile.Arguments + ".wide.png";

                await RenderTile(tileControl, wideImageFileName, 310, 150);

                tileControl.Width = tileControl.Height;

                await RenderTile(tileControl, imageFileName, 150, 150);

                secondaryTile.VisualElements.Square150x150Logo = new Uri("ms-appdata:///local/" + imageFileName);
                secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appdata:///local/" + wideImageFileName);

                secondaryTile.DisplayName = timetable.Name;
            }
            catch
            {
                secondaryTile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Logo.scale-240.png");
                secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/WideLogo.scale-240.png");

                return;
            }

        }

        private static async Task RenderTile(Grid tileControl, string imageFileName, int scaledWidth, int scaledHeight)
        {
            var renderTarget = new RenderTargetBitmap();
            await renderTarget.RenderAsync(tileControl, scaledWidth, scaledHeight);
            IBuffer pixels = await renderTarget.GetPixelsAsync();

            byte[] data = new byte[pixels.Length];
            DataReader.FromBuffer(pixels).ReadBytes(data);

            var outputFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(imageFileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            using (var outputStream = await outputFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outputStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)renderTarget.PixelWidth, (uint)renderTarget.PixelHeight, 96, 96, data);
                await encoder.FlushAsync();
            }
        }

        private async void UpdateTileAsync(BackgroundTaskDeferral deferral)
        {
            var tiles = await SecondaryTile.FindAllForPackageAsync();

            if (tiles.Count > 0)
            {
                var tileControl = await ReadTileControl();
                foreach (var tile in tiles)
                {
                    await UpdateTileImage(tileControl, tile);
                    await tile.UpdateAsync();
                }
            }

            deferral.Complete();
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            System.Diagnostics.Debug.WriteLine("Run called!");
            OnRun(taskInstance);
        }
    }
}