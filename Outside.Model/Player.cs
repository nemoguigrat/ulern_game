﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace UlernGame.Model
{
    public class Player : GameObject
    {
        private const int maxAmmunition = 50;
        private const int fullMagazine = 10;
        private const int maxHeals = 100;
        public const int speed = 5;
        public const int damage = 3;
        public int Ammunition { get; private set; }
        public int Magazine { get; private set; }
        public Directions Direction { get; private set; }

        public int Damage { get; }
        public int Heals { get; private set; }
        
        public Player(int x, int y)
        {
            Heals = maxHeals;
            X = x;
            Y = y;
            Magazine = fullMagazine;
            Ammunition = maxAmmunition;
            Damage = 10;
            Width = 50;
            Height = 50;
        }

        public void Reload()
        {
            var reload = fullMagazine - Magazine;
            if (reload > 0 && Ammunition - reload >= 0)
                Ammunition -= reload;
            Magazine += reload;
        }

        public void ReserveDamage(int damage)
        {
            Heals -= damage;
            if (Heals <= 0)
                throw new Exception();
        }

        public bool Shoot()
        {
            if (Magazine <= 0) return false;
            Magazine--;
            return true;
        }

        public void Heal()
        {
            Heals += Medkit.healCount;
        }

        public void AmmoAdd()
        {
            if (Ammunition + AmmunitionCrate.ammoCount <= maxAmmunition)
                Ammunition += AmmunitionCrate.ammoCount;
            else
                Ammunition = maxAmmunition;
        }

        public void Move(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public void SwitchDirection(Directions dir)
        {
            Direction = dir;
        }
    }
}