using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    internal class ScoreManager
    {

        private static string GetScoreFilePath()
        {
            // Get the application directory
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine with a file name to get the full path
            string filePath = Path.Combine(appDirectory, "previous.txt");

            return filePath;
        }

        public static void SaveScore(int score)
        {
            // Get the file path
            string filePath = GetScoreFilePath();

            // Write the score to the file
            File.WriteAllText(filePath, score.ToString());
        }


        public static int LoadScore()
        {
            // Get the file path
            string filePath = GetScoreFilePath();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the score from the file
                string scoreText = File.ReadAllText(filePath);

                // Try to parse the score
                if (int.TryParse(scoreText, out int score))
                {
                    return score;
                }
            }

            // Return a default score if the file doesn't exist or parsing fails
            return 0;
        }
    }
}
