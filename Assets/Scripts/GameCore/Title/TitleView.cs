using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;
    public Button nameChangeButton;
    public TextMeshProUGUI nameText;
    public Image iconImage;

    public void Initialize()
    {
        nameText.maxVisibleCharacters = GameCommonData.MaxNameCount;
    }
}