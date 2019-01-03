using NbaPredictionGame.Backend.APIs;
using NbaPredictionGame.Backend.GameObjects;
using NbaPredictionGame.Views;
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

        private static List<Game> games = new List<Game>();

        public static List<Game> Games { get => games; set => games = value; }

        public static void GetGames()
        {
            List<JToken> matches = GetAllMatches();
            bool isFirst = false;

            foreach (JToken token in matches)
            {
                Game game = token.ToObject<Game>();
                if (DateTime.Parse(game.Dt).Date.Equals(Login.date.Date))
                {
                    Games.Add(game);
                    if (!isFirst)
                    {
                        latestMatchIndex = matches.IndexOf(token);
                        isFirst = true;
                    }
                }
            }
        }

        public static Dictionary<Game, List<Game>> GetLastTenGames(List<Game> games, bool isLastTenHomeGames, DateTime today)
        {
            Dictionary<Game, List<Game>> lastTenHomeGames = new Dictionary<Game, List<Game>>();
            Dictionary<Game, List<Game>> lastTenVisitorGames = new Dictionary<Game, List<Game>>();
            int numberOfAllOldGames = 10 * games.Count;

            foreach (Game game in games)
            {
                List<Game> lastGamesList = new List<Game>();
                lastTenHomeGames.Add(game, lastGamesList);

                List<Game> lastGamesList2 = new List<Game>();
                lastTenVisitorGames.Add(game, lastGamesList);
            }

            matches = GetAllMatches();

            if (isLastTenHomeGames)
            {
                int stopCountH = 0;
                List<Game> lastGames = null;
                for (int index = latestMatchIndex - 1; index >= 0; index--)
                {
                    Game oldGame = matches[index].ToObject<Game>();
                    SetFullTeamNames(oldGame);

                    foreach (Game game in games)
                    {
                        if (oldGame.HTeam.TeamName == game.HTeam.TeamName || oldGame.VTeam.TeamName == game.HTeam.TeamName)
                        {
                            if (lastTenHomeGames[game].Count == 10)
                            {
                                break;
                            }
                            SetMatchScore(oldGame);
                            lastGames = lastTenHomeGames[game];
                            lastGames.Add(oldGame);
                            lastTenHomeGames[game] = lastGames;
                            stopCountH++;
                            break;
                        }
                    }
                    if (stopCountH == numberOfAllOldGames)
                    {
                        break;
                    }
                }
            }
            else
            {
                int stopCountV = 0;
                List<Game> lastGames = null;
                for (int index = latestMatchIndex - 1; index >= 0; index--)
                {
                    Game oldGame = matches[index].ToObject<Game>();
                    SetFullTeamNames(oldGame);

                    foreach (Game game in games)
                    {
                        if (oldGame.HTeam.TeamName == game.VTeam.TeamName || oldGame.VTeam.TeamName == game.VTeam.TeamName)
                        {
                            if (lastTenVisitorGames[game].Count == 10)
                            {
                                continue;
                            }
                            SetMatchScore(oldGame);
                            lastGames = lastTenVisitorGames[game];
                            lastGames.Add(oldGame);
                            lastTenVisitorGames[game] = lastGames;
                            stopCountV++;
                            break;
                        }
                    }
                    if (stopCountV == numberOfAllOldGames)
                    {
                        break;
                    }
                }
            }

            return isLastTenHomeGames ? lastTenHomeGames : lastTenVisitorGames;
        }

        public static void SetLastTenResults(List<Game> games, bool isLastTenHomeGames, DateTime today)
        {
            Dictionary<Game, List<Game>> lastTenGames = GetLastTenGames(games, isLastTenHomeGames, today);

            string result;
            bool isWinner;

            foreach (KeyValuePair<Game, List<Game>> entry in lastTenGames)
            {
                List<Result> results = new List<Result>();
                foreach (Game game in entry.Value)
                {
                    isWinner = false;

                    if (isLastTenHomeGames)
                    {
                        if ((entry.Key.HTeam.TeamName == game.HTeam.TeamName && game.Winner == 1) || (entry.Key.HTeam.TeamName == game.VTeam.TeamName && game.Winner == 2))
                        {
                            isWinner = true;
                        }
                    }
                    else
                    {
                        if ((entry.Key.VTeam.TeamName == game.HTeam.TeamName && game.Winner == 1) || (entry.Key.VTeam.TeamName == game.VTeam.TeamName && game.Winner == 2))
                            isWinner = true;
                    }

                    result = game.VTeam.TeamName + " " + game.Score + " " + game.HTeam.TeamName;

                    results.Add(new Result(result, isWinner));
                }

                if (isLastTenHomeGames)
                {
                    entry.Key.HTeamLastTenMatches = results;
                }
                else
                {
                    entry.Key.VTeamLastTenMatches = results;
                }
                Console.WriteLine("");
            }
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

        public static void SetMatchScore(Game game)
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
            string vTeamAbbreviation = jsonParser["sports_content"]["game"]["visitor"]["abbreviation"].Value<String>();
            string homeScore = jsonParser["sports_content"]["game"]["home"]["score"].Value<String>();
            string hTeamAbbreviation = jsonParser["sports_content"]["game"]["home"]["abbreviation"].Value<String>();

            int winner;

            if (Int32.Parse(visitorScore) > Int32.Parse(homeScore))
            {
                winner = 2;
            }
            else
            {
                winner = 1;
            }

            Result result = new Result(visitorScore + " : " + homeScore);
            result.Winner = winner;

            int breakIndex = 0;
            foreach (Team team in TeamsApi.Teams)
            {
                if (team.Abbreviation == vTeamAbbreviation)
                {
                    breakIndex++;
                    result.VTeamName = team.TeamName;
                }
                else if (team.Abbreviation == hTeamAbbreviation)
                {
                    breakIndex++;
                    result.HTeamName = team.TeamName;
                }

                if (breakIndex == 2)
                {
                    break;
                }
            }

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
