using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model
{
    /// <summary>
    /// The world represents the "state" of the simulation. 
    /// This class is responsible for tracking at least the following data: 
    /// the world Width and Height (please use read only 'constants'), all the cubes in the game
    /// </summary>
    /// Author: TFBoys
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
        /// Data structure to store cubes by using uid as key
        /// </summary>
        public Dictionary<int, Cube> cubes;

        /// <summary>
        /// Event handler
        /// </summary>
        public event Action OnPlayerDeath;

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
            this.cubes = new Dictionary<int, Cube>();
        }

        /// <summary>
        /// My helper method series: call this method in the View class
        /// The desrialized cubes be added in the World class by using this method
        /// </summary>
        /// <param name="uid">The uid of cube</param>
        /// <param name="cube">The cube</param>
        public void addCube(int uid, Cube cube)
        {
            if (cube.Mass == 0)
            {
                cubes.Remove(cube.uid);

                if (cube.uid == team_id) { if (OnPlayerDeath != null) OnPlayerDeath(); }
            }
            else
            {
                cubes[uid] = cube;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> getUids()
        {
            return cubes.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Cube getCube(int id)
        {
            Cube cube;
            cubes.TryGetValue(id, out cube);
            return cube;
        }
    }
}