using System;
using Discord.WebSocket;

namespace draftbot
{
    class DescWeapon : Command
    {

        public string Execute(Database database, SocketUser user, string state, string param)
        {
            if (param == null)
            {
                return "Please write a weapon name you wish described.";
            }

            LocalMemoryWrapper localMemory = new LocalMemoryWrapper(database);
            Weapon foundWeapon = new Weapon();
            WeaponAlias foundAlias = new WeaponAlias();

            if (localMemory.allWeaponsDictionary.TryGetValue(param, out foundWeapon))
            {
                return BuildString(param, localMemory);
            }
            else
            {
                if (localMemory.allWeaponAliasesDictionary.TryGetValue(param, out foundAlias))
                {
                    if (localMemory.allWeaponsDictionary.TryGetValue(localMemory.allWeaponAliasesDictionary[param].weaponName, out foundWeapon))
                    {
                        return BuildString(localMemory.allWeaponAliasesDictionary[param].weaponName, localMemory);
                    }
                }
            }

            return "Could not find the weapon. Try another phrase or write the weapon name fully.";
        }

        public string ShortDescription()
        {
            return "Describes a weapon's description.";
        }

        public string BuildString(string weaponName, LocalMemoryWrapper localMemory)
        {
            Weapon weapon = localMemory.allWeaponsDictionary[weaponName];

            String outString = "";
            outString += weapon.name.ToUpper() + "\n";
            outString += weapon.description + "\n\n";
            outString += "Slot: " + weapon.slot + "\n";
            outString += NullOrString("Cost: $", weapon.cost);
            outString += NullOrString("Damage: ", weapon.damage);
            outString += NullOrString("Pellets: ", weapon.pellets);
            outString += NullOrString("RPM: ", weapon.rpm);
            outString += NullOrString("Splash Size: ", weapon.splash);
            outString += NullOrString("Move Speed: ", weapon.moveSpeed, "%");
            outString += NullOrString("Combat Speed: ", weapon.combatSpeed, "%");
            outString += NullOrString("Mag Size: ", weapon.magSize);
            outString += NullOrString("Total Magazines: ", weapon.magAmount);
            outString += NullOrString("Reload Time: ", weapon.reloadTime);
            outString += NullOrString("Bullet Speed: ", weapon.bulletSpeed);
            outString += NullOrString("Range: ", weapon.range);
            outString += NullOrString("Base Recoil or Spread: ", weapon.baseRecoil);
            outString += NullOrString("Recoil Force: ", weapon.recoilForce);
            outString += NullOrString("Recoil Pivot: ", weapon.recoilPivot);
            outString += NullOrString("Movement Recoil: ", weapon.movementRecoil);

            return outString;
        }

        public string NullOrString(string prefixString, object obj)
        {
            return NullOrString(prefixString, obj, "");
        }

        public string NullOrString(string prefixString, object obj, string suffixString)
        {
            if (obj != null)
            {
                return prefixString + obj.ToString() + suffixString + "\n";
            }
            else
            {
                return "";
            }
        }

    }

}