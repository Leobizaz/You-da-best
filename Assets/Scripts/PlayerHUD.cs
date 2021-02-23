using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerHUD : MonoBehaviour
{
    public Image hpBarFILL;

    public void SetHPBarValue(float v)
    {
        DOVirtual.Float(hpBarFILL.fillAmount, v, 0.5f, UpdateBar);
    }

    private void UpdateBar(float v)
    {
        hpBarFILL.fillAmount = v;
    }

}
