using UnityEngine;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Framework; // ConditionBase

[Condition("MyConditions/HasNoAmmo")]
public class HasAmmoCondition : ConditionBase
{
    [InParam("Target Script")]
    public Complete.TankMovement tankMovement;

    public override bool Check()
    {
        if(!tankMovement.needRecharge)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
