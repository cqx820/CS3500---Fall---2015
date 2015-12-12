using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Model
{
    /// <summary>
    /// The world represents the "state" of the simulation. 
    /// This class is responsible for tracking at least the following data: 
    /// the world Width and Height (please use read only 'constants'), all the foodCubes in the game
    /// </summary>
 
    public class World
    {
        /// <summary>
        /// Width of the world
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Height of the world
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The team id
        /// </summary>
        public int team_id;

        /// <summary>
        /// Data structure to store foodCubes by using uid as key
        /// </summary>
        public Dictionary<int, Cube> foodCubes;

        public Dictionary<int, List<Cube>> teamCubes;


        /// <summary>
        /// Event handler
        /// </summary>
        public event Action OnPlayerDeath;

        public readonly int heartbeatPerSecond;

        public readonly int foodValue;

        public readonly int playerStartMass;

        public readonly int maxFood;

        public readonly double minSplitMass;

        public readonly double maxSplitDistance;

        public readonly double maxSplits;

        public readonly double topSpeed;

        public readonly double lowSpeed;

        public readonly double attritionRate;

        public readonly double absorbDistanceDelta;

        public int uidNum;

        /// <summary>
        /// World constructor
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public World(int Width, int Height)
        {
            //Initializing variables
            this.Width = Width;
            this.Height = Height;
            team_id = 0;
            this.foodCubes = new Dictionary<int, Cube>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="heartbeatPerSecond"></param>
        /// <param name="topSpeed"></param>
        /// <param name="lowSpeed"></param>
        /// <param name="attritionRate"></param>
        /// <param name="foodValue"></param>
        /// <param name="playerStartMass"></param>
        /// <param name="maxFood"></param>
        /// <param name="minSplitMass"></param>
        /// <param name="maxSplitDistance"></param>
        /// <param name="maxSplits"></param>
        /// <param name="absorbDistanceDelta"></param>
        public World(int Width, int Height, int heartbeatPerSecond, double topSpeed, double lowSpeed, double attritionRate,
            int foodValue, int playerStartMass, int maxFood, double minSplitMass, double maxSplitDistance, double maxSplits, double absorbDistanceDelta)
        {
            //Initializing variables
            this.Width = Width;
            this.Height = Height;
            //  team_id = 0; //Should move to view specific code...
            this.foodCubes = new Dictionary<int, Cube>();
            this.teamCubes = new Dictionary<int, List<Cube>>();
            this.uidNum = 1;
            this.absorbDistanceDelta = absorbDistanceDelta;
            this.heartbeatPerSecond = heartbeatPerSecond;
            this.lowSpeed = lowSpeed;
            this.topSpeed = topSpeed;
            this.attritionRate = attritionRate;
            this.foodValue = foodValue;
            this.playerStartMass = playerStartMass;
            this.maxFood = maxFood;
            this.minSplitMass = minSplitMass;
            this.maxSplitDistance = maxSplitDistance;
            this.maxSplits = maxSplits;
        }

        /// <summary>
        /// Convinience method to add cubes to the food list dictionary
        /// </summary>
        /// <param name="cube">cube to be added</param>
        public void addFood(Cube cube)
        {
            Cube temp;
            if (foodCubes.ContainsKey(cube.uid))
            {
                foodCubes.TryGetValue(cube.uid, out temp);
                foodCubes.Remove(cube.uid);
            }

            foodCubes.Add(cube.uid, cube);
        }
        /// <summary>
        /// My helper method series: call this method in the View class
        /// The desrialized foodCubes be added in the World class by using this method
        /// Additional logic added due to server and client difference when adding cubes
        /// </summary>
        /// <param name="uid">The uid of cube</param>
        /// <param name="cube">The cube</param>
        /// 
        public void addCube(Cube cube)
        {
            //if (cube.Mass == 0)
            //{
            //    foodCubes.Remove(cube.uid);

            //    if (cube.uid == team_id) { if (OnPlayerDeath != null) OnPlayerDeath(); }
            //}
            //else
            //{
            Cube temp;
            if (foodCubes.ContainsKey(cube.uid))
            {
                foodCubes.TryGetValue(cube.uid, out temp);
                if (team_id != 0)
                    if (temp.Mass == 0)
                    {
                        foodCubes.Remove(temp.uid);
                        if (cube.uid == team_id) { if (OnPlayerDeath != null) OnPlayerDeath(); }
                    }
                foodCubes[cube.uid] = cube;
            }
            else
            {
                foodCubes.Add(cube.uid, cube);
            }
            //}
        }

        /// <summary>
        /// helper method to add cubes into usercube
        /// </summary>
        /// <param name="cube"></param>
        public void addUserCube(Cube cube)
        {
            List<Cube> temp;
            if (teamCubes.TryGetValue(cube.team_id, out temp))
            {
                temp.Add(cube);

            }
            else
            {
                temp = new List<Cube>();
                temp.Add(cube);
                teamCubes[cube.team_id] = temp;
            }
        }

        public int randomColor()
        {
            Random randomGen = new Random();
            List<KnownColor> names = new List<KnownColor>((KnownColor[])Enum.GetValues(typeof(KnownColor)));

            //remove colors that should not be randomly generated
            names.Remove(KnownColor.White);
            names.Remove(KnownColor.Black);
            names.Remove(KnownColor.DarkGreen);
            names.Remove(KnownColor.DarkOliveGreen);
            names.Remove(KnownColor.DarkSeaGreen);
            names.Remove(KnownColor.ForestGreen);
            names.Remove(KnownColor.Green);
            names.Remove(KnownColor.GreenYellow);
            names.Remove(KnownColor.GreenYellow);
            names.Remove(KnownColor.LawnGreen);
            names.Remove(KnownColor.LightGreen);
            names.Remove(KnownColor.LightSeaGreen);
            names.Remove(KnownColor.LimeGreen);
            names.Remove(KnownColor.MediumSeaGreen);
            names.Remove(KnownColor.MediumSpringGreen);
            names.Remove(KnownColor.PaleGreen);
            names.Remove(KnownColor.SeaGreen);
            names.Remove(KnownColor.SpringGreen);
            names.Remove(KnownColor.YellowGreen);
            KnownColor randomColorName = names[randomGen.Next(names.Count)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            return randomColor.ToArgb();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> getUids()
        {
            return foodCubes.Keys;
        }

        /// <summary>
        /// Get the cube by using uid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Cube getFoodCube(int uid)
        {
            Cube cube;
            foodCubes.TryGetValue(uid, out cube);
            return cube;
        }



        public int incUID()
        {
            return this.uidNum++;
        }

        /// <summary>
        /// Conviniece method to remove cubes from teamList
        /// </summary>
        /// <param name="cube"></param>
        public bool userRemove(Cube cube)
        {
            List<Cube> temp;
            if (teamCubes.TryGetValue(cube.team_id, out temp))
            {
                temp.Remove(cube);
                teamCubes.Remove(cube.team_id);
                if (temp.Count == 0)
                {
                    return true;
                }
                teamCubes.Add(cube.team_id, temp);
            }
            return false;
        }
    }
}