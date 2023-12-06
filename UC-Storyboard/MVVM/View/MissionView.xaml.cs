using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UC_Storyboard.Core;
using UC_Storyboard.MVVM.ViewModel;

namespace UC_Storyboard.MVVM.View
{
    public partial class MissionView : UserControl
    {
        public MissionView()
        {
            InitializeComponent();
            missionDataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
        }

        private void AddMissionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MissionViewModel missionViewModel)
            {
                missionViewModel.AddNewMission();
            }
        }

        private void DeleteMissionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MissionViewModel missionViewModel)
            {
                // Make sure at least one mission is selected
                if (missionDataGrid.SelectedItems.Count > 0)
                {
                    missionViewModel.DeleteMissions(missionDataGrid.SelectedItems.Cast<MissionEntry>());
                }
                else
                {
                    MessageBox.Show("Please select at least one mission to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddMissionTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MissionViewModel missionViewModel)
            {
                // Templates start at ID 14 (to avoid weird game logic around the lower mission ID's. So if we're going to use a template, it needs to already be empty.
                if (missionDataGrid.Items.IsEmpty)
                {
                    missionViewModel.createTemplate();
                }
                else
                {
                    MessageBox.Show("Please delete all missions to create from template", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (missionDataGrid.SelectedItem != null)
            {
                MissionEditDialog editDialog = new MissionEditDialog((MissionEntry)missionDataGrid.SelectedItem);
                MissionEditViewModel editViewModel = new MissionEditViewModel((MissionEntry)missionDataGrid.SelectedItem);
                editDialog.DataContext = editViewModel;

                //To-Do: Like districts, can't seem to make editDialog != null when applying an OK button
                if (editDialog.ShowDialog() == true)
                {
                    
                }
            }
        }
    }
}
