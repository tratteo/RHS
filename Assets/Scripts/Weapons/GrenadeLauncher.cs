public class GrenadeLauncher : Shooter
{
    public void TriggerShoot(float force)
    {
        Projectile projectile = TriggerShoot();
        if (projectile is Grenade grenade)
        {
            grenade.Launch(force);
        }
    }
}