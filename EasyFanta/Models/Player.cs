using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyFanta.Models
{
    public class Player : ObservableObject
    {
        private int id;
        private string role;
        private int roleInt;
        private string team;
        private int price;
        private string name;
        private string teamImagePath;
        

        public int Id {
            get
            {
                return id;
            }
            set
            {
                Set<int>(() => this.Id, ref id, value);
            }
        }
        public string Role {
            get
            {
                return role;
            }
            set
            {
                Set<string>(() => this.Role, ref role, value);
            }
        }
        public int RoleInt {
            get
            {
                return roleInt;
            }
            set
            {
                Set<int>(() => this.RoleInt, ref roleInt, value);
            }
        }
        public string Team {
            get
            {
                return team;
            }
            set
            {
                Set<string>(() => this.Team, ref team, value);
            }
        }
        public string Name {
            get
            {
                return name;
            }
            set
            {
                Set<string>(() => this.Name, ref name, value);
            }
        }
        public int Price {
            get
            {
                return price;
            }
            set
            {
                Set<int>(() => this.Price, ref price, value);
            }
        }

        public string TeamImagePath
        {
            get
            {
                return teamImagePath;
            }
            set
            {
                Set<string>(() => this.TeamImagePath, ref teamImagePath, value);
            }
        }

        public Player(int id, string role, string name, string team, int price)
        {
            this.Id = id;
            this.Role = role;
            switch (role)
            {
                case "P":
                    this.roleInt = 0;
                    break;
                case "D":
                    this.RoleInt = 1;
                    break;
                case "C":
                    this.RoleInt = 2;
                    break;
                case "A":
                    this.RoleInt = 3;
                    break;
            }
            this.Name = name;
            this.Team = team;
            this.Price = price;
            this.TeamImagePath = "/Images/" + team + ".png";
        }

        public override bool Equals(object obj)
        {
            return obj is Player && (obj as Player).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
