using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace draftbot
{
    partial class Database
    {
        public HashSet<Weapon> GetAllWeapons()
        {
            //should only be called once per program lifespan and cached
            HashSet<Weapon> allWeaponsSet = new HashSet<Weapon>();

            SQLiteCommand getAllWeaponsCommand = _dbConnection.CreateCommand(); 

            getAllWeaponsCommand.CommandText = "SELECT Name, Description, Slot, Cost, Damage, Pellets, RPM, Splash, MoveSpeed, CombatSpeed, MagSize, MagAmount, ReloadTime, BulletSpeed, Range, BaseRecoil, RecoilForce, RecoilPivot, MovementRecoil FROM Weapon";
            using (SQLiteDataReader reader = getAllWeaponsCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Weapon currentlyReadWeapon = new Weapon();

                        currentlyReadWeapon.name = reader["Name"].ToString();
                        currentlyReadWeapon.description = reader["Description"].ToString();
                        currentlyReadWeapon.slot = reader["Slot"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("Cost")))
                        {
                            currentlyReadWeapon.cost = UInt32.Parse(reader["Cost"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Damage")))
                        {
                            currentlyReadWeapon.damage = Decimal.Parse(reader["Damage"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Pellets")))
                        {
                            currentlyReadWeapon.pellets = UInt32.Parse(reader["Pellets"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("RPM")))
                        {
                            currentlyReadWeapon.rpm = UInt32.Parse(reader["RPM"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Splash")))
                        {
                            currentlyReadWeapon.splash = Decimal.Parse(reader["Splash"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("MoveSpeed")))
                        {
                            currentlyReadWeapon.moveSpeed = Decimal.Parse(reader["MoveSpeed"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("CombatSpeed")))
                        {
                            currentlyReadWeapon.combatSpeed = Decimal.Parse(reader["CombatSpeed"].ToString());
                        }                        
                        if (!reader.IsDBNull(reader.GetOrdinal("MagSize")))
                        {
                            currentlyReadWeapon.magSize = UInt32.Parse(reader["MagSize"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("MagAmount")))
                        {
                            currentlyReadWeapon.magAmount = UInt32.Parse(reader["MagAmount"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("ReloadTime")))
                        {
                            currentlyReadWeapon.reloadTime = Decimal.Parse(reader["ReloadTime"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("BulletSpeed")))
                        {
                            currentlyReadWeapon.bulletSpeed = Decimal.Parse(reader["BulletSpeed"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Range")))
                        {
                            currentlyReadWeapon.range = Decimal.Parse(reader["Range"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("BaseRecoil")))
                        {
                            currentlyReadWeapon.baseRecoil = Decimal.Parse(reader["BaseRecoil"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("RecoilForce")))
                        {
                            currentlyReadWeapon.recoilForce = Decimal.Parse(reader["RecoilForce"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("RecoilPivot")))
                        {
                            currentlyReadWeapon.recoilPivot = UInt32.Parse(reader["RecoilPivot"].ToString());
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("MovementRecoil")))
                        {
                            currentlyReadWeapon.movementRecoil = UInt32.Parse(reader["MovementRecoil"].ToString());
                        }

                        allWeaponsSet.Add(currentlyReadWeapon);
                    }
                }

            }

            if (allWeaponsSet.Count == 0)
            {
                throw new Exception("There should be weapons in the database.");
            }

            return allWeaponsSet;
        }

        public HashSet<WeaponAlias> GetAllWeaponAliases()
        {
            //should only be called once per program lifespan and cached
            HashSet<WeaponAlias> allWeaponAliasSet = new HashSet<WeaponAlias>();

            SQLiteCommand getAllWeaponAliasesCommand = _dbConnection.CreateCommand();

            getAllWeaponAliasesCommand.CommandText = "SELECT Alias, Weapon FROM WeaponAlias";

            using (SQLiteDataReader reader = getAllWeaponAliasesCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        WeaponAlias currentlyReadWeaponAlias = new WeaponAlias();
                        currentlyReadWeaponAlias.aliasName = reader["Alias"].ToString();
                        currentlyReadWeaponAlias.weaponName = reader["Weapon"].ToString();
                        allWeaponAliasSet.Add(currentlyReadWeaponAlias);
                    }
                }

            }

            return allWeaponAliasSet;
        }

    }

}