// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Global.cs
//
// All Rights Reserved

public static class Layers
{
    public static readonly string ENEMY_PROJECTILES = "EnemyProjectiles";
    public static readonly string PROJECTILES = "Projectiles";
    public static readonly string FRIENDLIES = "Friendlies";
    public static readonly string HOSTILES = "Hostiles";
    public static readonly string ENVIROMENT = "Enviroment";
    public static readonly string WEAPONS = "Weapons";
}

public static class Categories
{
    public static readonly string PROJECTILES = "Projectiles";
}

public static class UIPrefs
{
    public static string EquipmentTitle(string baseName) => baseName;

    public static string Parameter(object parameter) => "<color=green>" + parameter.ToString() + "</color>";

    public static string TooltipTitle(string baseName) => "<size=70>" + baseName + "</size>";
}