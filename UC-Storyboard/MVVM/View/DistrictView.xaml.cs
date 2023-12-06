using System.Windows;
using System.Windows.Controls;
using UC_Storyboard.Core;
using UC_Storyboard.MVVM.ViewModel;
using System.Linq;
using System.Windows.Input;

namespace UC_Storyboard.MVVM.View
{
    public partial class DistrictView : UserControl
    {
        public DistrictView()
        {
            InitializeComponent();
            // Subscribe Double-Click event on the data grid to open District editor box
            districtDataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
        }
        private void AddDistrictButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DistrictViewModel districtViewModel)
            {
                districtViewModel.AddNewDistrict();
            }
        }
        private void DeleteDistrictButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DistrictViewModel districtViewModel)
            {
                // Check if there's a selected district
                if (districtDataGrid.SelectedItems.Count > 0)
                {
                    // Call the method to delete the selected missions
                    districtViewModel.DeleteDistricts(districtDataGrid.SelectedItems.Cast<District>());
                }
                else
                {
                    MessageBox.Show("Select a district before deleting.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (districtDataGrid.SelectedItem != null)
            {
                DistrictEditDialog editDialog = new DistrictEditDialog((District)districtDataGrid.SelectedItem);
                DistrictEditViewModel editViewModel = new DistrictEditViewModel((District)districtDataGrid.SelectedItem);
                editDialog.DataContext = editViewModel;

                //To-Do - figure out how to not make editDialog null so we can add an "OK" button, rather than just closing out the dialog box when finished editing
                if (editDialog.ShowDialog() == true)
                {
                }
            }
        }
        private void SelectOnMapButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Canvas districtButtonCanvas = mainWindow?.DistrictButtonCanvas;
            DistrictViewModel districtViewModel = this.DataContext as DistrictViewModel;

            // Make sure we have a district selected to set a position for...
            if (districtDataGrid.SelectedItems.Count > 0)
            {
                if (districtButtonCanvas != null && districtViewModel != null)
                {
                    districtButtonCanvas.Cursor = Cursors.Cross;

                    MouseButtonEventHandler mouseLeftButtonDownHandler = (s, args) => Canvas_MouseLeftButtonDown(s, args);
                    districtButtonCanvas.MouseLeftButtonDown += mouseLeftButtonDownHandler;

                    districtButtonCanvas.Focus();
                }
            }
            else
            {
                MessageBox.Show("Select a district before attempting to set position.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //When left button is clicked after Canvas is in focus, we retrieve the X/Y Position of the mouse on the Canvas and supply it as values to District object.
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((UIElement)sender);
            int xPosition = (int)position.X;
            int yPosition = (int)position.Y;

            DistrictViewModel districtViewModel = this.DataContext as DistrictViewModel;

            if (districtViewModel != null)
            {
                // Should still be selected, but if not, just do nothing
                if (districtDataGrid.SelectedItem is District selectedDistrict)
                {
                    selectedDistrict.XPos = xPosition;
                    selectedDistrict.YPos = yPosition;

                    districtViewModel.UpdateDistrict(selectedDistrict);
                }
            }
            ((Canvas)sender).Cursor = Cursors.Arrow;

            RemoveCanvasEventHandlers((UIElement)sender);
        }

        private void RemoveCanvasEventHandlers(UIElement canvas)
        {
            if (canvas != null)
            {
                canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            }
        }
    }
}
