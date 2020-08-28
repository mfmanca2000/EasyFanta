using EasyFanta.Models;
using EasyFanta.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyFanta.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point? _startPoint;

        public static RoutedCommand SearchCommand = new RoutedCommand();
        
        public MainWindow()
        {
            InitializeComponent();

            SearchCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));            
        }

        private void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            searchTxt.Focus();
            searchTxt.Select(0, searchTxt.Text.Length);
        }

        private void searchTxtKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
                Player found = vm.AllAvailablePlayers.Where(p => p.Name.ToLower().Replace(".", "").StartsWith(searchTxt.Text.ToLower())).FirstOrDefault();
                allPlayersGrid.SelectedItem = found;
                if (found != null)
                {
                    allPlayersGrid.ScrollIntoView(found);
                    searchTxt.Select(0, searchTxt.Text.Length);
                    allPlayersGrid.Focus();
                }
            }
        }


        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
            Player found = vm.AllAvailablePlayers.Where(p => p.Name.ToLower().Replace(".", "") == searchTxt.Text.ToLower()).FirstOrDefault();
            allPlayersGrid.SelectedItem = found;
            if (found != null)
            {
                allPlayersGrid.ScrollIntoView(found);
                searchTxt.Select(0, searchTxt.Text.Length);
                allPlayersGrid.Focus();
            }
            
        }


        private void ReloadPlayersBtnClick(object sender, RoutedEventArgs e)
        {
            var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
            vm.ReloadPlayers();
        }

        private void ClearBtnClick(object sender, RoutedEventArgs e)
        {
            searchTxt.Text = string.Empty;
        }

        private void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void DataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // No drag operation
            if (_startPoint == null)
                return;

            var dg = sender as DataGrid;
            if (dg == null) return;
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint.Value - mousePos;
            // test for the minimum displacement to begin the drag
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {

                // Get the dragged DataGridRow
                var DataGridRow =
                    FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);

                if (DataGridRow == null)
                    return;
                // Find the data behind the DataGridRow
                var dataTodrop = (Player)dg.ItemContainerGenerator.ItemFromContainer(DataGridRow);

                if (dataTodrop == null) return;

                // Initialize the drag & drop operation
                var dataObj = new DataObject(dataTodrop);
                dataObj.SetData("DragSource", sender);
                DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Move);
                _startPoint = null;
            }
        }

        private void dataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _startPoint = null;
        }



        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void SaveBtnClick(object sender, RoutedEventArgs e)
        {
            var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
            vm.SaveTeams();
        }

        private void AddTeamBtnClick(object sender, RoutedEventArgs e)
        {
            var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
            var i = new Random().Next(100);
            vm.AddTeam(null);
        }

        

        
    }
}
