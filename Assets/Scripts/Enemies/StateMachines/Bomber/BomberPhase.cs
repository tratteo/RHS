using GibFrame;
using UnityEngine;

public class BomberPhase : BossPhaseStateMachine
{
    [Header("Bomber")]
    [SerializeField] private RandomizedFloat shootUpdate;
    private float shootTimer;

    protected GrenadeLauncher Launcher { get; private set; } = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Weapon weapon = Owner.GetWeapon();
        if (weapon is GrenadeLauncher)
        {
            Launcher = weapon as GrenadeLauncher;
        }
        else
        {
            Debug.LogError("Bomber has no shooter weapon");
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!IsPerformingAbility && CanExecute())
        {
            if (shootTimer > 0)
            {
                shootTimer -= Time.deltaTime;
            }
            else
            {
                Launcher.TriggerShoot(Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position) * 0.75F);
                shootTimer = shootUpdate;
            }
        }
    }

    protected override float GetAttackRange() => 10F;

    protected override Vector3 GetMovementDirection()
    {
        Vector2 dir = (Owner.transform.position - Owner.TargetContext.Transform.position).normalized;
        if (Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position) > GetAttackRange())
        {
            return UnityEngine.Random.value < 0.2F ? Vector2.zero : -dir;
        }
        else
        {
            dir = UnityEngine.Random.value < 0.5F ? Quaternion.AngleAxis(90F, Vector3.forward) * dir : Quaternion.AngleAxis(-90F, Vector3.forward) * dir;
            Debug.DrawRay(Owner.transform.position, dir, Color.red, 0.5F);
            return dir;
        }
    }
}