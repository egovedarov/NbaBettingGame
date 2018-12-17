using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.GameObjects
{
    public class Bet
    {
        private readonly string matchId;
        private readonly string prediction;
        private string actualScore;
        private readonly string matchDate;

        public Bet(string _matchId, string _prediction, string _matchDate,string _actualScore = "")
        {
            matchId = _matchId;
            prediction = _prediction;
            ActualScore = _actualScore;
            matchDate = _matchDate;
        }

        public string ActualScore { get => actualScore; set => actualScore = value; }

        public string MatchId => matchId;

        public string MatchDate => matchDate;

        public string Prediction => prediction;
    }
}
