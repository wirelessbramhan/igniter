using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gpp.CommonUI.Console
{
    public class GppConsoleUIInput : MonoBehaviour
    {
        private List<MonoBehaviour> focusables = new();
        private int currentIdx = 0;

        private float stickThreshold = 0.5f;
        private float lastStickY = 0f;

        private Action defaultActionA;
        private Action defaultActionB;
        private Action defaultActionX;
        private Action defaultActionY;

        public void Init(List<MonoBehaviour> focusables, int idx = 0)
        {
            this.focusables = focusables ?? new List<MonoBehaviour>();
            currentIdx = Mathf.Clamp(idx, 0, this.focusables.Count - 1);
            SetFocus(currentIdx);
        }

        public void AddFocusable(MonoBehaviour focusable)
        {
            if (focusable != null && !focusables.Contains(focusable))
                focusables.Add(focusable);
        }

        public void RemoveFocusable(MonoBehaviour focusable)
        {
            focusables.Remove(focusable);
        }

        public void MoveNext()
        {
            MoveFocus(1);
        }

        public void SetDefaultActionA(Action action) => defaultActionA = action;
        public void SetDefaultActionB(Action action) => defaultActionB = action;
        public void SetDefaultActionX(Action action) => defaultActionX = action;
        public void SetDefaultActionY(Action action) => defaultActionY = action;

        private void Update()
        {
            if (Gamepad.current == null)
                return;

            float currentStickY = Gamepad.current.leftStick.ReadValue().y;
            if (currentStickY > stickThreshold && lastStickY <= stickThreshold)
            {
                MoveFocus(-1); // ����
            }
            else if (currentStickY < -stickThreshold && lastStickY >= -stickThreshold)
            {
                MoveFocus(1); // �Ʒ���
            }

            lastStickY = currentStickY;

            if (Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                MoveFocus(-1);
            }
            else if (Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                MoveFocus(1);
            }

            if (Gamepad.current.buttonSouth.wasPressedThisFrame) // A
            {
                defaultActionA?.Invoke();
                GetCurrent()?.OnAPressed();
            }
            if (Gamepad.current.buttonEast.wasPressedThisFrame) // B
            {
                defaultActionB?.Invoke();
                GetCurrent()?.OnBPressed();
            }
            if (Gamepad.current.buttonWest.wasPressedThisFrame) // X
            {
                defaultActionX?.Invoke();
                GetCurrent()?.OnXPressed();
            }
            if (Gamepad.current.buttonNorth.wasPressedThisFrame) // Y
            {
                defaultActionY?.Invoke();
                GetCurrent()?.OnYPressed();
            }
        }

        private void MoveFocus(int direction)
        {
            if (focusables is null)
                return;

            GetCurrent()?.OnDefocus();

            currentIdx = (currentIdx + direction + focusables.Count) % focusables.Count;
            SetFocus(currentIdx);
        }

        private void SetFocus(int index)
        {
            if (focusables is null)
                return;

            GetCurrent()?.OnFocus();
        }

        private IGppConsoleUIFocusable GetCurrent()
        {
            if (focusables is null)
                return null;

            if (focusables.Count == 0 || currentIdx < 0 || currentIdx >= focusables.Count)
                return null;

            return focusables[currentIdx] as IGppConsoleUIFocusable;
        }
    }
}