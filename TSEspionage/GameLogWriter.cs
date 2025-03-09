/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace TSEspionage
{
    public class GameLogWriter
    {
        private readonly string _gameLogDir;

        public GameLogWriter(string gameLogDir)
        {
            if (gameLogDir == "")
            {
                _gameLogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "Twilight Struggle");
            }
            else
            {
                _gameLogDir = Path.GetFullPath(gameLogDir);
            }
        }

        public void Write(uint gameId, IEnumerable<GameLogEntry> entries)
        {
            CreateOutputDir();

            var outputFile = new StreamWriter(Path.Combine(_gameLogDir, $"{gameId}.txt"));
            foreach (var entry in entries)
            {
                var components = new List<string>();
                if (!String.IsNullOrEmpty(entry.name))
                {
                    components.Add(entry.name);
                }

                if (!String.IsNullOrEmpty(entry.desc))
                {
                    components.Add(entry.desc);
                }

                if (!String.IsNullOrEmpty(entry.detail))
                {
                    components.Add(entry.detail);
                }

                if (components.Count > 0)
                {
                    outputFile.WriteLine(String.Join(": ", components));
                }
            }

            outputFile.Close();
        }

        public void WriteGameOver(uint gameId, EPlayer winningSuperpower, GameOverType type)
        {
            var superpowerString = "";
            switch (winningSuperpower)
            {
                case EPlayer.USSR:
                    superpowerString = "USSR";
                    break;
                case EPlayer.US:
                    superpowerString = "US";
                    break;
                default:
                    superpowerString = "Unknown";
                    break;
            }

            var gameOverTypeString = "";
            switch (type)
            {
                case GameOverType.VictoryPoints:
                    gameOverTypeString = "Victory Points";
                    break;
                case GameOverType.EuropeControl:
                    gameOverTypeString = "Europe Control";
                    break;
                case GameOverType.FinalScoring:
                    gameOverTypeString = "Final Scoring";
                    break;
                case GameOverType.DEFCON:
                    gameOverTypeString = "DEFCON";
                    break;
                case GameOverType.HeldCards:
                    gameOverTypeString = "Held Scoring Card";
                    break;
                case GameOverType.CubanMissileCrisis:
                    gameOverTypeString = "Cuban Missile Crisis";
                    break;
                case GameOverType.Wargames:
                    gameOverTypeString = "WarGames";
                    break;
                case GameOverType.Forfeit:
                    gameOverTypeString = "Forfeit";
                    break;
                case GameOverType.None:
                default:
                    gameOverTypeString = "Unknown";
                    break;
            }

            CreateOutputDir();

            var outputFile = new StreamWriter(Path.Combine(_gameLogDir, $"{gameId}.txt"), true);
            outputFile.WriteLine("{0} wins by {1}", superpowerString, gameOverTypeString);

            outputFile.Close();
        }

        private void CreateOutputDir()
        {
            if (!Directory.Exists(_gameLogDir))
            {
                Directory.CreateDirectory(_gameLogDir);
            }
        }
    }

    public class GameLogEntry
    {
        public string name;
        public string desc;
        public string detail;
    }
}