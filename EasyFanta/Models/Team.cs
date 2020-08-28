using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EasyFanta.Models
{
    public class Team : ObservableObject
    {
        private int id;
        private string name;
        private int availableAmount;
        private ObservableCollection<Player> players;        
        private Dictionary<string, int> maxPlayersPerRole;
        private bool isPopupOpen;
        private string popupText;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                Set<int>(() => this.Id, ref id, value);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Set<string>(() => this.Name, ref name, value);
            }
        }

        public int AvailableAmount
        {
            get
            {
                return availableAmount;
            }
            set
            {
                Set<int>(() => this.AvailableAmount, ref availableAmount, value);
            }
        }

        public ObservableCollection<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                Set<ObservableCollection<Player>>(() => this.Players, ref players, value);
            }
        }

        public bool IsPopupOpen
        {
            get
            {
                return isPopupOpen;
            }
            set
            {
                Set<bool>(() => this.IsPopupOpen, ref isPopupOpen, value);
            }
        }

        public string PopupText
        {
            get
            {
                return popupText;
            }
            set
            {
                Set<string>(() => this.PopupText, ref popupText, value);
            }
        }

        public Team(int id, string name, int availableAmount, int maxGoalkeepers, int maxDefenders, int maxMidfields, int maxForwards)
        {
            this.id = id;
            this.name = name;
            Players = new ObservableCollection<Player>();
            this.availableAmount = availableAmount;
            maxPlayersPerRole = new Dictionary<string, int>();
            maxPlayersPerRole["P"] = maxGoalkeepers;
            maxPlayersPerRole["D"] = maxDefenders;
            maxPlayersPerRole["C"] = maxMidfields;
            maxPlayersPerRole["A"] = maxForwards;
        }

        public bool AddPlayer(Player player)
        {
            bool enoughMoney = AvailableAmount - player.Price >= 0;
            bool availableSpace = GetNumberOf(player.Role) < maxPlayersPerRole[player.Role];

            if (enoughMoney && availableSpace)
            {
                Players.Add(player);
                AvailableAmount -= player.Price;
                return true;
            }

            if (!enoughMoney)
            {
                PopupText = "Not enough money!";
            } else if (!availableSpace)
            {
                PopupText = "Too many players for this role!";
            }

            IsPopupOpen = true;

            return false;
        }

        public int GetNumberOf(string role)
        {
            return Players.Count(p => p.Role.Equals(role));
        }

        public void SetMaxPlayersPerRole(int maxGoalkeepers, int maxDefenders, int maxMidfields, int maxForwards)
        {
            maxPlayersPerRole["P"] = maxGoalkeepers;
            maxPlayersPerRole["D"] = maxDefenders;
            maxPlayersPerRole["C"] = maxMidfields;
            maxPlayersPerRole["A"] = maxForwards;
        }

        public bool AddPlayers(List<Player> players)
        {
            bool oneNotAdded = false;
            foreach (Player p in players)
            {
                if (!AddPlayer(p))
                {
                    oneNotAdded = true;
                }
            }
            return oneNotAdded;
        }

        public bool RemovePlayer(Player player)
        {
            AvailableAmount += player.Price;
            return Players.Remove(player);
        }

        public bool HasPlayer(Player player)
        {
            return Players.Contains(player);
        }
    }
}
