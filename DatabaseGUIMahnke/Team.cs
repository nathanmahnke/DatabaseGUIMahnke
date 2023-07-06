using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGUIMahnke
{
    internal class Team
    {
        public string name { get; set; }
        public string homeFieldName { get; set; }
        public int division { get; set; }
        public int winsThisSeason { get; set; }
        public int lossesThisSeason { get; set; }
        public int numPlayers { get; set; }
        public int numRosterSpots { get; set; }
    }
}
