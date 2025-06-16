using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class Banner : MonoBehaviour
{
    [SerializeField]
    private Image Panel;
    [SerializeField]
    private TMP_Text Label;
    [SerializeField]
    private Color StartingPanelColor;

    private void
    Awake()
    {
        StartingPanelColor = Panel.color;
    }

    public IEnumerator
    DisplayBannerText( String Text )
    {
        Label.text = Text;
        yield return new WaitForSeconds( 5f );
        Destroy( this.gameObject );
        Panel.color = StartingPanelColor;
    }

    public void
    SetBannerColor( Color BannerColor )
    {
        Panel.color = BannerColor;
    }
}
