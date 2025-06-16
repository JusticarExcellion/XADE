using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameLabel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Label;
    [SerializeField]
    private Image Banner;

    public void
    SetBackground( Color color )
    {
        Banner.color = color;
    }

    public void
    SetText( string text )
    {
        string LabelText =  "Name: " + text;
        Label.text = LabelText;
    }
}
