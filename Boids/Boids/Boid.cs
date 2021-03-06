﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boids
{
    class Boid
    {
        Texture2D tex;
        List<Boid> friends;
        float speed = 70;
        float friendDistance = 60;
        float comfortDistance = 15f;
        float obstacleSafeDistance = 30f;
        float collisionTimer = 0;

        //Directions
        public Vector2 direction;
        public Vector2 allignVector;
        public Vector2 coheseVector;
        public Vector2 antiCrowdVector;
        public Vector2 avoidObstacleVector;

        //Weights
        float allignmentWeight = 2.0f;
        float coheseWeight = 0.5f;
        float antiCrowdWeight = 1.5f;
        float avoidObstacleWeight = 3.9f;

        public Vector2 pos;

        public Boid(Texture2D tex, Vector2 pos)
        {
            this.tex = tex;
            this.pos = pos;
            this.coheseVector = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.allignVector = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.direction = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.antiCrowdVector = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.friends = new List<Boid>();         
        }
        public void Update(GameTime time)
        {
            FindFriends();
            SetAverageAngle();
            Cohesion();
            AntiCrowding();
            AvoidObstacles();
            calcDirection();
            BoarderSwap();
            UpdateCollisionTimer(time);
            this.pos += direction * (float)time.ElapsedGameTime.TotalSeconds * speed;
            friends.Clear();
        }

        private void UpdateCollisionTimer(GameTime time)
        {
            if (collisionTimer > 0)
            {
                collisionTimer -= (float)time.ElapsedGameTime.TotalSeconds;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(tex, getHitBox(), null,  Color.White, (float)Math.Atan2(direction.Y , direction.X), new Vector2(tex.Bounds.Width / 2, tex.Bounds.Height / 2), SpriteEffects.None, 1);
        }
        public Rectangle getHitBox()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, 10, 10);
        }
        /// <summary>
        /// Timer makes sure that we dont get stuck
        /// in a loop of chaning direction after a 
        /// collision.
        /// </summary>
        void WallCollision()
        {
            if (pos.X + 25 > Game1.windowBounds.X || pos.X < 25)
            {
                direction.X *= -1;
                collisionTimer += 1f;
            }
            if (pos.Y + 25 > Game1.windowBounds.Y || pos.Y < 25)
            {
                direction.Y *= -1;
                collisionTimer += 1f;
            }
        }
        /// <summary>
        /// Comes in on one side, out the other
        /// </summary>
        void BoarderSwap()
        {
            if (pos.X > Game1.windowBounds.X)
            {
                pos.X = 0;
            }
            if (pos.Y + 25 > Game1.windowBounds.Y)
            {
                pos.Y = 0;
            }
            if (pos.X < 0)
            {
                pos.X = Game1.windowBounds.X;
            }
            if (pos.Y < 0)
            {
                pos.Y = Game1.windowBounds.Y;
            }
        }
        /// <summary>
        /// Fills the list of friends
        /// Updates every frame...
        /// </summary>
        void FindFriends()
        {
            foreach (Boid friend in SteeringBehaviourManager.boids)
            {
                if (friend == this)
                {
                    continue;
                }
                if (Vector2.Distance(friend.pos, this.pos) < friendDistance)
                {
                    friends.Add(friend);
                }
            }
        }
        /// <summary>
        /// Sets the avarge angle of
        /// all of the boids friends
        /// </summary>
        void SetAverageAngle()
        {
            if (collisionTimer <= 0.1f)
            {
                List<Vector2> directionsOfFriends = new List<Vector2>();
                foreach (Boid s in friends)
                {
                    directionsOfFriends.Add(s.direction);
                }
                foreach (Vector2 v in directionsOfFriends)
                {
                    this.allignVector.X += v.X;
                    this.allignVector.Y += v.Y;
                }
                allignVector.X = allignVector.X / (directionsOfFriends.Count + 1); //+1 because we dont want to miss our own vec and to not / by 0
                allignVector.Y = allignVector.Y / (directionsOfFriends.Count + 1);
                allignVector.Normalize();
            }
        }
        /// <summary>
        /// Steer towards the avarage point
        /// of friends
        /// </summary>
        void Cohesion()
        {
            if (collisionTimer <= 0.1f)
            {
                Vector2 avaragePos = this.pos;
                foreach (Boid friend in friends)
                {
                    avaragePos.X += friend.pos.X;
                    avaragePos.Y += friend.pos.Y;
                }
                if (friends.Count > 0)
                {
                    avaragePos.X = avaragePos.X / (friends.Count + 1);
                    avaragePos.Y = avaragePos.Y / (friends.Count + 1);
                    coheseVector = Vector2.Normalize(avaragePos - pos);
                }
                coheseVector.Normalize();
            }         
        }
        void AntiCrowding()
        {
            if (collisionTimer <= 0.1f)
            {
                int counter = 0;
                antiCrowdVector = new Vector2(0, 0);
                foreach (Boid friend in friends)
                {
                    if (Vector2.Distance(friend.pos, this.pos) < comfortDistance)
                    {
                        counter++;
                        antiCrowdVector.X = Vector2.Normalize(this.pos - friend.pos).X;
                        antiCrowdVector.Y = Vector2.Normalize(this.pos - friend.pos).Y;
                    }
                }
                if (counter != 0)
                {
                    antiCrowdVector.X /= counter;
                    antiCrowdVector.Y /= counter;
                    antiCrowdVector.Normalize();
                }
            }       
        }
        void AvoidObstacles()
        {
            if (collisionTimer <= 0.1f)
            {
                int counter = 0;
                avoidObstacleVector = new Vector2(0, 0);
                foreach (Obstacle obs in SteeringBehaviourManager.obstacles)
                {
                    if (Vector2.Distance(obs.pos, this.pos) < obstacleSafeDistance)
                    {
                        counter++;
                        avoidObstacleVector.X = Vector2.Normalize(this.pos - obs.pos).X;
                        avoidObstacleVector.Y = Vector2.Normalize(this.pos - obs.pos).Y;
                    }
                }
                if (Vector2.Distance(SteeringBehaviourManager.predetor.pos, this.pos) < obstacleSafeDistance)
                {
                    counter++;
                    avoidObstacleVector.X = Vector2.Normalize(this.pos - SteeringBehaviourManager.predetor.pos).X;
                    avoidObstacleVector.Y = Vector2.Normalize(this.pos - SteeringBehaviourManager.predetor.pos).Y;
                }
                if (counter != 0)
                {
                    avoidObstacleVector.X /= counter;
                    avoidObstacleVector.Y /= counter;
                    avoidObstacleVector.Normalize();
                }
            }     
        }
        void calcDirection()
        {
            if (collisionTimer <= 0.1f)
            {
                this.direction.X = (antiCrowdVector.X * antiCrowdWeight) + (allignVector.X * allignmentWeight) + (coheseVector.X * coheseWeight) + (avoidObstacleVector.X * avoidObstacleWeight);
                this.direction.Y = (antiCrowdVector.Y * antiCrowdWeight) + (allignVector.Y * allignmentWeight) + (coheseVector.Y * coheseWeight) + (avoidObstacleVector.Y * avoidObstacleWeight);
            }
        }
    }
}
