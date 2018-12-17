using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.GameObjects
{
    public class Team
    {
        private long teamId;
        private string abbreviation;
        private string teamName;
        private string simpleName;
        private string location;
        private string image;
        private List<string> lastTenMatches;

        public long TeamId { get => teamId; set => teamId = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }
        public string SimpleName { get => simpleName; set => simpleName = value; }
        public string Location { get => location; set => location = value; }
        public string TeamName { get => teamName; set => teamName = value; }
        public string Image { get => image; set => image = value; }
        public List<string> LastTenMatches { get => lastTenMatches; set => lastTenMatches = value; }
    }
}
