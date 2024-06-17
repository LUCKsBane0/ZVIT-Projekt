using UnityEngine;
using UnityEngine.UI;

public class ProgressRing : MonoBehaviour
{
    private Image ringImage;

    void Start()
    {
        ringImage = GetComponent<Image>();
        ringImage.fillAmount = 0f;
    }

    public void UpdateRing(float progress)
    {
        ringImage.fillAmount = progress;
    }
}
