using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UC_Storyboard.Converters;
using UC_Storyboard.MVVM.ViewModel;

namespace UC_Storyboard.Core
{
    //District objects represent an entry in the districts section at the bottom of .sty files.
    //DistrictID is not included in the sty files but is inherent based on the position of the entry in the list
    //e.g. the first District is district 0, the second is 1 etc. These are linked to Mission Entries
    //Format is <DistrictName> = <X-Position on Map>,<Y-Position on Map>
    //Example: 
    //    [districts]
    //    =0,0
    //    =0,0
    //    Baseball Ground = 420,163
    public class District : INotifyPropertyChanged
    {
        private int _districtID;
        private string _districtName;
        private int _xPos;
        private int _yPos;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DistrictEventArgs> PositionChanged;
        private DistrictViewModel _districtViewModel;
        private readonly AdjustXConverter _adjustXConverter = new AdjustXConverter();
        private readonly AdjustYConverter _adjustYConverter = new AdjustYConverter();

        public void SetDistrictViewModel(DistrictViewModel districtViewModel)
        {
            _districtViewModel = districtViewModel;
        }

        public int DistrictID
        {
            get => _districtID;
            set
            {
                _districtID = value;
                OnPropertyChanged(nameof(DistrictID));
            }
        }

        public string DistrictName
        {
            get => _districtName;
            set
            {
                _districtName = value;
                OnPropertyChanged(nameof(DistrictName));
            }
        }

        public int XPos
        {
            get => _xPos;
            set
            {
                _xPos = value;
                OnPropertyChanged(nameof(XPos));
                OnPositionChanged();
            }
        }

        public int YPos
        {
            get => _yPos;
            set
            {
                _yPos = value;
                OnPropertyChanged(nameof(YPos));
                OnPositionChanged();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPositionChanged()
        {
            PositionChanged?.Invoke(this, new DistrictEventArgs(this));
            // Call the UpdateButtonByDistrict method in DistrictViewModel
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            int districtID = this.DistrictID;

            // Access the ItemsControl in MainWindow.xaml
            ItemsControl itemsControl = mainWindow.FindName("districtButtonItemControl") as ItemsControl;

            // Find the corresponding button based on DistrictID
            Button targetButton = FindButtonByDistrictID(itemsControl, districtID);

            if (targetButton != null)
            {
                // Send to converter - note this step isn't really necessary, but legacy
                int xPos = (int)_adjustXConverter.Convert(this.XPos, null, targetButton, null);
                int yPos = (int)_adjustYConverter.Convert(this.YPos, null, targetButton, null);

                //Position button
                TranslateTransform newTransform = new TranslateTransform(xPos, yPos);
                targetButton.RenderTransform = newTransform;
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

    }

    public class DistrictEventArgs : EventArgs
    {
        public District District { get; }

        public DistrictEventArgs(District district)
        {
            District = district;
            
        }
    }
}

