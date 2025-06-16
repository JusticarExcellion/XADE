using UnityEngine;
using TMPro;

public class Label_Distance : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Label;

    public void
    SetLabelText( string Text )
    {
        Label.text = Text;
    }
}
