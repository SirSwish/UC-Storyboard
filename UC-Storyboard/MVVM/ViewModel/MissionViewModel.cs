using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UC_Storyboard.Core;

namespace UC_Storyboard.MVVM.ViewModel
{
    class MissionViewModel : ObservableObject
    {
        private ObservableCollection<MissionEntry> _missionCollection;

        public ObservableCollection<MissionEntry> MissionCollection
        {
            get { return _missionCollection; }
            set
            {
                _missionCollection = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<District> _districtCollection;

        public ObservableCollection<District> DistrictCollection
        {
            get { return _districtCollection; }
            set
            {
                _districtCollection = value;
                OnPropertyChanged();
            }
        }
        public MissionViewModel()
        {
            // Used to populate data grid - again this is done from file load in context of Main Window
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MissionListCalculated += OnMissionListCalculated;
                mainWindow.MissionListInitialized += OnMissionListInitialized;
            }
        }

        public List<string> DistrictNames
        {
            get { return DistrictCollection.Select(d => d.DistrictName).ToList(); }
        }

        public void AddNewMission()
        {
            if (MissionCollection == null)
            {
                // Initialize MissionCollection if there isn't one
                MissionCollection = new ObservableCollection<MissionEntry>();
            }

            // Check if the count of items is less than 20 - we have to do this because there are only 20 consecutive mission ID's with Audio briefings available
            if (MissionCollection.Count < 20)
            {
                // Generate a new Mission with predefined values
                MissionEntry newMission = new MissionEntry
                {
                    ObjectID = GetNextMissionID(),
                    MissionName = "New Mission",
                    GroupID = 0,
                    Parent = (GetNextMissionID() - 1 >= 14) ? (GetNextMissionID() - 1) : (GetNextMissionID() - 1 == 13) ? 1 : GetNextMissionID() - 1, //If empty, start from Mission 14
                    ParentIsGroup = 0,
                    Type = 1,
                    Flags = 0,
                    District = 0,
                    MissionFile = "newLevel.ucm",
                    MissionBriefing = "This is a sample mission briefing for a newly generated mission. \r \r - Replace with your own content \r - Have a nice day.",
                    BriefingAudioFilePath = ""
                };

                // Add the new Mission to the ObservableCollection
                MissionCollection.Add(newMission);
            }
            if (MissionCollection.Count == 20) 
            {
                MessageBox.Show("A maximum of 20 missions are allowed. This is because only 20 levels have modifiable briefing audios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        //Create a 10-mission linear template
        public void createTemplate()
        {
            if (MissionCollection == null)
            {
                MissionCollection = new ObservableCollection<MissionEntry>();
            }

            for (int i = 1; i <= 10; i++)
            {
                // Generate 10 Missions with predefined values
                MissionEntry newMission = new MissionEntry
                {
                    ObjectID = GetNextMissionID(),
                    MissionName = $"New Mission {i}",
                    GroupID = 0,
                    Parent = (GetNextMissionID() - 1 >= 14) ? (GetNextMissionID() - 1) : (GetNextMissionID() - 1 == 13) ? 1 : GetNextMissionID() - 1,
                    ParentIsGroup = 0,
                    Type = 1,
                    Flags = 0,
                    District = 0,
                    MissionFile = $"newLevel{i}.ucm",
                    MissionBriefing = "This is a sample mission briefing for a newly generated mission. \r \r - Replace with your own content \r - Have a nice day.",
                    BriefingAudioFilePath = ""
                };

                MissionCollection.Add(newMission);
            }
        }

        private int GetNextMissionID()
        {
            // If DistrictCollection is not initialized, return 14 as the first ID
            if (MissionCollection == null || MissionCollection.Count == 0)
            {
                return 14;
            }

            int maxId = MissionCollection.Max(d => d.ObjectID);
            return maxId + 1;
        }
        public void DeleteMissions(IEnumerable<MissionEntry> missions)
        {
            foreach (var mission in missions.ToList())
            {
                MissionCollection.Remove(mission);
            }
        }

        private void OnMissionListCalculated(object sender, ObservableCollection<MissionEntry> missions)
        {
            MissionCollection = missions;
        }
        private void OnMissionListInitialized(object sender, ObservableCollection<MissionEntry> missions)
        {
            MissionCollection = missions;
        }
    }
}
