using EasyFanta.Models;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace EasyFanta.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AppSettings settings;
        private ObservableCollection<Player> allAvailablePlayers;
        private ObservableCollection<Team> allTeams;
        private ICollectionView allAvailablePlayersView;
        

        public ICollectionView AllAvailablePlayersView
        {
            get { return allAvailablePlayersView; }
        }

        public ObservableCollection<Player> AllAvailablePlayers 
        {
            get
            {
                return allAvailablePlayers;
            }
            set
            {
                Set<ObservableCollection<Player>>(() => this.AllAvailablePlayers, ref allAvailablePlayers, value);
            }
        }
        public ObservableCollection<Team> AllTeams
        {
            get
            {
                return allTeams;
            }
            set
            {
                Set<ObservableCollection<Team>>(() => this.AllTeams, ref allTeams, value);
            }
        }


        public MainViewModel(IOptions<AppSettings> options)
        {
            settings = options.Value;
            AllAvailablePlayers = new ObservableCollection<Player>();
            allAvailablePlayersView = CollectionViewSource.GetDefaultView(AllAvailablePlayers);
            allAvailablePlayersView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
            AllTeams = new ObservableCollection<Team>();

            ReloadTeams();
            ReloadPlayers();

            //CreateFakeTeams(8);
            //SaveTeams(settings.TeamsFilepath);
            
        }

        public void ReloadPlayers()
        {
            ReloadPlayers(settings.PlayersFilepath);
        }

        public void ReloadPlayers(string filePath)
        {
            AllAvailablePlayers.Clear();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorkbook workbook = package.Workbook;
                if (workbook != null)
                {
                    if (workbook.Worksheets.Count > 0)
                    {
                        ExcelWorksheet worksheet = workbook.Worksheets.First();
                        foreach (Player p in WorksheetToPlayers(worksheet))
                        {
                            if (!IsPlayerAlreadyOwned(p))
                            {
                                AllAvailablePlayers.Add(p);
                            }                            
                        }                        
                    }
                }
            }            
        }

        private List<Player> WorksheetToPlayers(ExcelWorksheet worksheet)
        {
            List<Player> players = new List<Player>();

            int rows = worksheet.Dimension.End.Row;

            if (worksheet.Cells[1, 1].Value.Equals("Quotazioni Fantaclub Serie A"))
            {
                for (int i = 6; i <= rows; i++)
                {
                    players.Add(new Player(
                        int.Parse(worksheet.Cells[i, 1].Value.ToString()),
                        worksheet.Cells[i, 4].Value.ToString(),
                        worksheet.Cells[i, 2].Value.ToString(),
                        worksheet.Cells[i, 3].Value.ToString(),
                        int.Parse(worksheet.Cells[i, 7].Value.ToString())));
                }
            }
            else
            {
                for (int i = 2; i <= rows; i++)
                {
                    players.Add(new Player(
                        int.Parse(worksheet.Cells[i, 1].Value.ToString()),
                        worksheet.Cells[i, 2].Value.ToString(),
                        worksheet.Cells[i, 3].Value.ToString(),
                        worksheet.Cells[i, 4].Value.ToString(),
                        int.Parse(worksheet.Cells[i, 5].Value.ToString())));
                }
            }

            return players;
        }

        public bool IsPlayerAlreadyOwned(Player player)
        {           
            foreach(Team t in AllTeams)
            {
                if (t.HasPlayer(player))
                {
                    return true;
                }
            }

            return false;
        }        

        public void AddPlayerToAvailablePlayers(Player player)
        {
            AllAvailablePlayers.Add(player);
            AllAvailablePlayersView.Refresh();
        }


        public void ReloadTeams()
        {
            ReloadTeams(settings.TeamsFilepath);
        }

        public void ReloadTeams(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    AllTeams = (ObservableCollection<Team>)serializer.Deserialize(file, typeof(ObservableCollection<Team>));

                    foreach(Team t in AllTeams)
                    {
                        t.SetMaxPlayersPerRole(settings.MaxGoalKeepers, settings.MaxDefenders, settings.MaxMidfields, settings.MaxForwards);
                    }
                }
            }
        }

        public void SaveTeams()
        {
            SaveTeams(settings.TeamsFilepath);
        }
        public void SaveTeams(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                new JsonSerializer().Serialize(writer, AllTeams);                
            }
        }

        public void AddTeam(string name)
        {
            AllTeams.Add(new Team(AllTeams.Count, 
                                  string.IsNullOrEmpty(name) ? "Team" + AllTeams.Count : name, 
                                  settings.AvailableAmount,
                                  settings.MaxGoalKeepers,
                                  settings.MaxDefenders,
                                  settings.MaxMidfields,
                                  settings.MaxForwards));
        }

        private void CreateFakeTeams(int numberOfTeams)
        {
            for(int i=0; i<numberOfTeams; i++)
            {
                Team t = new Team(i, 
                                  "Team" + i, 
                                  settings.AvailableAmount,
                                  settings.MaxGoalKeepers,
                                  settings.MaxDefenders,
                                  settings.MaxMidfields,
                                  settings.MaxForwards);
                t.AddPlayer(AllAvailablePlayers[i]);
                t.AddPlayer(AllAvailablePlayers[i + 1]);
                t.AddPlayer(AllAvailablePlayers[i + 2]);
                t.AddPlayer(AllAvailablePlayers[i + 3]);
                t.AddPlayer(AllAvailablePlayers[i + 4]);

                AllTeams.Add(t);
            }
        }
    }
}
