﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using UlernGame.Model;
using UlernGame.View;

namespace UlernGame
{
    public partial class MyForm : Form
    {
        public readonly Game game;
        public readonly Sprites sprites = new Sprites();
        public List<IEnumerable<List<Point>>> pathsToPlayer;
        public MyForm()
        {
            DoubleBuffered = true;
            ClientSize = new Size(1280, 720);
            game = new Game();
            game.StartGame();
            AddControls();
            SetTimer(20, TimerTick); //основной игровой таймер
            SetTimer(500, DamageTimerTick); //таймер урона по монстрам 
            SetTimer(4000, () => game.SpawnMonsters());//таймер спавна монстров 
            SetTimer(1000, MonsterMovement); // обновление маршрута и движение монстра
        }

        private void AddControls()
        {
            var ammunition = new Label {Location = new Point(20, 70), AutoSize = true, BackColor = Color.Transparent};
            Controls.Add(ammunition);
        }
        private void DamageTimerTick()
        {
            if (game.CheckCollisionWithEnemy())
                game.Player.ReserveDamage(Monster.damage);
        }

        private void MonsterMovement()
        {
            game.Monsters.ForEach(x => x.MoveTo());
        }
        private void TimerTick()
        {
            game.Bullets.ForEach(x => x.Move());
            if (game.Bullets.Count > 0)
                game.BulletCollision();
            game.CheckCollisionWithBoosters();
            Controls[0].Text = $"{game.Player.Magazine} / {game.Player.Ammunition}";
            Controls[0].Font = new Font(FontFamily.GenericMonospace, 20);
            Invalidate();
        }

        private void SetTimer(int interval, Action func)
        {
            var timer = new Timer();
            timer.Interval = interval;
            timer.Tick += (sender, args) => func();
            timer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs key)
        {
            game.PlayerMoveDirection(key.KeyData);
        }

        protected override void OnKeyUp(KeyEventArgs key)
        {
            game.PlayerAction(key.KeyData);
        }

        protected override void OnPaint(PaintEventArgs g)
        {
            var graphic = g.Graphics;
            DrawMap(graphic);
            DrawPlayer(graphic);
            DrawMonster(graphic);
            graphic.FillRectangle(Brushes.PaleGreen, 
                new Rectangle(20, 20, game.Player.Heals * 5, 20));
            DrawBullets(graphic);
            DrawBoosters(graphic);
        }
        private void DrawMap(Graphics gr)
        {
            foreach (var e in game.Map.Objects)
            {
                if (e is Wall)
                    gr.DrawImage(sprites.Wall, e.X, e.Y);
            }
        }

        private void DrawBoosters(Graphics gr)
        {
            for (var i = 0; i < game.Boosters.Count; i++)
            {
                if (game.Boosters[i] is Medkit)
                    gr.DrawImage(sprites.Medkit, game.Boosters[i].X, game.Boosters[i].Y);
                if (game.Boosters[i] is AmmunitionCrate)
                    gr.DrawImage(sprites.AmmunitionCrate, game.Boosters[i].X, game.Boosters[i].Y);
            }
                
        }
        private void DrawPlayer(Graphics gr)
        {
            var a = game.Player.Direction;
            gr.DrawImage(sprites.Player[a.ToString()], game.Player.X, game.Player.Y);
            // gr.DrawRectangle(Pens.Chartreuse, game.Player.X, game.Player.Y, game.Player.Hitbox.Width, game.Player.Hitbox.Height);
        }

        private void DrawMonster(Graphics gr)
        {
            for (var i = 0; i < game.Monsters.Count; i++)
                gr.DrawImage(sprites.Monster[game.Monsters[i].Direction.ToString()], game.Monsters[i].X, game.Monsters[i].Y);
            
        }
        private void DrawBullets(Graphics g)
        {
            foreach (var bullet in game.Bullets)
            {
                g.FillEllipse(Brushes.Blue, bullet.X, bullet.Y, 6,6);
            }
        }
    }
}