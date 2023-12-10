﻿using System;
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
                outputFile.WriteLine("{0}: {1}: {2}", entry.name, entry.desc, entry.detail);
            }

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
