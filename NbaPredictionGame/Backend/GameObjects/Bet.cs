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
        private string vTeamName;
        private string hTeamName;
        private string vTeamImage;
        private string hTeamImage;
        private string score;
        private string predictionTeamName;
        private bool isPredictionCorrect = false;
        private string dateToVisialize;

        public Bet(string _matchId, string _prediction, string _matchDate, string _actualScore = "")
        {
            matchId = _matchId;
            prediction = _prediction;
            ActualScore = _actualScore;
            matchDate = _matchDate;
        }

        public string MatchId => matchId;

        public string MatchDate => matchDate;

        public string Prediction => prediction;

        public string ActualScore { get => actualScore; set => actualScore = value; }
        public string VTeamImage { get => vTeamImage; set => vTeamImage = value; }
        public string HTeamImage { get => hTeamImage; set => hTeamImage = value; }
        public string Score { get => score; set => score = value; }
        public string HTeamName { get => hTeamName; set => hTeamName = value; }
        public string VTeamName { get => vTeamName; set => vTeamName = value; }
        public string PredictionTeamName { get => predictionTeamName; set => predictionTeamName = value; }
        public bool IsPredictionCorrect { get => isPredictionCorrect; set => isPredictionCorrect = value; }
        public string DateToVisialize { get => dateToVisialize; set => dateToVisialize = value; }
    }
}
