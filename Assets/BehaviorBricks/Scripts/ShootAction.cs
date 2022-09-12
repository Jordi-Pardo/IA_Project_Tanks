using UnityEngine;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework; // BasePrimitiveAction

[Action("MyActions/ShootAction")]
public class ShootAction : BasePrimitiveAction
{

    [InParam("Tank movement script")]
    public Complete.TankMovement tankMovement;

    public override void OnStart()
    {
        tankMovement.StopMove();
    }
}