﻿/* MIT License

 * Copyright (c) 2021-2022 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

namespace SK.Libretro.Unity
{
    internal readonly struct SetDiskIndexBridgeCommand : IBridgeCommand
    {
        private readonly string _gamesDirectory;
        private readonly string[] _gameNames;
        private readonly int _index;

        public SetDiskIndexBridgeCommand(string gamesDirectory, string[] gameNames, int index)
        {
            _gamesDirectory = gamesDirectory;
            _gameNames      = gameNames;
            _index          = index;
        }

        public void Execute(Wrapper wrapper)
        {
            //if (wrapper.DiskHandler.Enabled && !string.IsNullOrWhiteSpace(_gamesDirectory) && _gameNames is not null && _index >= 0 && _index < _gameNames.Length)
            //    _ = wrapper.DiskHandler.SetImageIndexAuto((uint)_index, $"{_gamesDirectory}/{_gameNames[_index]}");
                
            //added by nacho here, a tiny bit of magic taken from RetroArch code that seems to work with np2kai core
            _ = wrapper.DiskHandler.AppendImageIndexAuto($"{_gamesDirectory}/{"Nostalgia 1907 (Disk 1)"}");

        }
    }
}
