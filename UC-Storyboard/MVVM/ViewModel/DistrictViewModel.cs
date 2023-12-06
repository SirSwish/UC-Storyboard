using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UC_Storyboard.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UC_Storyboard.Core;

namespace UC_Storyboard.MVVM.ViewModel
{
    public class DistrictViewModel : ObservableObject
    {

        private readonly AdjustXConverter _adjustXConverter = new AdjustXConverter();
        private readonly AdjustYConverter _adjustYConverter = new AdjustYConverter();
        private ObservableCollection<District> _districtCollection;
        public ObservableCollection<District> DistrictCollection
        {
            get => _districtCollection;
            set
            {
                if (_districtCollection != value)
                {
                    _districtCollection = value;
                    OnPropertyChanged(nameof(DistrictCollection));

                    // Subscribe to PropertyChanged event for each existing district
                    foreach (var district in _districtCollection)
                    {
                        district.PropertyChanged += District_PropertyChanged;
                    }
                }
            }
        }

        public DistrictViewModel()
        {
            // Used for populating the ObservableCollection - this happens from MainWindow (when file is loaded) hence we subscribe it here
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.DistrictListCalculated += OnDistrictListCalculated;
                mainWindow.DistrictListInitialized += OnDistrictListInitialized;

            }
        }

        private void District_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            // Check which property changed and perform actions accordingly
            if (e.PropertyName == nameof(District.XPos) || e.PropertyName == nameof(District.YPos))
            {
                if (sender is District updatedDistrict)
                {
                    int districtID = updatedDistrict.DistrictID;

                    // Access the ItemsControl in MainWindow.xaml
                    ItemsControl itemsControl = mainWindow.FindName("districtButtonItemControl") as ItemsControl;

                    // Find the corresponding button based on DistrictID
                    Button targetButton = FindButtonByDistrictID(itemsControl, districtID);

                    if (targetButton != null)
                    {
                        // Send to converter - note this step isn't really necessary, but legacy
                        int xPos = (int)_adjustXConverter.Convert(updatedDistrict.XPos, null, targetButton, null);
                        int yPos = (int)_adjustYConverter.Convert(updatedDistrict.YPos, null, targetButton, null);

                        //Position button
                        TranslateTransform newTransform = new TranslateTransform(xPos, yPos);
                        targetButton.RenderTransform = newTransform;
                    }
                }
            }
        }

        private Button FindButtonByDistrictID(ItemsControl itemsControl, int districtID)
        {
            foreach (var item in itemsControl.Items)
            {
                if (item is District district && district.DistrictID == districtID)
                {

                    ContentPresenter contentPresenter = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;

                    // Get the first child of the ContentPresenter, because it will be the button
                    Button targetButton = VisualTreeHelper.GetChild(contentPresenter, 0) as Button;

                    return targetButton;
                }
            }

            return null; // Button not found
        }

        public void AddNewDistrict()
        {
            if (DistrictCollection == null)
            {
                // Start new collection if this is the first District (i.e. we're starting blank)
                //DistrictCollection = new ObservableCollection<District>();
                //We shouldnt end up here anymore
            }

            // Generate a new District with predefined values
            District newDistrict = new District
            {
                DistrictID = GetNextDistrictID(),  // You need a method to get the next available ID
                DistrictName = "New District",
                XPos = 100,
                YPos = 100
            };

            DistrictCollection.Add(newDistrict);
        }

        private int GetNextDistrictID()
        {
            // If DistrictCollection is not initialized, return 1 as the first ID
            if (DistrictCollection == null || DistrictCollection.Count == 0)
            {
                return 0;
            }

            // Assuming DistrictCollection is not null and not empty, this will return the last ID in the collection
            int maxId = DistrictCollection.Max(d => d.DistrictID);
            return maxId + 1;
        }
        private void OnDistrictListCalculated(object sender, ObservableCollection<District> districts)
        {
            DistrictCollection = districts;
        }
        private void OnDistrictListInitialized(object sender, ObservableCollection<District> districts)
        {
            DistrictCollection = districts;
        }
        public void DeleteDistrict(District district)
        {
            DistrictCollection.Remove(district);
        }
        public void DeleteDistricts(IEnumerable<District> districts)
        {
            foreach (var district in districts.ToList()) // ToList() creates a copy to avoid modification during iteration
            {
                DistrictCollection.Remove(district);
            }
        }
        public void UpdateDistrict(District updatedDistrict)
        {
            // Find the original district in the collection and update it
            District originalDistrict = DistrictCollection.FirstOrDefault(d => d.DistrictID == updatedDistrict.DistrictID);
            if (originalDistrict != null)
            {
                originalDistrict.DistrictName = updatedDistrict.DistrictName;
                originalDistrict.XPos = updatedDistrict.XPos;
                originalDistrict.YPos = updatedDistrict.YPos;

                // Notify property changes
                OnPropertyChanged(nameof(DistrictCollection));
            }
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            int districtID = updatedDistrict.DistrictID;

            // Access the ItemsControl in MainWindow.xaml
            ItemsControl itemsControl = mainWindow.FindName("districtButtonItemControl") as ItemsControl;

            // Find the corresponding button based on DistrictID
            Button targetButton = FindButtonByDistrictID(itemsControl, districtID);

            if (targetButton != null)
            {
                // Send to converter - note this step isn't really necessary, but legacy
                int xPos = (int)_adjustXConverter.Convert(updatedDistrict.XPos, null, targetButton, null);
                int yPos = (int)_adjustYConverter.Convert(updatedDistrict.YPos, null, targetButton, null);

                //Position button
                TranslateTransform newTransform = new TranslateTransform(xPos, yPos);
                targetButton.RenderTransform = newTransform;
            }
        }
        public void UpdateButtonsByDistrict(District updatedDistrict)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            int districtID = updatedDistrict.DistrictID;

            // Access the ItemsControl in MainWindow.xaml
            ItemsControl itemsControl = mainWindow.FindName("districtButtonItemControl") as ItemsControl;

            // Find the corresponding button based on DistrictID
            Button targetButton = FindButtonByDistrictID(itemsControl, districtID);

            if (targetButton != null)
            {
                // Send to converter - note this step isn't really necessary, but legacy
                int xPos = (int)_adjustXConverter.Convert(updatedDistrict.XPos, null, targetButton, null);
                int yPos = (int)_adjustYConverter.Convert(updatedDistrict.YPos, null, targetButton, null);

                //Position button
                TranslateTransform newTransform = new TranslateTransform(xPos, yPos);
                targetButton.RenderTransform = newTransform;
            }
        }
    }   
}
