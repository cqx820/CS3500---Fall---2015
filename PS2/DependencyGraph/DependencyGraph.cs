using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //Declare variables
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            //Initialize variables
            this.dependents = new Dictionary<string, HashSet<string>>();
            this.dependees = new Dictionary<string, HashSet<string>>();
            this.size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            //Return the size
            get { return this.size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                //Set a temp hashset
                HashSet<string> temp;
                //If dependee's count is empty, then return 0
                if (dependees.Count == 0)
                {
                    return 0;
                }
                //If string s has dependees, then get the dependee's set and return the count
                if (dependees.ContainsKey(s))
                {
                    dependees.TryGetValue(s, out temp);
                    return temp.Count;
                }
                //Else, return 0
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            //Set a string type hashset named temp
            HashSet<string> temp;
            //If s has dependents, get the set of s's dependents
            if (dependents.ContainsKey(s))
            {
                dependents.TryGetValue(s, out temp);
                //If the temp is not empty, then return true
                if (temp.Count != 0)
                {
                    return true;
                }
                //Otherwise, return false
                return false;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            //Set a string type hashset named temp
            HashSet<string> temp;
            //If string s has dependees
            if (dependees.ContainsKey(s))
            {
                //Try to get the dependees of s
                dependees.TryGetValue(s, out temp);
                //If has dependee, then return true
                if (temp.Count != 0)
                {
                    return true;
                }
                //Otherwise, return false
                return false;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {

            if (dependents.ContainsKey(s))
            {
                return new HashSet<string>(dependents[s]);
            }
            return new HashSet<string>();

        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {

            if (dependees.ContainsKey(s))
            {
                return new HashSet<string>(dependees[s]);
            }
            return new HashSet<string>();

        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   s depends on t
        ///
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first.  S depends on T</param>
        public void AddDependency(string s, string t)
        {
            //Set a string type hashset named temp
            HashSet<string> temp;
            //If s contains dependents
            if (dependents.ContainsKey(s))
            {
                //Get all dependents of s and store in temp
                dependents.TryGetValue(s, out temp);
                //If t is not the dependents
                if (!temp.Contains(t))
                {
                    //Add t into the temp
                    temp.Add(t);
                    //Store the temp back into the key position of dictionary
                    dependents[s] = temp;
                    //If t does not have dependees
                    if (!dependees.ContainsKey(t))
                    {
                        //Initialize temp again and add s into temp
                        temp = new HashSet<string>();
                        temp.Add(s);
                        //Add t and temp into the dependees dictionary
                        dependees.Add(t, temp);
                    }
                    else
                    {
                        //Else, try to get all dependees of t
                        dependees.TryGetValue(t, out temp);
                        //Add s into temp
                        temp.Add(s);
                        //Store temp into key position of dependees dictionary
                        dependees[t] = temp;
                    }
                    //Size increment
                    this.size++;
                }
            }
            else
            {
                //Else, initialize temp again
                temp = new HashSet<string>();
                //Add t into temp
                temp.Add(t);
                //Store s and temp into the dependents dictionary
                dependents.Add(s, temp);
                //If t does not have dependee
                if (!dependees.ContainsKey(t))
                {
                    //Initialize temp again
                    temp = new HashSet<string>();
                    //Add s into temp
                    temp.Add(s);
                    //Store into the dependees dictionary
                    dependees.Add(t, temp);
                }
                else
                {
                    //Else, try to get all dependees of t
                    dependees.TryGetValue(t, out temp);
                    //Add s into temp
                    temp.Add(s);
                    //Store temp in key position of dependees dictionary
                    dependees[t] = temp;
                }
                //Size increment
                this.size++;
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //Set two string hastset named dependentTemp and dependeeTemp
            HashSet<string> dependentTemp;
            HashSet<string> dependeeTemp;
            //If s has dependents 
            if (dependents.ContainsKey(s))
            {
                //Try to get all dependents of s
                dependents.TryGetValue(s, out dependentTemp);
                //Try to get all dependees of t
                dependees.TryGetValue(t, out dependeeTemp);
                //If t has dependent 
                if (dependentTemp.Contains(t))
                {
                    //Remove t from dependentTemp
                    dependentTemp.Remove(t);
                    //Store dependentTemp into key position of dependents dictionary
                    dependents[s] = dependentTemp;
                    //Remove s from dependeeTemp
                    dependeeTemp.Remove(s);
                    //Store dependeeTemp into key position of dependees dictionary
                    dependees[t] = dependeeTemp;
                    //Decrease the size
                    this.size--;
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //Set a string type temporary hashset named temp
            HashSet<string> temp;
            //If s has dependent
            if (dependents.ContainsKey(s))
            {
                //Try to get all dependents of s
                dependents.TryGetValue(s, out temp);
                //Using for-each loop to handle all string in temp
                foreach (string r in temp.ToArray())
                {
                    //Call the RemoveDependency method
                    RemoveDependency(s, r);
                }
                //Using for-each loop to handle all string in newDependents
                foreach (string t in newDependents)
                {
                    //Call the AddDependency method
                    AddDependency(s, t);
                }
            }
            else
            {
                //Using for-each loop to handle all string in temp
                foreach (string t in newDependents)
                {
                    //Call the AddDependency method
                    AddDependency(s, t);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //Set a string type temporary hashset named temp
            HashSet<string> temp;
            //If s has dependee
            if (dependees.ContainsKey(s))
            {
                //Try to get all dependees of s
                dependees.TryGetValue(s, out temp);
                //Using for-each loop to handle all string in temp
                foreach (string r in temp.ToArray())
                {
                    //Call the RemoveDependency method
                    RemoveDependency(r, s);
                }
                //Using for-each loop to handle all string in temp
                foreach (string t in newDependees)
                {
                    //Call the AddDependency method
                    AddDependency(t, s);
                }
            }
            else
            {
                //Using for-each loop to handle all string in temp
                foreach (string t in newDependees)
                {
                    //Call the AddDependency method
                    AddDependency(t, s);
                }
            }
        }
    }
}