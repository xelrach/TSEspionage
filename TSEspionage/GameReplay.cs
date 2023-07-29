/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace TSEspionage
{
    public static class GameReplay
    {
        internal static readonly byte[] MagicNumber =
        {
            0xC8, 0x29, 0xA1, 0xEF, 0x61, 0x5E, 0xB9, 0x6F
        };

        private static readonly Dictionary<uint, GameReplayWriter>
            EventWriters = new Dictionary<uint, GameReplayWriter>();

        public static GameReplayWriter GetGameReplayWriter(uint gameId)
        {
            if (!EventWriters.ContainsKey(gameId))
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Playdek",
                    "Twilight Struggle");
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not open or create replay directory: {directory}", e);
                }

                var path = Path.Combine(directory, $"{gameId}.pts");
                EventWriters.Add(gameId, new GameReplayWriter(File.Open(path, FileMode.Append)));
            }

            return EventWriters[gameId];
        }

        public static void CloseGameReplayWriter(uint gameId)
        {
            if (EventWriters.ContainsKey(gameId))
            {
                var writer = EventWriters[gameId];
                writer.Dispose();
                EventWriters.Remove(gameId);
            }
        }
    }

    public class GameReplayWriter : IDisposable
    {
        private readonly FileStream _stream;

        public GameReplayWriter(FileStream stream)
        {
            _stream = stream;
            _stream.Write(GameReplay.MagicNumber, 0, 8);
            _stream.Flush();
        }

        public void WriteGameEvent(Playdek.TwilightStruggle.V1.GameEvent replayEntry)
        {
            replayEntry.WriteDelimitedTo(_stream);
            _stream.Flush();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }

    public class GameReplayReader : IDisposable
    {
        private readonly FileStream _stream;
        private List<Playdek.TwilightStruggle.V1.GameEvent> _replay;

        public GameReplayReader(FileStream stream)
        {
            _stream = stream;
            _replay = new List<Playdek.TwilightStruggle.V1.GameEvent>();
        }

        public void Dispose()
        {
            _stream.Dispose();
            _replay.Clear();
        }
    }
}