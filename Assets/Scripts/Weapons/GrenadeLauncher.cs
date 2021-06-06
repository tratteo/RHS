// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GrenadeLauncher.cs
//
// All Rights Reserved

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