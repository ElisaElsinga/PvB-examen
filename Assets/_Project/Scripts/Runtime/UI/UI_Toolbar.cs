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

using SK.Libretro.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UI_Toolbar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private LibretroInstanceVariable _libretro;

        public void Construct(bool visible, LibretroInstanceVariable libretro)
        {
            _libretro = libretro;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        //commented out by nacho
        //public void OnPointerEnter(PointerEventData eventData) => _libretro.SetInputEnabled(false);
        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        //commented out by nacho
        //public void OnPointerExit(PointerEventData eventData) => _libretro.SetInputEnabled(true);
        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}
