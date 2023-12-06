using UC_Storyboard.Core;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UC_Storyboard.MVVM.View
{
    internal class MissionEditViewModel : ObservableObject
    {
        private MissionEntry _missionEntry;

        public int MissionID
        {
            get => _missionEntry.ObjectID;
            set
            {
                _missionEntry.ObjectID = value;
                OnPropertyChanged();
            }
        }

        public string MissionName
        {
            get => _missionEntry.MissionName;
            set
            {
                _missionEntry.MissionName = value;
                OnPropertyChanged();
            }
        }

        public string MissionFile
        {
            get => _missionEntry.MissionFile;
            set
            {
                _missionEntry.MissionFile = value;
                OnPropertyChanged();
            }
        }
        public int MissionDistrict
        {
            get => _missionEntry.District;
            set
            {
                _missionEntry.District = value;
                OnPropertyChanged();
            }
        }

        public string SelectedType
        {
            get => _missionEntry.Type.ToString();
            set
            {
                if (int.TryParse(value, out int type))
                {
                    _missionEntry.Type = type;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedFlag
        {
            get => _missionEntry.Flags.ToString();
            set
            {
                if (int.TryParse(value, out int flags))
                {
                    _missionEntry.Flags = flags;
                    OnPropertyChanged();
                }
            }
        }

        public int GroupID
        {
            get => _missionEntry.GroupID;
            set
            {
                _missionEntry.GroupID = value;
                OnPropertyChanged();
            }
        }

        public int UnlockedBy
        {
            get => _missionEntry.Parent;
            set
            {
                _missionEntry.Parent = value;
                OnPropertyChanged();
            }
        }

        public bool ParentIsGroup
        {
            get => _missionEntry.ParentIsGroup != 0;
            set
            {
                _missionEntry.ParentIsGroup = value ? 1 : 0;
                OnPropertyChanged();
            }
        }

        public string MissionBriefing
        {
            get => _missionEntry.MissionBriefing;
            set
            {
                _missionEntry.MissionBriefing = value;
                OnPropertyChanged();
            }
        }

        public string BriefingAudioFilePath
        {
            get => _missionEntry.BriefingAudioFilePath;
            set
            {
                _missionEntry.BriefingAudioFilePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; }

        public MissionEditViewModel(MissionEntry missionEntry)
        {
            _missionEntry = missionEntry;
           
            SubmitCommand = new RelayCommand(o =>
            {
                if (o is MissionEditDialog editDialog)
                {
                    editDialog.DialogResult = true;
                }
            });
        }
    }
}
