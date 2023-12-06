using UC_Storyboard.Core;
using System.Windows.Input;

namespace UC_Storyboard.MVVM.View
{
    internal class DistrictEditViewModel : ObservableObject
    {
        private District _district;

        public int DistrictID
        {
            get => _district.DistrictID;
            set
            {
                _district.DistrictID = value;
                OnPropertyChanged();
            }
        }

        public string DistrictName
        {
            get => _district.DistrictName;
            set
            {
                _district.DistrictName = value;
                OnPropertyChanged();
            }
        }

        public int XPos
        {
            get => _district.XPos;
            set
            {
                _district.XPos = value;
                OnPropertyChanged();
            }
        }

        public int YPos
        {
            get => _district.YPos;
            set
            {
                _district.YPos = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; }

        public DistrictEditViewModel(District district)
        {
            _district = district;

            SubmitCommand = new RelayCommand(o =>
            {
            });
        }
    }
}
