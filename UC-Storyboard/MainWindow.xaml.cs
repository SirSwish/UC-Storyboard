using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using UC_Storyboard.Converters;
using UC_Storyboard.Core;
using UC_Storyboard.MVVM.ViewModel;
using static UC_Storyboard.Converters.ImageConvert;

namespace UC_Storyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml - the chonkiness lives here!
    /// </summary>

    public partial class MainWindow : Window
    {

        private MainViewModel _mainViewModel;
        public event EventHandler<ObservableCollection<District>> DistrictListCalculated;
        public event EventHandler<ObservableCollection<MissionEntry>> MissionListCalculated;
        public event EventHandler<ObservableCollection<District>> DistrictListInitialized;
        public event EventHandler<ObservableCollection<MissionEntry>> MissionListInitialized;
        private readonly AdjustXConverter _adjustXConverter = new AdjustXConverter();
        private readonly AdjustYConverter _adjustYConverter = new AdjustYConverter();
        public ObservableCollection<MissionEntry> missionEntries = new ObservableCollection<MissionEntry>();
        public ObservableCollection<District> districts = new ObservableCollection<District>();

        public DistrictViewModel DistrictViewModel { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            DistrictListInitialized?.Invoke(this, districts);
            MissionListInitialized?.Invoke(this, missionEntries);
            _mainViewModel = new MainViewModel();
        }

        //Event handler for when you click the relevant district on the map. We want it to show the district information and all Missions underneath
        private void mapDistrictBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is District district)
            {
                StringBuilder message = new StringBuilder($"District ID: {district.DistrictID}\n{district.DistrictName}\n\nMissions:\n");
                MissionViewModel? missionViewModel = (Application.Current.MainWindow.DataContext as MainViewModel)?.MissionVM;

                if (missionViewModel != null)
                {
                    // Find missions related to the current district
                    var relatedMissions = missionViewModel.MissionCollection
                        .Where(mission => mission.District == district.DistrictID)
                        .ToList();

                    foreach (var mission in relatedMissions)
                    {
                        message.AppendLine($"- {mission.MissionName}");
                    }
                }
                MessageBox.Show(message.ToString(), "District Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
       
        //Executes when buttons in District ItemControl on map load - we use this to adjust position of the button on canvas depending on X/Y Pos next to District
        private void onMapDistrictBtnLoad(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is District district)
                {
                    // Send to converter - note this step isn't really necessary, but legacy
                    int xPos = (int)_adjustXConverter.Convert(district.XPos, null, button, null);
                    int yPos = (int)_adjustYConverter.Convert(district.YPos, null, button, null);

                    //Position button
                    TranslateTransform newTransform = new TranslateTransform(xPos, yPos);
                    button.RenderTransform = newTransform;
                }
            }
        }
        
        //Hacky stolen code that allows us in WPF to select a directory - we use this to select base Urban Chaos directory. We need this because we are tracking original UC files that we modify for restoration if necessary
        private void SelectDirectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = directoryLabel.Content?.ToString(), // Use current value for initial dir
                Title = "Select a Directory", // Instead of default "Save As"
                Filter = "Directory|*.this.directory", // Prevents displaying files
                FileName = "select" // Filename will then be "select.this.directory"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");

                // If the user has changed the filename, create the new directory
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Update the label with the selected folder path
                directoryLabel.Content = path;
            }
        }
        
        //Restores all files in a UC directory that have been modified by the UC Storyboard tool
        private void RestoreToDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            // Directory obviously can't be empty..
            if (string.IsNullOrEmpty((string)directoryLabel.Content) || (string)directoryLabel.Content == "No Directory Selected")
            {
                MessageBox.Show("Please select a valid UC directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string ucDirectory = (string)directoryLabel.Content;
            string talk2Directory = Path.Combine(ucDirectory, "talk2"); //Stores Mission Briefings
            string dataDirectory = Path.Combine(ucDirectory, "data"); //Stores .sty and .tga map files

            RestoreFileExtensions(talk2Directory);
            RestoreFileExtensions(dataDirectory);

            MessageBox.Show("Restore to defaults completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Executes when select map button is clicked
        private void SelectMapImageBtn_Click(object sender, RoutedEventArgs e)
        {
            //We can only use bitmaps...
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Map Image",
                Filter = "BMP Files (*.bmp)|*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                // Validate image dimensions - must be 640x480 that's what the game uses
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    var bitmap = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    int width = bitmap.PixelWidth;
                    int height = bitmap.PixelHeight;

                    if (width == 640 && height == 480)
                    {
                        // Update the Image source overlaying the canvas
                        mapImage.Source = new BitmapImage(new Uri(imagePath));
                        mapImageLabel.Content = imagePath;
                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions 640x480.", "Invalid Image Dimensions", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        //Tool appends .orig to original files that were modified during usage of the tool. Simply check for existence of these files and overwrite any imposters!
        private void RestoreFileExtensions(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    // Get all .orig files in the directory
                    var origFiles = Directory.GetFiles(directory, "*.orig");

                    foreach (var origFile in origFiles)
                    {
                        try
                        {
                            // Build the target file path without the .orig extension
                            string targetFile = Path.ChangeExtension(origFile, null);

                            if (File.Exists(targetFile))
                            {
                                File.Delete(targetFile);
                            }
                            // Rename the .orig file by removing the extension, effectively removing .orig
                            File.Move(origFile, targetFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error restoring file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        //Used to save the new .sty file, any custom audio briefings and any custom map that was used
        private void ExportFileBtn_Click(object sender, RoutedEventArgs e) 
        {

            // Need valid directory!
            if ((string)directoryLabel.Content == "No Directory Selected")
            {
                MessageBox.Show("Please select the UC Directory before exporting.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //If we have a custom map, lets export it now!
            if ((string)mapImageLabel.Content != "No File Selected") 
            {
                ExportCustomMap((string)mapImageLabel.Content, (string)directoryLabel.Content);
            }

            if (districts == null || districts.Count == 0 || missionEntries == null || missionEntries.Count == 0)
            {
                MessageBox.Show("Ensure there is at least one entry in both District and Mission collections before exporting.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Export custom audio briefings. If none are used, the originals are simply .orig'd and the game won't play them over custom missions
            ExportCustomBriefingAudio(missionEntries, (string)directoryLabel.Content);

            var saveFileDialog = new SaveFileDialog
            {
                FileName = "urban.sty",
                DefaultExt = ".sty",
                Filter = "Urban Chaos Story Script (.sty)|*.sty|All Files|*.*",
                InitialDirectory = Path.Combine((string)directoryLabel.Content, "data")
            };

            // Create backup .sty
            string originalFilePath = Path.Combine((string)directoryLabel.Content, "data", "urban.sty");
            string backupFilePath = Path.Combine((string)directoryLabel.Content, "data", "urban.sty.orig");

            try
            {
                // Copy the file if it exists
                if (File.Exists(originalFilePath) && !File.Exists(backupFilePath))
                {
                    File.Copy(originalFilePath, backupFilePath, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying urban.sty to urban.sty.orig: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the new header
                    writer.WriteLine("//");
                    writer.WriteLine("// Urban Chaos Story Script");
                    writer.WriteLine("// Version 4");
                    writer.WriteLine("// Exported using the UC Storyboard Tool on " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
                    writer.WriteLine("//");
                    writer.WriteLine("// Object ID : Group ID : Parent : Parent-is-group : Type : Flags : District : Filename : Title : Briefing");
                    writer.WriteLine("//");

                    // Write the mission entries
                    foreach (MissionEntry missionEntry in missionEntries)
                    {
                        // Replace "\r\n" with "\r" in the briefing
                        string briefing = missionEntry.MissionBriefing.Replace("\r\n", "\r");
                        string entry = $"{missionEntry.ObjectID} : {missionEntry.GroupID} : {missionEntry.Parent} : {missionEntry.ParentIsGroup} : {missionEntry.Type} : {missionEntry.Flags} : {missionEntry.District} : {missionEntry.MissionFile} : {missionEntry.MissionName} : {briefing}";
                        writer.WriteLine(entry);
                    }

                    // Write the districts
                    writer.WriteLine("[districts]");
                    foreach (District district in districts)
                    {
                        string entry = $"{district.DistrictName}={district.XPos},{district.YPos}";
                        writer.WriteLine(entry);
                    }
                }

                MessageBox.Show("Export completed successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
       
        //Function for exporting selected custom map
        private void ExportCustomMap(string imagePath, string directory)
        {
            // There are four different map files that change depending on mission number, so we need to overwrite them all
            List<string> fileNamesToRename = new List<string>
            {
                "map blood darci.tga",
                "map leaves darci.tga",
                "map rain darci.tga",
                "map snow darci.tga"
            };

            bool origFilesExist = fileNamesToRename.All(fileName => System.IO.File.Exists(System.IO.Path.Combine(directory, "data", fileName + ".orig")));

            // .orig extension files are already present, no need to backup
            if (origFilesExist)
            {
                //do nothing
            }
            else
            {
                foreach (string fileName in fileNamesToRename)
                {
                    string filePath = System.IO.Path.Combine(directory, "data", fileName);
                    string originalFilePath = filePath + ".orig";

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Move(filePath, originalFilePath);
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {fileName}", "File Not Found. Have you selected the correct Base Urban Chaos directory?", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Convert the image to TGA format - saved by TGASharpLib!
                ConvertToTGA(imagePath, fileNamesToRename, directory);

                MessageBox.Show("Custom map exported successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        //Function that calls TGASharpLib for image conversion
        private void ConvertToTGA(string imagePath, List<string> fileNamesToRename, string directory)
        {
            Bitmap inputBitmap = new Bitmap(imagePath); //TGASharpLib doesn't support JPEGs, so BMP it must be
            inputBitmap.RotateFlip(RotateFlipType.Rotate180FlipX); //For some reason TGA converters seem to write files upside down....simply flip BMP before conversion
            TGA tgaInstance = new TGA(inputBitmap, UseRLE: false, NewFormat: false, ColorMap2BytesEntry: false);
            foreach (string fileName in fileNamesToRename)
            {
                string filePath = System.IO.Path.Combine(directory, "data", fileName);
                tgaInstance.Save(filePath);
            }
        }

        //Function that exports custom audio briefing to correct filename
        private void ExportCustomBriefingAudio(ObservableCollection<MissionEntry> missionCollection, string directory)
        {
            // Check if the talk2 directory exists, if not, we're in the wrong place
            string talk2Directory = Path.Combine(directory, "talk2");
            if (!Directory.Exists(talk2Directory))
            {
                MessageBox.Show("The selected directory is incorrect. Please choose the correct UC directory.", "Directory Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            //Otherwise, let's make the lookup
            AudioFileLookup audioFileLookup = new AudioFileLookup();

            // Make backups of each file
            foreach (KeyValuePair<int, string> entry in audioFileLookup.idToFileMap)
            {
                string originalFilePath = Path.Combine(talk2Directory, $"{entry.Value}.orig");
                string currentFilePath = Path.Combine(talk2Directory, entry.Value);

                if (File.Exists(currentFilePath) && !File.Exists(originalFilePath))
                {
                    File.Move(currentFilePath, originalFilePath);
                }
            }

            foreach (MissionEntry missionEntry in missionEntries)
            {
                if (!string.IsNullOrEmpty(missionEntry.BriefingAudioFilePath))
                {
                    // Get the mission ID and corresponding audio filename
                    int missionId = missionEntry.ObjectID;
                    string audioFilename = audioFileLookup.GetFileNameById(missionId);

                    // Copy the file to the talk2 directory with the correct filename based on lookup
                    string sourceFilePath = missionEntry.BriefingAudioFilePath;
                    string destinationFilePath = Path.Combine(talk2Directory, audioFilename);

                    File.Copy(sourceFilePath, destinationFilePath, true); // Set overwrite to true to replace existing files
                }
            }
        }
  
        //Executes when we click the Load file button
        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            //Open STY files - currently only supports v4 - but easy enough to implement v3 (available from UC Prototype)
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Storyboard Files (*.sty)|*.sty|All Files (*.*)|*.*",
                Title = "Select a Storyboard File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Make sure it is V4 at this stage
                bool isValid = ValidFileCheck(filePath);

                if (isValid)
                {
                    fileLabel.Content = filePath;

                    missionEntries = GetMissions(filePath);
                    districts = GetDistricts(filePath);

                    DistrictListCalculated?.Invoke(this, districts);
                    MissionListCalculated?.Invoke(this, missionEntries);

                }
                else
                {
                    // Show an error message if the file is not valid
                    MessageBox.Show("The selected file is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //Check if file is V4 and is even a sty file
        private bool ValidFileCheck(string filePath)
        {
            try
            {
                // Read the first three lines of the file - this is the header info
                string[] fileLines = File.ReadAllLines(filePath);

                // Check if the required lines are present
                if (fileLines.Length >= 3 &&
                    fileLines[1].Contains("// Urban Chaos Story Script") &&
                    fileLines[2].Contains("// Version 4"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        //Get the Districts from the file
        static ObservableCollection<District> GetDistricts(String filePath)
        {
            ObservableCollection<District> districts = new ObservableCollection<District>();
            bool inDistrictsSection = false;
            int districtIdCounter = 0;
            string line;

            using (StreamReader reader = new StreamReader(filePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    // Check if it's the start of the 'districts' section
                    if (line.Trim() == "[districts]")
                    {
                        inDistrictsSection = true;
                        continue;
                    }

                    if (inDistrictsSection)
                    {
                        // Parse district lines
                        string[] districtParts = line.Split('=');

                        if (districtParts.Length == 2)
                        {
                            string districtName = districtParts[0].Trim();
                            string[] coordinates = districtParts[1].Split(',');

                            if (coordinates.Length == 2 && int.TryParse(coordinates[0], out int latitude) && int.TryParse(coordinates[1], out int longitude))
                            {
                                District district = new District
                                {
                                    DistrictID = districtIdCounter,
                                    DistrictName = districtName,
                                    XPos = latitude,
                                    YPos = longitude
                                };
                                districts.Add(district);
                                districtIdCounter++;
                            }
                        }
                    }
                }
            }

            return districts;
        }
        
        //Get the missions from the file
        static ObservableCollection<MissionEntry> GetMissions(String filePath)
        {
            ObservableCollection<MissionEntry> missionEntries = new ObservableCollection<MissionEntry>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Ignore the first 7 lines (header containing comments)
                for (int i = 0; i < 7; i++)
                {
                    reader.ReadLine();
                }

                string allLines = reader.ReadToEnd();

                // Split the lines into separate entries using the hex byte sequence "0D 0A" i.e. new-lines
                string delimiter = "\u000D\u000A";
                string[] entries = allLines.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string entry in entries)
                {
                    // Parse each entry and create MissionEntry objects
                    string[] entryParts = entry.Split(':');
                    byte hexByte = 0x2E;
                    if (entryParts.Length >= 9)
                    {
                        MissionEntry missionEntry = new MissionEntry
                        {
                            ObjectID = int.Parse(entryParts[0].Trim()),
                            GroupID = int.Parse(entryParts[1].Trim()),
                            Parent = int.Parse(entryParts[2].Trim()),
                            ParentIsGroup = int.Parse(entryParts[3].Trim()),
                            Type = int.Parse(entryParts[4].Trim()),
                            Flags = int.Parse(entryParts[5].Trim()),
                            District = int.Parse(entryParts[6].Trim()),
                            MissionFile = entryParts[7].Trim(),
                            MissionName = entryParts[8].Trim()
                        };

                        // Extract the Mission Briefing
                        StringBuilder missionBriefing = new StringBuilder();
                        for (int i = 9; i < entryParts.Length; i++)
                        {
                            missionBriefing.Append(entryParts[i]);
                            if (i < entryParts.Length - 1)
                            {
                                missionBriefing.Append(" : ");
                            }
                        }
                        missionBriefing.Append((char)hexByte);
                        missionEntry.MissionBriefing = missionBriefing.ToString();

                        missionEntries.Add(missionEntry);
                    }
                }
            }
            return missionEntries;
        }
        
    }
}


