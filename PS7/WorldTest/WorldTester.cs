using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using System.Collections.Generic;

namespace ModelTester
{
    /// <summary>
    /// This is the tester of World class
    /// </summary>
    /// Author: TFBoys
    [TestClass]
    public class WorldTester
    {
        World world = new World(1000, 1000);
        Cube cube = new Cube(926.0, 682.0, -65536, 5571, 0, false, "MyName", 1000.0);
        Cube cube2 = new Cube(116, 350, -15560314, 1, 1, true, "", 1.0);
        Cube cube3 = new Cube(193, 55, -8243084, 2, 1, true, "", 1.0);
        Cube cube4 = new Cube(267, 580, -4759773, 3, 1, true, "", 1.0);
        Cube cube5 = new Cube(885.0, 452, -5905052, 4, 1, true, "", 1.0);
        Cube cube6 = new Cube(387, 666, -9834450, 5, 1, true, "", 1.0);
        Cube cube7 = new Cube(585, 869, -2210515, 6, 1, true, "", 1.0);
        Cube cube8 = new Cube(286, 884, -11930702, 7, 1, true, "", 1.0);
        Cube cube9 = new Cube(185, 392, -4232190, 8, 1, true, "", 1.0);
        Cube cube10 = new Cube(64, 614, -11083980, 9, 1, true, "", 1.0);
        Cube cube11 = new Cube(748, 40, -2364104, 10, 1, true, "", 1.0);

        /// <summary>
        /// Testing addCube method
        /// </summary>
        [TestMethod]
        public void addCubeTest1()
        {
            world.addCube(5571, cube);
            Assert.AreEqual(cube, world.cubes[5571]);
        }

        /// <summary>
        /// Testing addCube method
        /// </summary>
        [TestMethod]
        public void addCubeTest2()
        {
            world.addCube(36, cube2);
            Assert.AreEqual(cube2, world.cubes[36]);
        }

        /// <summary>
        /// Exception occurs
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void addCubeTest3()
        {
            cube2 = new Cube(885.0, 634.0, -15560314, 36, 1, true, "", 0.0);
            world.addCube(36, cube2);
            Assert.AreEqual(cube2, world.cubes[36]);
        }

        /// <summary>
        /// A little stress test of addCube method
        /// </summary>
        [TestMethod]
        public void addCubeTest4()
        {
            world.addCube(1, cube2);
            world.addCube(2, cube3);
            world.addCube(3, cube4);
            world.addCube(4, cube5);
            world.addCube(5, cube6);
            world.addCube(6, cube7);
            world.addCube(7, cube8);
            world.addCube(8, cube9);
            world.addCube(9, cube10);
            world.addCube(10, cube11);
            Assert.AreEqual(cube2, world.cubes[1]);
            Assert.AreEqual(cube3, world.cubes[2]);
            Assert.AreEqual(cube4, world.cubes[3]);
            Assert.AreEqual(cube5, world.cubes[4]);
            Assert.AreEqual(cube6, world.cubes[5]);
            Assert.AreEqual(cube7, world.cubes[6]);
            Assert.AreEqual(cube8, world.cubes[7]);
            Assert.AreEqual(cube9, world.cubes[8]);
            Assert.AreEqual(cube10, world.cubes[9]);
            Assert.AreEqual(cube11, world.cubes[10]);
        }

        /// <summary>
        /// The getUids method is not useful now, but still test
        /// </summary>
        [TestMethod]
        public void getUidsTest()
        {
            world.addCube(1, cube2);
            world.addCube(2, cube3);
            world.addCube(3, cube4);
            world.addCube(4, cube5);
            world.addCube(5, cube6);
            world.addCube(6, cube7);
            world.addCube(7, cube8);
            world.addCube(8, cube9);
            world.addCube(9, cube10);
            world.addCube(10, cube11);
            Assert.AreEqual(world.cubes.Keys, world.getUids());
        }

        /// <summary>
        /// Testing getCube method 
        /// </summary>
        [TestMethod]
        public void getCubeTest()
        {
            world.addCube(1, cube2);
            world.addCube(2, cube3);
            world.addCube(3, cube4);
            world.addCube(4, cube5);
            world.addCube(5, cube6);
            world.addCube(6, cube7);
            world.addCube(7, cube8);
            world.addCube(8, cube9);
            world.addCube(9, cube10);
            world.addCube(10, cube11);
            Assert.AreEqual(cube2, world.getCube(1));
            Assert.AreEqual(cube3, world.getCube(2));
            Assert.AreEqual(cube4, world.getCube(3));
            Assert.AreEqual(cube5, world.getCube(4));
            Assert.AreEqual(cube6, world.getCube(5));
            Assert.AreEqual(cube7, world.getCube(6));
            Assert.AreEqual(cube8, world.getCube(7));
            Assert.AreEqual(cube9, world.getCube(8));
            Assert.AreEqual(cube10, world.getCube(9));
            Assert.AreEqual(cube11, world.getCube(10));
        }
    }
}