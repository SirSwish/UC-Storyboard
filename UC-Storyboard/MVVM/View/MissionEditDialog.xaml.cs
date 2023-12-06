using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using UC_Storyboard.Core;

namespace UC_Storyboard.MVVM.View
{
    public partial class MissionEditDialog : Window
    {
        public MissionEditDialog(MissionEntry missionEntry)
        {
            InitializeComponent();
            DataContext = new MissionEditViewModel(missionEntry);
        }
        //To-Do: Add validation, game only takes .wav Files I think...
        private void SelectBriefingAudioFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Briefing Audio File",
                Filter = "Audio Files|*.wav|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (DataContext is MissionEditViewModel viewModel)
                {
                    viewModel.BriefingAudioFilePath = openFileDialog.FileName;
                }
            }
        }
    }
}
