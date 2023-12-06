using System.Windows;
using UC_Storyboard.Core;

namespace UC_Storyboard.MVVM.View
{
    public partial class DistrictEditDialog : Window
    {
        public DistrictEditDialog(District district)
        {
            InitializeComponent();
            DataContext = new DistrictEditViewModel(district);        
        } 
    }
}
