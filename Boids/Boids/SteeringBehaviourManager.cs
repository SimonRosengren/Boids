using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boids
{
    class SteeringBehaviourManager
    {
        public static List<Ship> ships = new List<Ship>(); 

        public void Update(GameTime time)
        {
            foreach (Ship s in ships)
            {
                s.Update(time);
            }
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (Ship s in ships)
            {
                s.Draw(sb);
            }
        }
        public void AddShip(Texture2D tex, Vector2 pos)
        {
            ships.Add(new Ship(tex, pos));
        }
    }
}
