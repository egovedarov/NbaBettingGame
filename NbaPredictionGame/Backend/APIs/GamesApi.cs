using NbaPredictionGame.Backend.APIs;
using NbaPredictionGame.Backend.GameObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend
{
    public class GamesApi
    {
        private const string gamesUrl = "http://data.nba.net/json/cms/2018/league/nba_games.json";
        private const string matchUrlTemplate = "http://data.nba.net/json/cms/noseason/game/{0}/00{1}/boxscore.json";
        private static List<JToken> matches = null;
        private static int latestMatchIndex = 0;
        private const int getEnoughMatches = 180;

        private static List<Game> games = new List<Game>();

        public static List<Game> Games { get => games; set => games = value; }

        public static void GetGames()
        {
            List<JToken> matches = GetAllMatches();

            foreach (JToken token in matches)
            {
                Game game = token.ToObject<Game>();
                if (DateTime.Parse(game.Dt).Date.Equals(DateTime.Today.Date))
                {
                    Games.Add(game);
                }
            }
        }

        public static List<Game> GetLastTenGames(String teamName, DateTime today)
        {
            List<Game> lastTenGames = new List<Game>();

            if (matches == null)
            {
                matches = GetAllMatches();

                foreach (JToken match in matches)
                {
                    Game game = match.ToObject<Game>();
                    SetFullTeamNames(game);
                    if (DateTime.Parse(game.Dt).Date.Equals(DateTime.Today.Date) && (game.HTeam.TeamName == teamName || game.VTeam.TeamName == teamName))
                    {
                        latestMatchIndex = matches.IndexOf(match);
                        break;
                    }
                }
                matches = matches.GetRange(latestMatchIndex - getEnoughMatches, getEnoughMatches);
            }

            int stopCount = 0;
            for (int index = getEnoughMatches-1; index >= 0; index--)
            {
                Game game = matches[index].ToObject<Game>();
                SetFullTeamNames(game);
                if (game.HTeam.TeamName == teamName || game.VTeam.TeamName == teamName)
                {
                    GetMatchScore(game);
                    lastTenGames.Add(game);
                    stopCount++;
                }
                if (stopCount == 10)
                {
                    break;
                }
            }

            return lastTenGames;
        }

        public static List<Result> SetLastTenResults(String teamName, DateTime today)
        {
            List<Game> games = GetLastTenGames(teamName, today);
            List<Result> results = new List<Result>();
            string result;
            bool isWinner;

            foreach (Game game in games)
            {
                isWinner = false;
                if ((teamName == game.HTeam.TeamName && game.Winner == 2) || (teamName == game.VTeam.TeamName && game.Winner == 1))
                {
                    isWinner = true;
                }

                result = game.VTeam.TeamName + " " + game.Score + " " + game.HTeam.TeamName;

                results.Add(new Result(result, isWinner));
            }

            return results;
        }

        public static void SetFullTeamNames(List<Game> gamesToUpdate)
        {
            foreach (Game game in games)
            {
                SetFullTeamNames(game);
            }
        }

        public static void SetFullTeamNames(Game game)
        {
            foreach (Team team in TeamsApi.Teams)
            {
                if (game.H_abrv == team.Abbreviation)
                {
                    game.HTeam = team;

                    continue;
                }
                if (game.V_abrv == team.Abbreviation)
                {
                    game.VTeam = team;
                    continue;
                }
            }
        }

        public static void GetMatchScore(Game game)
        {
            DateTime matchDate = DateTime.Parse(game.Dt);

            Result result = GetMatchScore(Convert.ToInt32(game.Id), matchDate.ToString("yyyyMMdd"));

            game.Winner = result.Winner;

            game.Score = result.ResultString;
        }

        public static Result GetMatchScore(int gameId, string matchDate)
        {
            WebClient client = new WebClient();

            string url = String.Format(matchUrlTemplate, matchDate, gameId);
            string rawJson = client.DownloadString(url);

            JObject jsonParser = JObject.Parse(rawJson);
            string visitorScore = jsonParser["sports_content"]["game"]["visitor"]["score"].Value<String>();
            string homeScore = jsonParser["sports_content"]["game"]["home"]["score"].Value<String>();

            int winner;

            if (Int32.Parse(visitorScore) > Int32.Parse(homeScore))
            {
                winner = 1;
            }
            else
            {
                winner = 2;
            }

            Result result = new Result(visitorScore + " : " + homeScore);
            result.Winner = winner;

            return result;
        }

        private static List<JToken> GetAllMatches()
        {
            WebClient client = new WebClient();

            string rawJson = client.DownloadString(gamesUrl);
            JObject jsonParser = JObject.Parse(rawJson.Replace("\"id\":0", "\"id\":1"));
            return jsonParser["sports_content"]["schedule"]["game"].Children().ToList();
        }
    }
}
