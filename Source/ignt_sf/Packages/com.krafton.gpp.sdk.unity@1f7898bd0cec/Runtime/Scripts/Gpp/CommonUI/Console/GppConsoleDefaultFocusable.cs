using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GppConsoleDefaultFocusable : MonoBehaviour, IGppConsoleUIFocusable
{
    private Action defaultActionFocus;
    private Action defaultActionDefocus;
    private Action defaultActionA;
    private Action defaultActionB;
    private Action defaultActionX;
    private Action defaultActionY;

    public void SetDefaultActionFocus(Action action) => defaultActionFocus = action;
    public void SetDefaultActionDefocus(Action action) => defaultActionDefocus = action;
    public void SetDefaultActionA(Action action) => defaultActionA = action;
    public void SetDefaultActionB(Action action) => defaultActionB = action;
    public void SetDefaultActionX(Action action) => defaultActionX = action;
    public void SetDefaultActionY(Action action) => defaultActionY = action;

    public void OnFocus()
    {
        defaultActionFocus?.Invoke();
    }
    public void OnDefocus()
    {
        defaultActionDefocus?.Invoke();
    }
    public void OnAPressed()
    {
        defaultActionA?.Invoke();
    }    public void OnBPressed()
    {
        defaultActionB?.Invoke();
    }
    public void OnXPressed()
    {
        defaultActionX?.Invoke();
    }
    public void OnYPressed()
    {
        defaultActionY?.Invoke();
    }
}
