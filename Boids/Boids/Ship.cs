using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boids
{
    class Ship
    {

        Vector2 pos;
        Texture2D tex;
        float speed = 50;
        float friendDistance = 60;
        List<Ship> friends;
        float collisionTimer = 0;
        //Weights
        float averageAngleWeight = 0.5f;

        public Vector2 velocity;

        public Ship(Texture2D tex, Vector2 pos)
        {
            this.tex = tex;
            this.pos = pos;
            this.velocity = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            friends = new List<Ship>();
            
        }
        public void Update(GameTime time)
        {
            WallCollision();
            velocity.Normalize();
            FindFriends();
            SetAverageAngle();
            UpdateCollisionTimer(time);
            this.pos += velocity * (float)time.ElapsedGameTime.TotalSeconds * speed;
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
            sb.Draw(tex, getHitBox(), null,  Color.White, (float)Math.Atan2(velocity.Y , velocity.X), new Vector2(0, 0), SpriteEffects.None, 1);
        }
        public Rectangle getHitBox()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, 20, 20);
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
                velocity.X *= -1;
                collisionTimer += 1f;
            }
            if (pos.Y + 25 > Game1.windowBounds.Y || pos.Y < 25)
            {
                velocity.Y *= -1;
                collisionTimer += 1f;
            }
        }
        /// <summary>
        /// Fills the list of friends
        /// Updates every frame...
        /// </summary>
        void FindFriends()
        {
            foreach (Ship friend in SteeringBehaviourManager.ships)
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
                foreach (Ship s in friends)
                {
                    directionsOfFriends.Add(s.velocity);
                }
                foreach (Vector2 v in directionsOfFriends)
                {
                    this.velocity.X += v.X;
                    this.velocity.Y += v.Y;
                }
                velocity.X = velocity.X / (directionsOfFriends.Count + 1); //+1 because we dont want to miss our own vec
                velocity.Y = velocity.Y / (directionsOfFriends.Count + 1);
            }
        }
        void Flock()
        {

        }
    }
}
