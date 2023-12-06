using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UC_Storyboard.Core
{

    /// <summary>
    /// Mission Entry represents the missions in the .sty files. The MissionEntry object here includes briefing audio file path which is not included
    /// in original .sty files. Rather this is calculated in-game depending on the Mission ID. We can set custom audios for briefings by simply over-writing the relevant
    /// .wav file depending on the Mission ID in use. Not only 20 or so missions have audio. When creating a new or Templated mission collection, 
    /// Mission ID's start from 14 for this reason. There is also additional logic in the game itself that makes custom mission storyboards challenging (e.g. Mission 4 can only be played
    /// when the first 3 missions are all completed. Mission 7 only unlocks when Driving Gold is complete etc. This is all in game and cannot be over-ridden using sty files.
    /// Starting at Mission 14 bypasses all these mechanisms and allows a clean, linear story progression
    /// 
    /// Format is 
    /// Object ID : Group ID : Parent : Parent-is-group : Type : Flags : District : Filename    : Title           : Briefing
    /// 2         : 0        : 1      : 0               : 1    : 0     : 24       : FTutor1.ucm : Combat Tutorial : Follow the instructions given, in order to learn hand to hand combat.
    /// 
    /// Object ID - refers to mission number. Mission briefing audio plays depending on this number
    /// Group ID - probably was used in earlier version of STY files (this is v4!) perhaps this was used before a separate "Districts" section was introduced but currently always set to 0
    /// Parent - this value tells the game what mission needs to be completed before the current mission is open for play. Usually this is the previous mission ID.
    /// Parent-Is-Group - again probably used in earlier version of STY files - this may have been used if the previous mission was in the same group as the current mission. Always 0 now
    /// Type - Another apparently unused parameter. Currently always set to 1 - this may have been used for Multiplayer or as a marker for if its a Mako mission or a Darci mission
    /// Flags - Also seemingly unused - currently always set to 0 - may have been used as above
    /// District - This refers to the District the mission will appear in. ID is based on the order of the District in the districts list
    /// Filename - Refers to the .ucm filename for the mission. Certain .ucm filenames are used for additional logic in-game (e.g. playing the Bonus Level sound-effect) best not to use original filenames in custom campaigns
    /// Title - The title of the mission as it appears in the Map in-game
    /// Briefing - The briefing for the mission. 
    /// 
    /// All entries are separated by \r\n . Any new-lines used in mission briefings are \r only which is how the game parses them. The editor here replaces all new-lines in briefing with \r
    ///
    /// </summary>
    public class MissionEntry : INotifyPropertyChanged
    {
        private int _objectID;
        private int _groupID;
        private int _parent;
        private int _parentIsGroup;
        private int _type;
        private int _flags;
        private int _district;
        private string _missionFile;
        private string _missionName;
        private string _missionBriefing;
        private string _briefingAudioFilePath;
        public int ObjectID {
            get => _objectID;
            set
            {
                _objectID = value;
                OnPropertyChanged(nameof(ObjectID));
            }
        }
        public int GroupID {
            get => _groupID;
            set
            {
                _groupID = value;
                OnPropertyChanged(nameof(GroupID));
            }
        }
        public int Parent {
            get => _parent;
            set
            {
                _parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }
        public int ParentIsGroup {
            get => _parentIsGroup;
            set
            {
                _parentIsGroup = value;
                OnPropertyChanged(nameof(ParentIsGroup));
            }
        }
        public int Type {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public int Flags {
            get => _flags;
            set
            {
                _flags = value;
                OnPropertyChanged(nameof(Flags));
            }
        }
        public int District {
            get => _district;
            set
            {
                _district = value;
                OnPropertyChanged(nameof(District));
            }
        }
        public string MissionFile {
            get => _missionFile;
            set
            {
                _missionFile = value;
                OnPropertyChanged(nameof(MissionFile));
            }
        }
        public string MissionName {
            get => _missionName;
            set
            {
                _missionName = value;
                OnPropertyChanged(nameof(MissionName));
            }
        }
        public string MissionBriefing {
            get => _missionBriefing;
            set
            {
                _missionBriefing = value;
                OnPropertyChanged(nameof(MissionBriefing));
            }
        }
        public string BriefingAudioFilePath {
            get => _briefingAudioFilePath;
            set
            {
                _briefingAudioFilePath = value;
                OnPropertyChanged(nameof(BriefingAudioFilePath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}
