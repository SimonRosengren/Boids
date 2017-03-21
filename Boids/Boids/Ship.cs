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
        //float averageAngleWeight = 0.5f;

        public Vector2 allignVector;
        public Vector2 coheseVector;

        public Vector2 direction;

        public Ship(Texture2D tex, Vector2 pos)
        {
            this.tex = tex;
            this.pos = pos;
            this.coheseVector = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.allignVector = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            this.direction = new Vector2((float)Game1.rnd.NextDouble(), (float)Game1.rnd.NextDouble());
            friends = new List<Ship>();         
        }
        public void Update(GameTime time)
        {
            FindFriends();
            SetAverageAngle();
            Cohesion();
            calcDirection();
            WallCollision();
            UpdateCollisionTimer(time);
            this.pos += direction * (float)time.ElapsedGameTime.TotalSeconds * speed;
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
            sb.Draw(tex, getHitBox(), null,  Color.White, (float)Math.Atan2(direction.Y , direction.X), new Vector2(0, 0), SpriteEffects.None, 1);
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
                foreach (Ship friend in friends)
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
        void calcDirection()
        {
            this.direction.X = (coheseVector.X + allignVector.X) / 2;
            this.direction.Y = (coheseVector.Y + allignVector.Y) / 2;
            //this.direction.Normalize();
        }
    }
}
