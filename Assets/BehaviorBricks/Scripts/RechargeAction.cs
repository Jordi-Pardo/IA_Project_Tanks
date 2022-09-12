using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework; // BasePrimitiveAction

[Action("MyActions/RechargeAction")]
public class RechargeAction: BasePrimitiveAction
{
    [InParam("Tank movement script")]
    public Complete.TankMovement tankMovement;

    [InParam("Ammo UI")]
    public AmmoUIController ammoUIController;

    [InParam("Agent")]
    public NavMeshAgent agent;

    float currentTime = 0f;
    bool startedRecharge = false;

    public override void OnStart()
    {

        tankMovement.SetRechargeDestination();
        tankMovement.CanMove();
        tankMovement.QuitTarget();
        Debug.DrawLine(agent.transform.position, agent.destination, Color.red, 3f);
    }

    public override TaskStatus OnUpdate()
    {

        if(agent.remainingDistance <= 0.5f)
        {
            if (!startedRecharge)
            {
                ammoUIController.StartRechargeCO();
                startedRecharge = true;

            }
          

            if(currentTime <= tankMovement.shootScript.timeToRecharge)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime -= tankMovement.shootScript.timeToRecharge;
                tankMovement.needRecharge = false;
                tankMovement.shootScript.RechargeAmmo();
                Debug.Log("Recharged");
                startedRecharge = false;
            }
        }
        return TaskStatus.RUNNING;
    }


    


}
