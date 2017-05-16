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
        public static List<Boid> boids = new List<Boid>();
        public static List<Obstacle> obstacles = new List<Obstacle>();
        public static Predetor predetor;

        Texture2D predTex;

        public SteeringBehaviourManager(Texture2D predTex)
        {
            this.predTex = predTex;
            predetor = new Predetor(new Vector2(50, 50), predTex);
        }
        public void Update(GameTime time)
        {
            foreach (Boid s in boids)
            {
                s.Update(time);
            }
            predetor.Update(time);
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (Boid s in boids)
            {
                s.Draw(sb);
            }
            foreach (Obstacle o in obstacles)
            {
                o.Draw(sb);
            }
            predetor.Draw(sb);
        }
        public void AddShip(Texture2D tex, Vector2 pos)
        {
            boids.Add(new Boid(tex, pos));
        }
        public void AddObstacle(Texture2D tex, Vector2 pos)
        {
            obstacles.Add(new Obstacle(pos, tex));
        }
    }
}
