using NbaPredictionGame.Backend.GameObjects;
using Newtonsoft.Json;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NbaPredictionGame.Backend.APIs
{
    public class TeamsApi
    {
        private static bool updateTeamIcons = false;

        private static List<Team> teams = new List<Team>();

        private const string teamsUrl = "https://raw.githubusercontent.com/bttmly/nba/master/data/teams.json";

        public static bool UpdateTeamIcons { get { return updateTeamIcons; } set { updateTeamIcons = value; } }

        public static List<Team> Teams { get { return teams; } set { teams = value; } }

        internal static void GetTeams()
        {
            WebClient client = new WebClient();

            string rawJson = client.DownloadString(teamsUrl);
            Teams = JsonConvert.DeserializeObject<List<Team>>(rawJson);

            if (UpdateTeamIcons)
            {
                UpdateIcons();
            }

            string resourcesDirectory = @"C:\Users\MARIN\Desktop\NbaPredictionGame\NbaPredictionGame\NbaPredictionGame";

            foreach (Team team in Teams)
            {
                team.Image = String.Format(@"{0}\Resources\{1}.png", resourcesDirectory, team.TeamName);
            }
        }

        public static void UpdateIcons()
        {
            string url = null;
            string iconFile = null;

            foreach (Team team in Teams)
            {
                url = String.Format("https://www.nba.com/.element/img/1.0/teamsites/logos/teamlogos_500x500/{0}.png", team.Abbreviation.ToLower());
                iconFile = @"C:\Users\MARIN\Desktop\NbaPredictionGame\NbaPredictionGame\NbaPredictionGame\Resources\" + team.TeamName + ".png";
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(url), iconFile);
                }

                //var svgDocument = Svg.SvgDocument.Open(iconFile);  
                //svgDocument.ShapeRendering = SvgShapeRendering.Auto;

                //Bitmap bmp = svgDocument.Draw(200, 200);                          
                //bmp.Save(iconFile.Replace("svg","png"), ImageFormat.Png);

                //File.Delete(iconFile);
            }
        }
    }
}
