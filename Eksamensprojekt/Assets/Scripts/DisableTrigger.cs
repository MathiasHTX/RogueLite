using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrigger : MonoBehaviour
{
    [SerializeField] GameObject weaponController;

    public void CallDisableTrigger()
    {
        weaponController.GetComponent<WeaponController>().DisableTrigger();
    }

    public void CallEnableTrigger()
    {
        weaponController.GetComponent<WeaponController>().EnableTrigger();
    }
}
