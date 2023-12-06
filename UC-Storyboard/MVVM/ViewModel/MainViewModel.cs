using System;
using UC_Storyboard.Core;

namespace UC_Storyboard.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand AboutViewCommand { get; set; }
        public RelayCommand DistrictViewCommand { get; set; }
        public RelayCommand MissionViewCommand { get; set; }

        public AboutViewModel AboutVM;
        public DistrictViewModel DistrictVM;
        public MissionViewModel MissionVM;

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            AboutVM = new AboutViewModel();
            DistrictVM = new DistrictViewModel();
            MissionVM = new MissionViewModel();

            CurrentView = AboutVM;

            AboutViewCommand = new RelayCommand(o =>
            {
                CurrentView = AboutVM;
            });
            DistrictViewCommand = new RelayCommand(o =>
            {
                CurrentView = DistrictVM;
            });
            MissionViewCommand = new RelayCommand(o =>
            {
                CurrentView = MissionVM;
            });

        }
    }
}
