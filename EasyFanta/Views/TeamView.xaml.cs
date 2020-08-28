using EasyFanta.Models;
using EasyFanta.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for TeamView.xaml
    /// </summary>
    public partial class TeamView : UserControl
    {
        public TeamView()
        {
            InitializeComponent();
        }

        private void ListBoxDrop(object sender, DragEventArgs e)
        {
            var dg = sender as ListBox;
            if (dg == null) return;
            var dgSrc = e.Data.GetData("DragSource") as DataGrid;
            var data = e.Data.GetData(typeof(Player));
            if (dgSrc == null || data == null) return;

            var team = dg.DataContext as Team;
            if (team.AddPlayer(data as Player))
            {
                var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
                vm.AllAvailablePlayers.Remove(data as Player);
            }                       
        }

        private void RemovePlayerBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Player p = btn.Tag as Player;
            var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
            vm.AddPlayerToAvailablePlayers(p);
            Team t = this.DataContext as Team;
            t.RemovePlayer(p);
        }
    }
}
