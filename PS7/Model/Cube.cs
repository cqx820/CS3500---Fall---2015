using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model
{
    /// <summary>
    /// This is the Cube class
    /// The Cube class should represent a cube in the game logic
    /// </summary>
    /// Author: TFBoys
    public class Cube
    {
        /// <summary>
        /// The uid of cube
        /// </summary>
        [JsonProperty]
        public int uid { get; set; }

        /// <summary>
        /// The x-coordinate of the cube 
        /// </summary>
        [JsonProperty]
        public double loc_x { get; set; }

        /// <summary>
        /// The y-coordinate of the cube
        /// </summary>
        [JsonProperty]
        public double loc_y { get; set; }

        /// <summary>
        /// The mass of the cube
        /// </summary>
        [JsonProperty]
        public double Mass { get; set; }

        /// <summary>
        /// The color of the cube
        /// </summary>
        [JsonProperty]
        public int argb_color { get; set; }

        /// <summary>
        /// The cube's name
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Whether the cube is food
        /// </summary>
        [JsonProperty]
        public bool food { get; set; }

        /// <summary>
        /// The new field team_id
        /// </summary>
        [JsonProperty]
        public int team_id { get; set; }

        /// <summary>
        /// The constructor of Cube class
        /// </summary>
        /// <param name="loc_x">The position in space</param>
        /// <param name="loc_y">The position in space</param>
        /// <param name="argb_color">A color</param>
        /// <param name="uid">A unique id</param>
        /// <param name="team_id">Id for splited cube</param>
        /// <param name="food">Food status</param>
        /// <param name="Name">A name -- if this is a player cube</param>
        /// <param name="Mass">A mass</param>
        [JsonConstructor]
        public Cube(double loc_x, double loc_y, int argb_color, int uid, int team_id, bool food, string Name, double Mass)
        {
            //Initializing variables
            this.loc_x = loc_x;
            this.loc_y = loc_y;
            this.argb_color = argb_color;
            this.uid = uid;
            this.food = food;
            this.Name = Name;
            this.Mass = Mass;
            this.team_id = team_id;
        }

        /// <summary>
        /// The width of a cube is defined by the square root of the mass.
        /// For better game experience, I set the food's mass three times bigger
        /// </summary>
        /// <returns></returns>
        public int getWidth()
        {
            ////return Convert.ToInt32(Math.Sqrt(this.Mass));
            if (food)
            {
                return Convert.ToInt32(Mass * 2);
            }
            return Convert.ToInt32(Math.Pow(this.Mass, 0.65));
        }

        /// <summary>
        /// It would be useful from an SE point of view to create other properties associated with your cube object
        /// </summary>
        /// <returns></returns>
        public double getTop()
        {
            return loc_y + (getWidth() / 2);
        }

        /// <summary>
        /// It would be useful from an SE point of view to create other properties associated with your cube object
        /// </summary>
        /// <returns></returns>
        public double getBottom()
        {
            return loc_y - (getWidth() / 2);
        }

        /// <summary>
        /// It would be useful from an SE point of view to create other properties associated with your cube object
        /// </summary>
        /// <returns></returns>
        public double getRight()
        {
            return loc_x - (getWidth() / 2);
        }

        /// <summary>
        /// It would be useful from an SE point of view to create other properties associated with your cube object
        /// </summary>
        /// <returns></returns>
        public double getLeft()
        {
            return loc_x + (getWidth() / 2);
        }
    }
}