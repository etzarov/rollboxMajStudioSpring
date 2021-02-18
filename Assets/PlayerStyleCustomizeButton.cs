using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStyleCustomizeButton : MonoBehaviour
{
    public PlayerStyle styleDisplay;
    public int starsToUnlock;
    [Space]
    public BoxCollider hitbox;

    private void Start()
    {
        UpdateStyleCustomization();
    }

    private void Update()
    {
        Vector3 tPos = Vector3.zero;
        bool pressed = false;
        ExtensionMethods.DetectTouches(out pressed, out tPos);
        if (ExtensionMethods.TouchedHitbox(hitbox,tPos))
        {
            print("set");
            SFXManager.main.PlaySelectSound(SFXManager.SelectSound.HardClick,.5f);
            MenuManager.main.SetPlayerStyle(styleDisplay);
        }
    }

    public void UpdateStyleCustomization()
    {
        PlayerStyleManager psm = GetComponent<PlayerStyleManager>();
        psm.UpdateStyle(styleDisplay);
    }
}
