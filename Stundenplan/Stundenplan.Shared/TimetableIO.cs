using Stundenplan.Data;
using Stundenplan.Localization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Core;

namespace Stundenplan
{
    static class TimetableIO
    {
        private static readonly string backupExtension = ".backup";
        private static readonly string currentTimetableFileName = "CurrentTimetable.txt";
        private static readonly string timetablesDirectoryName = "Timetables";
        private static readonly string timetableFileNameFormat = "Timetable.xml";

        private static async Task<StorageFolder> GetTimetableDirectory()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(timetablesDirectoryName, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<Timetable> LoadCurrentTimetable()
        {
            try
            {
                string timetableFileName = null;
                using (var stream = await (await GetTimetableDirectory()).OpenStreamForReadAsync(currentTimetableFileName))
                using (var reader = new StreamReader(stream))
                {
                    timetableFileName = reader.ReadToEnd();
                }

                return await LoadTimetable(timetableFileName);
            }
            catch
            {
            }

            //Legacy Support
            try
            {
                var timetableFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Timetable.xml");

                var newFileName = await ReserveNewTimetableFilename();
                var newFile = await (await GetTimetableDirectory()).GetFileAsync(newFileName);
                await timetableFile.MoveAndReplaceAsync(newFile);

                return await LoadTimetable(newFileName);
            }
            catch
            {

            }

            return null;
        }

        public static async Task<Timetable> LoadTimetable(string fileName)
        {
            Exception fileException = null;
            try
            {
                return await LoadTimetableInner(fileName);
            }
            catch (Exception e)
            {
                fileException = e;
            }

            //Naja, noch kann man ja versuchen ein Backup zu laden.
            try
            {
                var timetable = await LoadTimetableInner(fileName);

                //Hmmpf... grade noch mal die Kurve gekriegt
                ComposeErrorReport(Strings.LoadSaveBackupLoaded, fileException, fileException.InnerException);

                return timetable;
            }
            catch (Exception fileBackupException)
            {
                //Worst Case Situation
                ComposeErrorReport(Strings.LoadSaveBackupFailed, fileBackupException, fileBackupException.InnerException);
                return null;
            }
        }

        private static async Task<Timetable> LoadTimetableInner(string fileName)
        {
            var timetableDirectory = await GetTimetableDirectory();

            string xml;
            try
            {
                using (var stream = await timetableDirectory.OpenStreamForReadAsync(fileName))
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();

                throw new Exception(Strings.TimetableLoadReadException, e);
            }

            //Datei lesen
            try
            {
                return Timetable.ParseXml(xml, fileName, Strings.TimetableNewName);
            }
            catch (Exception e)
            {
                throw new Exception(Strings.TimetableLoadParseException, e);
            }

        }

        public static async Task SaveTimetable(Timetable timetable)
        {
            try
            {
                var timetableDirectory = await GetTimetableDirectory();
                var timetableFile = await timetableDirectory.CreateFileAsync(timetable.FileName, CreationCollisionOption.OpenIfExists);

                try
                {
                    var backupFile = await timetableDirectory.CreateFileAsync(timetable.FileName + backupExtension, CreationCollisionOption.OpenIfExists);
                    await timetableFile.CopyAndReplaceAsync(backupFile);
                    timetableFile = await timetableDirectory.CreateFileAsync(timetable.FileName, CreationCollisionOption.ReplaceExisting);
                }
                catch (Exception e)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();

                    throw new Exception(Strings.TimetableSaveToFileBackupException, e);
                }

                //XML-Generieren
                string xml;
                try
                {
                    var appVersion = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor;
                    xml = timetable.WriteXml(appVersion);
                }
                catch (Exception e)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();

                    throw new Exception(Strings.TimetableSaveFileInnerXmlException, e);
                }

                //Alles in die Datei schreiben
                try
                {
                    using (var stream = await timetableFile.OpenStreamForWriteAsync())
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.Write(xml);
                    }
                }
                catch (Exception e)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();

                    throw new Exception(Strings.TimetableSaveFileInnerSaveException, e);
                }

                using (var stream = await (await (await GetTimetableDirectory()).CreateFileAsync(currentTimetableFileName, CreationCollisionOption.ReplaceExisting))
                    .OpenStreamForWriteAsync())
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(timetable.FileName);
                }
            }
            catch (Exception error)
            {
                //Worst Case Situation
                ComposeErrorReport(String.Format(Strings.LoadSaveSaveErrorFormat, error.Message), error, error.InnerException);
            }
        }

        public static async Task<string> ReserveNewTimetableFilename()
        {
            var timetableDirectory = await GetTimetableDirectory();

            var file = await timetableDirectory.CreateFileAsync(timetableFileNameFormat, CreationCollisionOption.GenerateUniqueName);

            return file.Name;
        }

        public static CoreDispatcher ErrorMessageDispatcher { get; set; }

        private static async void ComposeErrorReport(string messageBoxText, Exception error1, Exception error2)
        {
            if (ErrorMessageDispatcher == null)
                return;
            await ErrorMessageDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var messageDialog = new MessageDialog(String.Format(Strings.LoadSaveWarningFormat, messageBoxText), Strings.MessageBoxCriticalError);

                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, async e =>
                {
                    var appVersion = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor;
                    
                    var mail = new EmailMessage();

                    mail.Subject = Strings.LoadSaveReportSubject;
                    string exceptionString = string.Format(Strings.LoadSaveReportPrimaryErrorFormat, error1.GetType().Name, error1.Message, error1.StackTrace);

                    if (error2 != null)
                    {
                        exceptionString += string.Format(Strings.LoadSaveReportSecondaryErrorFormat, error2.GetType().Name, error2.Message, error2.StackTrace);
                    }

                    mail.Body = string.Format(Strings.LoadSaveReport, Strings.AppName, appVersion, exceptionString);

                    mail.To.Add(new EmailRecipient()
                    {
                        Address = "support@philbi.de"
                    });

                    await EmailManager.ShowComposeNewEmailAsync(mail);
                }));
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));

                await messageDialog.ShowAsync();
            });
        }

        public static async Task<List<TimetableDescription>> LoadAllTimetableDescriptions()
        {
            var timetableDirectory = await GetTimetableDirectory();
            var files = await timetableDirectory.GetFilesAsync();

            List<TimetableDescription> timetables = new List<TimetableDescription>();

            foreach (var file in files)
            {
                try
                {
                    if (file.FileType == ".xml") //Backups unbedingt rausfiltern
                    {
                        using (var stream = await file.OpenStreamForReadAsync())
                        using (var reader = new StreamReader(stream))
                        {
                            string data = reader.ReadToEnd();
                            timetables.Add(new TimetableDescription(data, file.Name, Strings.TimetableDescriptionNoName, Strings.TimetableDescriptionInvalidTimetable));
                        }
                    }
                }
                catch
                {

                }
            }

            return timetables;
        }

        public static async Task DeleteTimetable(string fileName)
        {
            var timetableDirectory = await GetTimetableDirectory();

            try
            {
                await (await timetableDirectory.GetFileAsync(fileName)).DeleteAsync();
            }
            catch { }

            try
            {
                await (await timetableDirectory.GetFileAsync(fileName + backupExtension)).DeleteAsync();
            }
            catch { }
        }


        public static async Task CopyTimetable(string fileName)
        {
            var timetableDirectory = await GetTimetableDirectory();
            await (await timetableDirectory.GetFileAsync(fileName)).CopyAndReplaceAsync(await timetableDirectory.GetFileAsync(await ReserveNewTimetableFilename()));
        }

        public static async Task ImportTimetable(StorageFile file)
        {
            var fileName = await ReserveNewTimetableFilename();
            var destinationFile = await (await GetTimetableDirectory()).GetFileAsync(fileName);
            try
            {
                await file.CopyAndReplaceAsync(destinationFile);
                Timetable.ParseXml(await FileIO.ReadTextAsync(destinationFile), fileName, Strings.TimetableNewName);
                return;
            }
            catch  { }
            await destinationFile.DeleteAsync();
            await new MessageDialog(Strings.TimetableIOImportError, Strings.MessageBoxWarningCaption).ShowAsync();
        }


        internal static async Task ExportTimetable(string exportFileName, StorageFile destinationFile)
        {
            try
            {
                var sourceFile = await (await GetTimetableDirectory()).GetFileAsync(exportFileName);
                await sourceFile.CopyAndReplaceAsync(destinationFile);
                return;
            }
            catch  { }
            await destinationFile.DeleteAsync();
            await new MessageDialog(Strings.TimetableIOExportError, Strings.MessageBoxWarningCaption).ShowAsync();
        }
    }
}
