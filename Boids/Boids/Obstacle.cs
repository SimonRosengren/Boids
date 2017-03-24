using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boids
{
    class Obstacle
    {
        Texture2D tex;
        public Vector2 pos;

        public Obstacle(Vector2 pos, Texture2D tex)
        {
            this.pos = pos;
            this.tex = tex;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(tex, GetHitBox(), Color.White);
        }

        public Rectangle GetHitBox()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, 15, 15);
        }
    }
}
