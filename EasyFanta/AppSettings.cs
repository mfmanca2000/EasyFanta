using System;
using System.Collections.Generic;
using System.Text;

namespace EasyFanta
{
    public class AppSettings
    {
        public int AvailableAmount { get; set; }
        public string PlayersFilepath { get; set; }
        public string TeamsFilepath { get; set; }

        public int MaxGoalKeepers { get; set; }
        public int MaxDefenders { get; set; }
        public int MaxMidfields { get; set; }
        public int MaxForwards { get; set; }

    }
}
