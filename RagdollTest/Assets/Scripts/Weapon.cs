using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon :  ScriptableObject
{
    public int AmmoCapacity = 100;
    public bool HasUnlimitedAmmo = true;
    public int Range = 5;
    public float Force = 20;
    private int _ammo = 0;
    public string Name = "Default";
	public GameObject weapon3D;
    public int Ammo
    {
        get
        {
            return _ammo;
        }
    }
    public void LoadAmmo()
    {
        _ammo = AmmoCapacity;
    }
    public void Fire()
    {
        if (!HasUnlimitedAmmo)
        {
            --_ammo;
        }
    }
}
