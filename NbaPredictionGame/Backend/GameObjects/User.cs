using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.GameObjects
{
    public class User
    {
        private int id;
        private string userName;
        private string password;
        private List<Bet> betMatchIds;
        private List<Bet> betHistory;
        private int score;

        public List<Bet> BetMatchIds { get => betMatchIds; set => betMatchIds = value; }
        public string Password { get => password; set => password = value; }
        public string UserName { get => userName; set => userName = value; }
        public int Score { get => score; set => score = value; }
        public int Id { get => id; set => id = value; }
        public List<Bet> BetHistory { get => betHistory; set => betHistory = value; }

        public User(int _id, string _userName, string _password, int _score)
        {
            id = _id;
            userName = _userName;
            password = _password;
            score = _score;
        }
    }
}
