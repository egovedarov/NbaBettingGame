using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.GameObjects
{
    public class Game
    {
        private string h_abrv;
        private string v_abrv;
        private string dt;
        private long id;
        private string r_reg;
        private bool is_lp;
        private bool sg;
        private Team hTeam;
        private Team vTeam;
        private string score;
        private int winner;
        private List<Result> hTeamLastTenMatches;
        private List<Result> vTeamLastTenMatches;

        public long Id { get => id; set => id = value; }
        public string V_abrv { get => v_abrv; set => v_abrv = value; }
        public string H_abrv { get => h_abrv; set => h_abrv = value; }
        public string Dt { get => dt; set => dt = value; }
        public string R_reg { get => r_reg; set => r_reg = value; }
        public bool Is_lp { get => is_lp; set => is_lp = value; }
        public bool Sg { get => sg; set => sg = value; }
        public Team HTeam { get => hTeam; set => hTeam = value; }
        public Team VTeam { get => vTeam; set => vTeam = value; }
        public string Score { get => score; set => score = value; }
        public int Winner { get => winner; set => winner = value; }
        public List<Result> HTeamLastTenMatches { get => hTeamLastTenMatches; set => hTeamLastTenMatches = value; }
        public List<Result> VTeamLastTenMatches { get => vTeamLastTenMatches; set => vTeamLastTenMatches = value; }
    }
}
