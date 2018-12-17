﻿using MySql.Data.MySqlClient;
using NbaPredictionGame.Backend.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.Database
{
    class DatabaseManager
    {
        public static User VerifyUser(String username, String password)
        {
            MySqlConnection connection = GetConnection();

            string query = String.Format("select * from User where binary username='{0}' and pass='{1}';", username, password);

            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader reader = null;

            User user = null;

            try
            {
                //Execute command
                reader = cmd.ExecuteReader();
                reader.Read();

                int id = (Int32)reader["id"];
                string name = (string)reader["username"];
                string pass = (string)reader["pass"];
                int points = (Int32)reader["points"];

                user = new User(id, name, pass, points);
            }
            catch (MySqlException)
            {
                return null;
            }
            finally
            {
                CloseConnection(connection);
            }

            return user;
        }

        public static List<Bet> GetUnscoredMatches(int userId)
        {
            MySqlConnection connection = GetConnection();

            string query = String.Format("select * from Bets where userId={0} and isScored=false;", userId);

            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader reader = null;

            List<Bet> bets = new List<Bet>();

            try
            {
                //Execute command
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int matchId = (Int32)reader["matchId"];
                    string prediction = (string)reader["prediction"];
                    string matchDate = (string)reader["matchDate"];

                    bets.Add(new Bet(Convert.ToString(matchId), prediction, matchDate));
                }
            }
            catch (MySqlException)
            {
                return null;
            }

            return bets;
        }

        public static List<Bet> GetLastTwentyBets(int userId)
        {
            MySqlConnection connection = GetConnection();

            string query = String.Format("select * from Bets where userId={0} and isScored=true limit 20;", userId);

            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader reader = null;

            List<Bet> bets = new List<Bet>();

            try
            {
                //Execute command
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int matchId = (Int32)reader["matchId"];
                    string prediction = (string)reader["prediction"];
                    string matchDate = (string)reader["matchDate"];
                    string winner = (string)reader["winner"];

                    bets.Add(new Bet(Convert.ToString(matchId), prediction, matchDate, winner));
                }
            }
            catch (MySqlException)
            {
                return null;
            }

            return bets;
        }

        public static User AddUser(String username, String password)
        {
            User user;

            try
            {
                MySqlConnection connection = GetConnection();

                string query = String.Format("insert into User(username, pass) values('{0}', '{1}');", username, password);

                MySqlCommand cmd = new MySqlCommand(query, connection);


                cmd.ExecuteNonQuery();

                query = String.Format("select * from User where username='{0}';", username);

                cmd = new MySqlCommand(query, connection);

                MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                int id = (Int32)reader["id"];

                user = new User(id, username, password, 0);
            }
            catch (MySqlException)
            {
                return null;
            }

            return user;
        }

        public static bool PlaceBet(User user, Game game, string winner)
        {
            try
            {
                MySqlConnection connection = GetConnection();

                string query = String.Format("insert into Bets(prediction,isScored,matchId,userId, matchDate) values('{0}',{1},{2},{3},{4});", winner, 0, game.Id, user.Id, DateTime.Today.ToString("yyyyMMdd"));

                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        public static bool DoesGameBetExist(User user, Game game)
        {
            try
            {
                MySqlConnection connection = GetConnection();

                string query = String.Format("select * from Bets where userId={0} and matchId={1};", user.Id, Convert.ToInt32(game.Id));

                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                return reader.HasRows;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public static bool UpdateUserScore(User user)
        {
            try
            {
                MySqlConnection connection = GetConnection();

                string query = String.Format("update User set points = {0} where id = {1};", user.Score, user.Id);

                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                query = String.Format("update Bets set isScored=true where userId={0};", user.Id);

                cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                foreach (Bet bet in user.BetMatchIds)
                {
                    query = String.Format("update Bets set winner={0} where userId={1} and matchId={2}", bet.ActualScore, user.Id, bet.MatchId);

                    cmd = new MySqlCommand(query, connection);

                    cmd.ExecuteNonQuery(); 
                }
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        private static MySqlConnection GetConnection()
        {
            string serverIp = "sql7.freemysqlhosting.net";
            string databaseName = "sql7268985";
            string databaseUsername = "sql7268985";
            string databasePassword = "7xzjgC4Uyj";
            string connectionString;
            connectionString = "SERVER=" + serverIp + ";" + "DATABASE=" +
            databaseName + ";" + "UID=" + databaseUsername + ";" + "PASSWORD=" + databasePassword + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        private static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
        }
    }
}