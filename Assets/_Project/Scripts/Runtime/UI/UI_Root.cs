/* MIT License

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

using SK.Libretro.Unity;
using Unity.Mathematics;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, DefaultExecutionOrder(1000)]
    public sealed class UI_Root : MonoBehaviour
    {
        [SerializeField] private LibretroInstanceVariable _libretro;
        [SerializeField] private UI_Toolbar _toolbar;
        [SerializeField] private UI_Button _gameButton;
        [SerializeField] private UI_ToolbarMenu _gameMenu;
        [SerializeField] private UI_Button _gameStartButton;
        [SerializeField] private UI_Button _gameRestartButton;
        [SerializeField] private UI_Button _gameStopButton;
        [SerializeField] private UI_ToolbarMenu _stateMenu;
        [SerializeField] private UI_Button _stateSave;
        [SerializeField] private UI_Button _diskButton;
        [SerializeField] private UI_ToolbarMenu _diskMenu;
        [SerializeField] private UI_Button _diskDecreaseIndexButton;
        [SerializeField] private UI_Button _diskIncreaseIndexButton;
        [SerializeField] private UI_Button _diskReplaceButton;
        [SerializeField] private UI_Button _memoryButton;
        [SerializeField] private UI_ToolbarMenu _memoryMenu;
        [SerializeField] private UI_Button _memorySaveSRAMButton;
        [SerializeField] private UI_Button _memoryLoadSRAMButton;
        [SerializeField] private UI_Button _coreOptionsButton;
        [SerializeField] private UI_CoreOptionsMenu _coreOptionsMenu;
        [SerializeField] private UI_Button[] _stateSaveButtons;
        [SerializeField] private UI_Button[] _stateLoadButtons;

        private int _stateSlot = 1;
        private int _diskIndex;


        private void Start()
        {
            for (int i = 0; i < _stateSaveButtons.Length; i++)
            {
                int selectedSlot = i + 1;
                _stateSaveButtons[i].Construct(true, true, () => OnStateItemSelected(selectedSlot));
            }
            for (int i = 0; i < _stateLoadButtons.Length; i++)
            {
                int selectedSlot = i + 1;
                _stateLoadButtons[i].Construct(true, true, () => OnStateLoadSelected(selectedSlot));
            }

            _libretro.OnInstanceChanged += LibretroInstanceChangedCallback;
            LibretroInstanceChangedCallback(_libretro.Current);
            _gameStartButton.Construct(true, true, () => _libretro.StartContent());
            _gameRestartButton.Construct(true, false, () => OnstateLoadDefault());
            _gameStopButton.Construct(true, false, () => _libretro.StopContent());
            _stateSave.Construct(true, false, () => _stateSave.SetVisible(false));
        }
        private void OnEnable()
        {
            /* _libretro.OnInstanceChanged += LibretroInstanceChangedCallback;
             LibretroInstanceChangedCallback(_libretro.Current);

             _gameResetButton.Construct(true, false, () => _libretro.ResetContent());
             _gameStopButton.Construct(true, false, () => _libretro.StopContent()); */


            /* diskButton.Construct(true, false, () => _diskMenu.SetVisible(true));
             _diskMenu.Construct(false, _libretro);
             _diskDecreaseIndexButton.Construct(true, false, () =>
             {
                 _diskIndex = math.max(0, --_diskIndex);
                 _diskReplaceButton.Text = $"Replace ({_diskIndex})";
                 _diskDecreaseIndexButton.SetInteractable(_diskIndex > 0);
                 _diskIncreaseIndexButton.SetInteractable(_diskIndex < 999999);
             });
             _diskIncreaseIndexButton.Construct(true, true, () =>
             {
                 _diskIndex = math.min(++_diskIndex, 999999);
                 _diskReplaceButton.Text = $"Replace ({_diskIndex})";
                 _diskDecreaseIndexButton.SetInteractable(_diskIndex > 0);
                 _diskIncreaseIndexButton.SetInteractable(_diskIndex < 999999);
             });
             _diskReplaceButton.Construct(true, true, () => _libretro.SetDiskIndex(_diskIndex));

             _memoryButton.Construct(true, false, () => _memoryMenu.SetVisible(true));
             _memoryMenu.Construct(false, _libretro);
             _memorySaveSRAMButton.Construct(true, true, () => _libretro.SaveSRAM());
             _memoryLoadSRAMButton.Construct(true, true, () => _libretro.LoadSRAM());

             _coreOptionsMenu.Construct(_libretro);
             _coreOptionsButton.Construct(true, false, () => _coreOptionsMenu.SetVisible(true)); */
        }

        private void LibretronInstancePausedCallback()
        {
            _gameStopButton.SetInteractable(true);
            _gameStartButton.Text = "Resume Game";
            _gameStartButton.SetCallback(() =>
            {
                _libretro.ResumeContent();

            });
            _stateSave.SetInteractable(true);
            _gameRestartButton.SetInteractable(true);
        }

        private void LibretronInstanceStoppedCallback()
        {
            _gameStartButton.Text = "Start Game";
            _gameStartButton.SetCallback(() =>
            {
                _libretro.StartContent();
                LibretronInstancePausedCallback();
            });
            _gameStopButton.SetInteractable(false);
        }
        private void OnStateItemSelected(int selectedSlot)
        {
            Debug.Log($"Slot {selectedSlot} selected");
            _stateSlot = selectedSlot;
            _libretro.SaveState(selectedSlot);
        }
        private void OnStateLoadSelected(int selectedSlot)
        {
            /*  _stateSlot = selectedSlot;
               Debug.Log($"Slot {selectedSlot} selected");
               _libretro.LoadState(_stateSlot);  */
            //The code above is the old code and is replaced  by the code below. The code checks if Libretro is running and then adds a delay before the _librtro.LoadState otherwise if running it resumes the game.

            if (!_libretro.Current.Running)
            {
                _libretro.StartContent(); 
            }
            else
            {
                _libretro.ResumeContent();
            }
            _stateSlot = selectedSlot;
            Invoke("LoadStateAfterWait", .5f); 
        }
        private void LoadStateAfterWait() 
        {
            _libretro.LoadState(_stateSlot);
        }
        private void OnstateLoadDefault()
        {
            _libretro.LoadDefaultState();
            _libretro.ResumeContent(); 
        }
        private void OnDisable()
        {
            _libretro.OnInstanceChanged -= LibretroInstanceChangedCallback;
            if (_libretro.Current)
            {
                _libretro.Current.OnInstanceStarted -= LibretronInstanceStartedCallback;
                _libretro.Current.OnInstancePaused += LibretronInstancePausedCallback;
                _libretro.Current.OnInstanceStopped += LibretronInstanceStoppedCallback;
            }
        }

        private void LibretroInstanceChangedCallback(LibretroInstance libretroInstance)
        {
            if (!libretroInstance)
                return;

            libretroInstance.OnInstanceStarted -= LibretronInstanceStartedCallback;
            libretroInstance.OnInstanceStarted -= LibretronInstanceStartedCallback;
            libretroInstance.OnInstancePaused  +=  LibretronInstancePausedCallback;
            libretroInstance.OnInstancePaused  += LibretronInstancePausedCallback;
            libretroInstance.OnInstanceStopped += LibretronInstanceStoppedCallback;
            libretroInstance.OnInstanceStopped -= LibretronInstanceStoppedCallback;
        }

        private void LibretronInstanceStartedCallback()
        {
            /*_diskButton.SetInteractable(_libretro.DiskHandlerEnabled);
           _memoryButton.SetInteractable(true);
           _coreOptionsButton.SetInteractable(true); */
        }
    }
}

