using UnityEngine;
using UnityEngine.UI;

public class RoomScene : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] newSprite;
    public static int img = 0;

    private void Start()
    {
        ChooseImage();
    }

    public void ChooseImage()
    {
        targetImage.sprite = newSprite[img];
    }
}
