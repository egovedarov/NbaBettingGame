using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.GameObjects
{
    public class Result
    {
        private readonly string resultString;
        private bool isWinner;
        private string hTeamName;
        private string vTeamName;

        public Result(string result, bool _isWinner=false)
        {
            resultString = result;
            isWinner = _isWinner;
        }

        public string ResultString => resultString;

        public bool IsWinner => isWinner;

        public int Winner { get; set; }
        public string VTeamName { get => vTeamName; set => vTeamName = value; }
        public string HTeamName { get => hTeamName; set => hTeamName = value; }
    }
}
