using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIOptionPicker : MonoBehaviour
{
    [SerializeField] private Text optionText = default;

    public int CurrentIndex { get; private set; }

    private string[] options = new string[0];

    public void Setup(Type enumOptions, int startIndex)
    {
        string[] options = Enum.GetNames(enumOptions);

        Setup(options, startIndex);
    }

    public void Setup(string[] options, int startIndex)
    {
        CurrentIndex = startIndex;

        this.options = options;

        UpdateVisuals();
    }

    public void PreviousOnClick()
    {
        CurrentIndex--;

        if (CurrentIndex < 0)
            CurrentIndex = options.Length - 1;

        UpdateVisuals();
    }

    public void NextOnClick()
    {
        CurrentIndex++;

        if (CurrentIndex >= options.Length)
            CurrentIndex = 0;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        optionText.text = options[CurrentIndex];
    }
}
