using UnityEngine;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Framework; // ConditionBase

[Condition("MyConditions/HasTarget")]
public class HasTargetCondition : ConditionBase
{
    [InParam("Target Script")]
    public Complete.TankMovement tankMovement;

    public override bool Check()
    {
        if(tankMovement.shootScript.target == null || tankMovement.needRecharge)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
