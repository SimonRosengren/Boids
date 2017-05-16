using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boids
{
    class Predetor
    {
        Boid target;
        float speed = 200;
        Texture2D tex;
        Vector2 randomTargetPos = new Vector2(0, 0);
        float randomAngle = 0;
        float initialTargetDistane = 1;

        public enum SteeringBehaviour { Approach, Pursuit, Arrive, Wander}
        SteeringBehaviour currSteeringBehaviour = SteeringBehaviour.Arrive;

        public Vector2 pos;
        public Vector2 dir;

        public Predetor(Vector2 pos, Texture2D tex)
        {
            this.pos = pos;
            this.dir = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.tex = tex;
        }
        public void Update(GameTime time)
        {
            this.pos += dir * speed * (float)time.ElapsedGameTime.TotalSeconds;

            switch (currSteeringBehaviour)
            {
                case SteeringBehaviour.Approach:
                    Approach();
                    break;
                case SteeringBehaviour.Pursuit:
                    Pursuit();
                    break;
                case SteeringBehaviour.Arrive:
                    Arrive();
                    break;
                case SteeringBehaviour.Wander:
                    Wander();
                    break;
                default:
                    break;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(tex, GetHitBox(), null, Color.Black, (float)Math.Atan2(dir.Y, dir.X), new Vector2(tex.Bounds.Width / 2, tex.Bounds.Height / 2), SpriteEffects.None, 1);
        }
        public Rectangle GetHitBox()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, 25, 25);
        }
        /*Approaches the target at full speed*/
        void Approach()
        {
            //We dont want to look for new targets all the time
            if (target == null)
            {
                //Find the closest target
                Vector2 approachPos = new Vector2(0, 0);
                float closestDistance = 10000;

                foreach (Boid boid in SteeringBehaviourManager.boids)
                {
                    if (Vector2.Distance(this.pos, boid.pos) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(this.pos, boid.pos);
                        target = boid;
                    }
                }   
            }
            if (target != null)
            {
                dir = Vector2.Normalize(target.pos - this.pos);    
            }
        }
        /*Approaches the future location of target at full speed*/
        void Pursuit()
        {
            //We dont want to look for new targets all the time
            if (target == null)
            {
                //Find the closest target
                Vector2 approachPos = new Vector2(0, 0);
                float closestDistance = 10000;

                foreach (Boid boid in SteeringBehaviourManager.boids)
                {
                    if (Vector2.Distance(this.pos, boid.pos) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(this.pos, boid.pos);
                        target = boid;
                    }
                }
            }
            if (target != null)
            {
                dir = Vector2.Normalize((target.pos + target.direction * 30.5f) - this.pos);
            }
        }
        /*Approaches the target but slows down the closer it gets*/
        void Arrive()
        {
            //We dont want to look for new targets all the time
            if (target == null)
            {
                //Find the closest target
                Vector2 approachPos = new Vector2(0, 0);
                float closestDistance = 10000;

                foreach (Boid boid in SteeringBehaviourManager.boids)
                {
                    if (Vector2.Distance(this.pos, boid.pos) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(this.pos, boid.pos);
                        target = boid;
                        initialTargetDistane = closestDistance; 
                    }
                }
            }
            if (target != null)
            {
                dir = Vector2.Normalize((target.pos + target.direction * 30.5f) - this.pos) * (Vector2.Distance(this.pos, target.pos) / initialTargetDistane);               
            }
        }
        /*Seemingly wandering randomly. Actually following a target travelling at radnom direction on a large circle*/
        void Wander()
        {
            //Find a random point on a circle
            float circleRadius = 300;
            randomAngle += Game1.rnd.Next(-10, 11) * 0.01f;

            randomTargetPos.X = 400 + circleRadius * (float)Math.Cos(randomAngle);
            randomTargetPos.Y = 250 + circleRadius * (float)Math.Sin(randomAngle);

            //Move towards that point
            this.dir = Vector2.Normalize(randomTargetPos - this.pos);           
        }
    }
}
