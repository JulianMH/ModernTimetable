using Stundenplan.Localization;
using Stundenplan.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Store;
using System.Threading.Tasks;
using Stundenplan.ViewModels;
using Stundenplan.Common;
using Windows.Storage;
using Windows.UI.Popups;

namespace Stundenplan
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public static Timetable Timetable { get; set; }
        
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            WindowsRuntimeResourceManager.InjectIntoResxGeneratedApplicationResourcesClass(typeof(Strings));

            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            Subject.None.Name = Strings.SubjectFreeTime;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null || (e.Arguments != null && e.Arguments != ""))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.CacheSize = 3;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            TimetableIO.ErrorMessageDispatcher = rootFrame.Dispatcher;

            // Load timetable synchronous here to avoid having to rewrite the entire Codebase to understand async.
            if (e.Arguments != null && e.Arguments != "" && (Timetable == null ||  Timetable.FileName != e.Arguments))
            {
                Timetable = Task.Run(async () =>
                {
                    if (Timetable != null)
                        await TimetableIO.SaveTimetable(Timetable);
                    return await TimetableIO.LoadTimetable(e.Arguments); 
                }).Result;
            }

            if(Timetable == null)
            {
                Timetable = Task.Run(async () =>
                {
                    var timetable = await TimetableIO.LoadCurrentTimetable();

                    if (timetable == null)
                        timetable = Timetable.CreateNew(Strings.TimetableNewName, await TimetableIO.ReserveNewTimetableFilename());

                    return timetable;
                }).Result;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
                rootFrame.Navigating += this.RootFrame_Navigating;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            ShowOpenSourceMessage();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async void ShowOpenSourceMessage()
        {
            if (DateTime.Now < new DateTime(2016, 3, 1))
            {
                try
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync("HideOpenSourceMessage.txt");
                }
                catch (FileNotFoundException)
                {
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("HideOpenSourceMessage.txt");

                    var githubLink = "https://github.com/JulianMH/ModernTimetable";
                    MessageDialog messageDialog = new MessageDialog(String.Format(Localization.Strings.OpenSourceMessage, Strings.AppName, githubLink));
                    messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK));
                    messageDialog.Commands.Add(new UICommand(Strings.OpenSourceLookAt, async action => await Windows.System.Launcher.LaunchUriAsync(new Uri(githubLink))));
                    await messageDialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await TimetableIO.SaveTimetable(Timetable);

            deferral.Complete();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            switch (args.Kind)
            {
                case ActivationKind.PickFileContinuation:
                    if (args is FileOpenPickerContinuationEventArgs)
                    {
                        var files = ((FileOpenPickerContinuationEventArgs)args).Files;

                        foreach (var file in files)
                            await TimetableIO.ImportTimetable(file);

                        var page = ((Frame)Window.Current.Content).Content as SelectTimetablePage;
                        if (page != null && page.DataContext != null)
                            ((SelectTimetablePageViewModel)page.DataContext).LoadData();
                    }
                    break;
                case ActivationKind.PickSaveFileContinuation:
                    if (args is FileSavePickerContinuationEventArgs)
                    {
                        var file = ((FileSavePickerContinuationEventArgs)args).File;
                        var exportFileName = (string)((FileSavePickerContinuationEventArgs)args).ContinuationData["FileName"];

                        await TimetableIO.ExportTimetable(exportFileName, file);
                    }
                    break;
            }
            base.OnActivated(args);
        }
        
        internal static string NavigationHistory { get; set; }

        internal void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //Debugdaten, müssen vom User nicht verstanden werden.
            switch (e.NavigationMode)
            {
                case NavigationMode.Back:
                    NavigationHistory += "Navigiere zurück:\t" + e.SourcePageType.Name + "\n";
                    break;
                case NavigationMode.New:
                    NavigationHistory += "Navigiere weiter:\t" + e.SourcePageType.Name + "\n";
                    break;
                default:
                    NavigationHistory += "Navigiere:\t" + e.SourcePageType.Name + "\n";
                    break;
            }
            if (e.Parameter != null)
                NavigationHistory += "\tParameter:" + e.Parameter.ToString();
        }
    }
}