using Stundenplan.Common;
using Stundenplan.Data;
using Stundenplan.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.StartScreen;

namespace Stundenplan.ViewModels
{
    public class SelectTimetablePageViewModel : NotifyPropertyChangedObject
    {
        private Action goBack;
        private Action goToSettings;

        public ICommand DeleteCommand { get; protected set; }
        public ICommand SelectCommand { get; protected set; }
        public ICommand DublicateCommand { get; protected set; }
        public ICommand SettingsCommand { get; protected set; }
        public ICommand AddCommand { get; protected set; }

        protected ObservableCollection<TimetableDescription> timetables;
        public ReadOnlyObservableCollection<TimetableDescription> Timetables { get; protected set; }

        private TimetableDescription deleteWhenChanged;

        public SelectTimetablePageViewModel(Action goBack, Action goToSettings)
        {
            this.goBack = goBack;
            this.goToSettings = goToSettings;
            LoadData();
        }

        public async virtual void LoadData()
        {
            await TimetableIO.SaveTimetable(App.Timetable);
            this.timetables = new ObservableCollection<TimetableDescription>((await TimetableIO.LoadAllTimetableDescriptions()).OrderByDescending(p => p.FileName == App.Timetable.FileName).ThenBy(p => p.Name));
            this.Timetables = new ReadOnlyObservableCollection<TimetableDescription>(timetables);
            NotifyPropertyChanged("Timetables");

            this.DeleteCommand = new RelayCommandWithParameter(async p =>
            {
                var timetable = p as TimetableDescription;

                var messageDialog = new MessageDialog(string.Format(Strings.PageSelectTimetableDeleteWarning, timetable.Name), Strings.MessageBoxRemoveCaption);
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, async x =>
                {
                    timetables.Remove(timetable);

                    if (App.Timetable.FileName == timetable.FileName)
                        deleteWhenChanged = timetable;
                    else
                        await TimetableIO.DeleteTimetable(timetable.FileName);
                }));

                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));
                await messageDialog.ShowAsync();

            });

            this.AddCommand = new RelayCommand(async () => {
                var timetable = Timetable.CreateNew(Strings.TimetableNewName, await TimetableIO.ReserveNewTimetableFilename());
                await TimetableIO.SaveTimetable(timetable);
                timetables.Add(new TimetableDescription(timetable.FileName, timetable.Name, 0, 0));
            });

            this.SettingsCommand = new RelayCommandWithParameter(async p =>
            {
                var timetable = p as TimetableDescription;

                await TimetableIO.CopyTimetable(timetable.FileName);

                timetables.Add(new TimetableDescription(timetable.FileName, timetable.Name, timetable.HomeworksCount, timetable.DatesCount));
            });

            this.SelectCommand = new RelayCommandWithParameter(p => SelectTimetable((TimetableDescription)p, goBack));
            this.SettingsCommand = new RelayCommandWithParameter(p => SelectTimetable((TimetableDescription)p, goToSettings));
        }

        private async void SelectTimetable(TimetableDescription timetableDescription, Action doAfter)
        {
            await TimetableIO.SaveTimetable(App.Timetable);

            if (deleteWhenChanged != null)
            {
                await TimetableIO.DeleteTimetable(deleteWhenChanged.FileName);
                deleteWhenChanged = null;
            }

            var timetable = await TimetableIO.LoadTimetable(timetableDescription.FileName);
            if (timetable != null)
            {
                App.Timetable = timetable;
                doAfter();
            }
        }
    }
}
