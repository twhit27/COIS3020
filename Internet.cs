/*
 * Assignment 1: The Internet
 * Jamie Le Neve, Camryn Moerchen, Victoria Whitworth
 * COIS 3020H
 * Brian Patrick
 * February 4, 2024
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace COIS3020HAssignment1
{
    public class ServerGraph
    {
        //WebServer constructor
        private class WebServer
        {
            public string name;
            //might not be allowed
            public List<WebPage> P = new List<WebPage>();

            public override string ToString()
            {
                return name;
            }

        }

        private WebServer[] V;
        private bool[,] E;
        private int NumServers;

        //ServerGraph constructor
        //create an empty server graph
        public ServerGraph()
        {
            //create empty V
            V = new WebServer[5];

            //create truly empty multi dm array
            E = new bool[5, 5];

            //NumServers = 0
            NumServers = 0;

            Console.WriteLine("The server graph was created!");

        }

        // Return the index of the server with the given name; otherwise return -1
        private int FindServer(string name)
        {
            //special case for empty list
            if (V[0] == null)
                return -1;

            for (int i = 0; i < V.Length; i++)
            {
                //if you get to a null position, it means you've looped through whole V list w/o finding it
                if (V[i] == null)
                    break;
                else if (V[i].name == name)
                    return i;
            }

            return -1;
        }

        // Double the capacity of the server graph with the respect to web servers
        //only the V list? I feel like E might be needed too

        private void DoubleCapacity()
        {
            int sizeOld = V.Length;
            int sizeNew = V.Length * 2;

            WebServer[] V2 = new WebServer[(sizeNew)];
            bool[,] E2 = new bool[(sizeNew), (sizeNew)];

            for (int i = 0; i < sizeOld; i++)
            {
                V2[i] = V[i];
            }

            V = V2;

            for (int i = 0; i < sizeOld; i++)
            {
                for (int j = 0; j < sizeOld; j++)
                {
                    E2[i, j] = E[i, j];
                }
            }

            E = E2;
        }


        // Add a server (vertex) with the given name and connect it to the other server
        // Return true if successful; otherwise return false
        // Shouldn't be able to make a server if there isn't a connecting server
        public bool AddServer(string name, string other)
        {
            int start = FindServer(name);
            int end = FindServer(other);

            //if the server getting added is new
            if (start == -1)
            {
                //extend server number if needed
                if (NumServers + 1 > V.Length)
                    DoubleCapacity();

                WebServer server = new WebServer();
                server.name = name;

                //special case for the very first server to be added, which allows it to connect to itself
                if (end == -1 && V[0] == null)
                {
                    V[NumServers] = server;
                    //simply set the diagonal (loop for now)
                    E[NumServers, NumServers] = true;
                    NumServers++;
                }
                else if (end != -1)
                {
                    V[NumServers] = server;
                    E[NumServers, end] = true;
                    E[end, NumServers] = true;
                    NumServers++;
                }
                else
                {
                    Console.WriteLine("{0} server does not exist.", other);
                    return false;
                }


                Console.WriteLine("Sever {0} successfully added.", name);
                return true;
            }
            //else the server alredy exists
            //cascading error messages
            Console.WriteLine("{0} server already exists.", name);
            return false;

        }

        // Add a webpage to the server with the given name
        // Return true if successful; other return false
        public bool AddWebPage(WebPage w, string name)
        {
            int find = FindServer(name);

            //server in list
            if (find != -1)
            {
                V[find].P.Add(w);
                Console.WriteLine("Webpage {0} was is now hosted on server {1}.", w.Name, name);
                return true;
            }

            //else host server doesn't exist
            Console.WriteLine("Could not find host.");
            return false;
        }

        // Remove the server with the given name by assigning its connections
        // and webpages to the other server
        // Return true if successful; otherwise return false
        //some type of cloning to check for success?

        //go to the row of the server, check which column has values, use the column number to reassign the webpages to the other server
        public bool RemoveServer(string name, string other)
        {
            int start = FindServer(name);
            int end = FindServer(other);

            if ((start != -1) && (end != -1))
            {
                //traversing down a column, regardless of value reassign optic cables
                for (int i = 0; i < NumServers; i++)
                {
                    //Ensuring not change any of the true values for the other server
                    if (E[i, end] == false)
                        E[i, end] = E[i, start];
                }

                //traversing across a row, regardless of value reassign optic cables
                for (int i = 0; i < NumServers; i++)
                {
                    //Ensuring not change any of the true values for the other server
                    if (E[end, i] == false)
                        E[end, i] = E[start, i];
                }

                //process of tacking on webpages to the other server
                for (int j = 0; j < V[start].P.Count; j++)
                {
                    V[end].P.Add(V[start].P[j]);
                }

                //process of moving last server up into old row & column
                NumServers--;
                V[start] = V[NumServers];
                for (int j = NumServers; j >= 0; j--)
                {
                    E[j, start] = E[j, NumServers];
                    E[start, j] = E[NumServers, j];
                }


                Console.WriteLine("Server {0} was successfully removed and it's connections moved to {1}.", name, other);
                return true;
            }

            Console.WriteLine("Could not find one of the two servers");
            return false;
        }



        public bool RemoveWebPage(string webpage, string host)
        {
            // find the host
            for (int i = 0; i < NumServers; i++)
            {
                //enter into the server
                if (V[i].name == host)
                {
                    // loop through webpages associated with host server
                    for (int j = 0; j < V[i].P.Count; j++)
                    {
                        // find the webpage to remove
                        if (V[i].P[j].Name == webpage)
                        {
                            V[i].P.RemoveAt(j);
                            return true;
                        }
                    }

                    // if this loop finishes, the webpage doesn't exist
                    Console.WriteLine("Webpage doesn't exist.");
                    return false;
                }
            }

            // otherwise server doesn't exist
            Console.WriteLine("Server doesn't exist.");
            return false;

        }

        // Add a connection from one server to another
        // Return true if successful; otherwise return false
        // Note that each server is connected to at least one other server
        public bool AddConnection(string from, string to)
        {
            int i = FindServer(from);
            int j = FindServer(to);

            if (i > -1 && j > -1)
            {
                if (E[i, j] == false)
                {
                    E[i, j] = true;
                    E[j, i] = true;

                    Console.WriteLine("A new connection was created between {0} and {1}.", from, to);

                    return true;
                }
                Console.WriteLine("A connection between {0} and {1} already exists.", from, to);
                return false;
            }
            Console.WriteLine("Could not find one of the two servers");
            return false;
        }

        // OVERALL METHOD
        // "Remove" each server 1 at a time, then perform a DFS on the graph, if you visit less servers than the orginal graph - 1, that server is critical
        public string[] CritcialServers()
        {
            int before = NumServers;    // the functionality of the AddServer method doesn't allow the graph to be split normally, so we can set before to NumServers
            int after = 0;
            int pathCount = 0;
            bool[,] temp = new bool[NumServers, NumServers];
            bool[] visited = new bool[NumServers];
            string[] path = new string[NumServers];

            // create a deep copy of E
            for (int i = 0; i < NumServers; i++)
            {
                for (int j = 0; j < NumServers; j++)
                {
                    temp[i, j] = E[i, j];
                }
            }

            // MAIN LOOP
            // will perform the delete/restoration for each server
            for (int i = 0; i < NumServers; i++)
            {
                // remove the connections (rows & columns) of the ith server, and reset visited servers values
                for (int j = 0; j < NumServers; j++)
                {
                    visited[j] = false;
                    E[i, j] = E[j, i] = false;
                }

                // get the number of servers you can visit with this server removed
                if (i == 0)
                    after = DepthFirstSearch(1);
                else
                    after = DepthFirstSearch(0);

                // if you visit less servers than NumServers - 1, that means the graph is split somewhere and this ith server is critical
                if (after < before - 1)
                {
                    path[pathCount] = V[i].name;
                    pathCount++;
                }

                // restore ith connections before moving on to next server
                for (int j = 0; j < NumServers; j++)
                {
                    E[i, j] = temp[i, j];
                    E[j, i] = temp[j, i];
                }

            }
            if (path.All(x => x == null))
            {
                Console.WriteLine("No Criticial Servers");
            }
            return path;
        }

        // The following 2 methods are Professor Patricks with two modifictaions:
        // 1) The DFS will only start at a specefied server and go as far as it can, it will not search the whole graph
        // (supports the critical functionality described in CriticalServer)
        // 2) It will keep track of how many servers get visited in this one travseral
        public int DepthFirstSearch(int startVertex)
        {
            bool[] visited = new bool[NumServers];
            int count = 0;

            // Set all vertices as unvisited
            for (int i = 0; i < NumServers; i++)
                visited[i] = false;

            DepthFirstSearch(startVertex, visited, ref count); // Start DFS from the specified server
            return count;
        }

        private void DepthFirstSearch(int i, bool[] visited, ref int count)
        {
            visited[i] = true;
            count++;

            // Visit next unvisited adjacent server
            for (int j = 0; j < NumServers; j++)
            {
                if (!visited[j] && E[i, j])
                {
                    DepthFirstSearch(j, visited, ref count);
                }
            }
        }

        public void DepthFirstSearch(bool[] v)
        {
            int i;
            int restarted;
            bool[] visited = v;

            for (i = 0; i < NumServers; i++)     // Set all vertices as unvisited
                visited[i] = false;

            for (i = 0; i < NumServers; i++)
                if (!visited[i])                  // (Re)start with vertex i
                {
                    DepthFirstSearch(i, visited);
                }
        }

        // Professors code for a Depth First Search
        private void DepthFirstSearch(int i, bool[] visited)
        {
            int j;

            visited[i] = true;    // Output vertex when marked as visited

            for (j = 0; j < NumServers; j++)    // Visit next unvisited adjacent vertex
                if (!visited[j] && E[i, j])
                    DepthFirstSearch(j, visited);
        }

        // Return the shortest path from one server to another
        // Hint: Use a variation of the breadth-first search
        public int ShortestPath(string from, string to)
        {
            // intialization
            int count = 0, fromIndex = FindServer(from), toIndex = FindServer(to);
            string path = "";
            bool[] visited = new bool[NumServers];
            Queue<int> Q = new Queue<int>();
            List<int> pathList = new List<int>();

            // visited will be used to mark nodes as visited
            for (int i = 0; i < NumServers; i++)
                visited[i] = false;

            // if both servers exist...
            if (fromIndex != -1 && toIndex != -1)
            {
                // since we din't need to traverse the entire graph, start the process at from
                Q.Enqueue(fromIndex);
                visited[fromIndex] = true;

                //pathList.Add(fromIndex);

                // loop until we arrive at the 'to' server
                while ((!Q.Contains(toIndex)) && (!pathList.Contains(toIndex)))
                {
                    // these 3 lines 'process' the server
                    int i = Q.Dequeue();
                    int count2 = 0;
                    path = path + V[i].name + " ";
                    pathList.Add(i);
                    count++;


                    // from prof's code, move down the line in the matrix checking for server connections
                    for (int j = 0; j < NumServers; j++)
                    {
                        if (!visited[j] && E[i, j] && !pathList.Contains(toIndex))
                        {
                            Q.Enqueue(j);
                            visited[j] = true;           // Mark vertex as visited
                            count2++;
                        }

                    }
                    if (count2 == 0)
                    {
                        pathList.Remove(i);
                    }

                }
                //Comparing the queue and array to find the shortest path
                for (int n = 0; n < visited.Length; n++)
                {
                    if (pathList.Count(x => x == n) > 1)
                    {
                        count--;
                    }
                }
                Console.WriteLine("Path List count {0}:", pathList.Count());
                Console.WriteLine("The shortest path from {0} to {1} is {2}", from, to, path);

                return count;

            }

            Console.WriteLine("Either 1 or both server names are invalid");
            return -1;
        }

        // Print the name and connections of each server as well as
        // the names of the webpages it hosts
        public void PrintGraph()
        {
            for (int i = 0; i < NumServers; i++)
            {
                //Printing out the server's connections
                Console.WriteLine("\n**Server {0}**\nConnections to: ", V[i].name);
                for (int j = 0; j < NumServers; j++)
                {
                    if (E[i, j] != false)
                    {
                        Console.WriteLine("{0}", V[j].name);
                    }

                }

                //Printing out the webpages the server hosts
                Console.WriteLine("Hosted web pages: ", V[i].name);
                for (int j = 0; j < V[i].P.Count; j++)
                {

                    Console.WriteLine("{0}", V[i].P[j].Name);

                }

                Console.WriteLine();
            }

        }

    }

    public class WebPage
    {
        //data members
        public string Name { get; set; }
        public string Server { get; set; }
        public List<WebPage> E { get; set; }

        //constructor
        //possibly need to specify something in the header, big space in the assign. doc 
        public WebPage(string name, string host)
        {
            Name = name;
            Server = host;
            E = new List<WebPage>(); // Initializing a list of webpages for the hyperlinks on the webpage
        }

        // Searching the list of webpages (hyperlinks) to find the index of a webpage 
        // Return the index if the link is found, and return -1 if the link is not found
        public int FindLink(string name)
        {
            int i;

            // Search through the list of hyperlinks
            for (i = 0; i < E.Count; i++)
            {
                if (E[i].Name.Equals(name))
                    return i;
            }
            return -1;
        }

        //Evaluates if two webpages are the same / 'equal to each other'
        //Returns true if they are equal, and false if they are not equal
        public bool Equals(string name, string host)
        {
            //If the names and hosts of both of the webpages are the same, the webpages are the same
            if ((Name == name) && (Server == host))
            {
                return true;
            }

            //If they are not return false
            return false;
        }
    }

    public class WebGraph
    {
        private List<WebPage> P;

        // Create an empty WebGraph
        public WebGraph()
        {
            P = new List<WebPage>(); // Initializing the web graph to have an empty list for webpages
            Console.WriteLine("The webgraph was created!");
        }

        // Return the index of the webpage with the given name; otherwise return -1
        private int FindPage(string name)
        {
            int i;

            // Search through the list of webpages
            for (i = 0; i < P.Count; i++)
            {
                if (P[i].Name.Equals(name))
                    return i;
            }
            return -1;
        }

        // Add a webpage with the given name and store it on the host server
        // Return true if successful; otherwise return false
        public bool AddPage(string name, string host, ServerGraph S)
        {
            // If the page already exists in the web graph, don't add it
            if (FindPage(name) == -1)
            {
                WebPage p = new WebPage(name, host);

                // Attempt to add the webpage to the server
                // If it is successfully added, add it to the webpage graph
                // This allows us to ensure that the host exists in the server graph
                // Also allows us to check if the webpage is a duplicate for that server
                if (S.AddWebPage(p, host) == true)
                {
                    P.Add(p); // Add the webpage to the webpage graph
                    Console.WriteLine("Page was successfully added!");
                    return true; // Return true if the webpage was successfully added 
                }

                //Error message will be printed out in AddWebPage if this fails
            }

            Console.WriteLine("Page was not added.");
            return false; // Return false if the webpage was not added
        }

        // Remove the webpage with the given name, including the hyperlinks
        // from and to the webpage
        // Return true if successful; otherwise return false

        public bool RemovePage(string name, ServerGraph S)
        {
            //Find the location of the page being deleted in the WebGraph
            int pageIndex = FindPage(name);

            //Only remove page if it does not exist
            if (pageIndex != -1)
            {
                //Go to each page of the WebGraph to check if it has hyperlinks that point to the page being removed
                for (int j = 0; j < P.Count; j++)
                {
                    //Go through all of the hyperlinks on the page and delete the hyperlink if it points to the page being removed
                    //Since there are no duplicate hyperlinks, we only have to check for one hyperlink
                    int hypIndex = P[j].FindLink(name);
                    if (hypIndex != -1)
                    {
                        RemoveLink(P[j].Name, name);
                    }
                }
                //Removing the page
                S.RemoveWebPage(name, P[pageIndex].Server);
                P.RemoveAt(pageIndex);
                Console.WriteLine("The page '{0}' was successfully deleted!", name);
                return true;
            }

            Console.WriteLine("Page was not removed since it does not exist.");
            return false;
        }

        // Add a hyperlink from one webpage to another
        // Return true if successful; otherwise return false
        public bool AddLink(string from, string to)
        {
            // Checking that both of the pages exist
            // If they dont, return false
            if ((FindPage(from) == -1) || (FindPage(to) == -1))
            {
                Console.WriteLine("Link was not added because at least one of the pages do not exist.");
                return false;
            }

            // Checking that the webpage doesn't already have a hyperlink to the webpage
            // If it does, return false
            if (P[FindPage(from)].FindLink(to) != -1)
            {
                Console.WriteLine("Link was not added because {0} already has a hyperlink to {1}.", from, to);
                return false;
            }

            // Adding the hyperlink
            P[FindPage(from)].E.Add(P[FindPage(to)]);
            Console.WriteLine("Hyperlink was successfully added!");
            return true;
        }

        // Remove a hyperlink from one webpage to another
        // Return true if successful; otherwise return false
        public bool RemoveLink(string from, string to)
        {
            // Checking that the webpage has a hyperlink to the webpage
            // If it doesn't, return false
            if (P[FindPage(from)].FindLink(to) == -1)
            {
                Console.WriteLine("Link was not removed because it does not exist.");
                return false;
            }


            // Removing the hyperlink
            P[FindPage(from)].E.Remove(P[FindPage(to)]);
            Console.WriteLine("Hyperlink was successfully removed!");
            return true;
        }

        // Return the average length of the shortest paths from the webpage with
        // given name to each of its hyperlinks
        // Hint: Use the method ShortestPath in the class ServerGraph
        public float AvgShortestPaths(string name, ServerGraph S)
        {
            int pageIndex = FindPage(name);
            float avgShortPath = 0.0f;
            int pathSum = 0;
            string mainHost, linkHost;

            //Checking that the webpage exists
            if (pageIndex == -1)
            {
                Console.WriteLine("AvgShortestPath was not calculated because page does not exist.");
                return 0;
            }

            //Checking that the webpage has hyperlinks
            if (P[pageIndex].E.Count == 0)
            {
                Console.WriteLine("AvgShortestPath was not calculated because page does not have any hyperlinks.");
                return 0;
            }

            mainHost = P[pageIndex].Server;
            //Console.WriteLine(mainHost);

            //Find the shortest path to each hyperlink
            for (int j = 0; j < P[pageIndex].E.Count; j++)
            {
                linkHost = P[pageIndex].E[j].Server;
                pathSum += S.ShortestPath(mainHost, linkHost);
            }

            //Calculate the average shortest path
            avgShortPath = pathSum / (float) P[pageIndex].E.Count;

            Console.WriteLine("The average shortest path was successfully calculated and is {0}.", avgShortPath);

            return avgShortPath;
        }


        // Print the name and hyperlinks of each webpage
        public void PrintGraph()
        {
            //Printing the name of each webpage
            for (int i = 0; i < P.Count; i++)
            {
                Console.WriteLine(P[i].Name);

                //Printing out each hyperlink associated with the webpage
                for (int j = 0; j < P[i].E.Count; j++)
                {
                    Console.WriteLine("    Hyperlink to: " + P[i].E[j].Name);
                }
            }
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            //1. Instantiate a server graph and a web graph
            //we may want to add a condition here (if we decided on user input and not hard coding) for adding the very first graph (i.e, connect it to itself)
            ServerGraph foo = new ServerGraph();
            WebGraph foo2 = new WebGraph();
            //2. Add a number of servers
            foo.AddServer("Canada", "Canada");
            foo.AddServer("Europe", "Canada");
            foo.AddServer("Asia", "Europe");
            foo.AddServer("Africa", "Asia");
            foo.AddServer("Australia", "Asia");
            foo.AddServer("Antarctica", "Canada");
            foo.AddServer("Canada", "Europe");
            foo.AddServer("US", "Peru");
            //3. Add additional connections between servers
            foo.AddConnection("Canada", "Asia");
            foo.AddConnection("Africa", "Canada");
            foo.AddConnection("Asia", "Africa");
            foo.AddConnection("America", "Canada");
            //4. Add a number of webpages to various servers
            foo2.AddPage("Wikipedia", "Canada", foo);
            foo2.AddPage("Trent", "Canada", foo);
            foo2.AddPage("Google", "Europe", foo);
            foo2.AddPage("YouTube", "Asia", foo);
            foo2.AddPage("GitHub", "America", foo);
            foo2.AddPage("National Geographic", "Australia", foo);
            //5. Add hyperlinks between web pages
            foo2.AddLink("Google", "Wikipedia");
            foo2.AddLink("Google", "Trent");
            foo2.AddLink("Wikipedia", "YouTube");
            foo2.AddLink("Google", "GitHub");
            foo2.AddLink("National Geographic", "Google");
            foo2.AddLink("National Geographic", "Trent");
            foo2.AddLink("National Geographic", "YouTube");
            //8. Calculate the average shortest distance to hyperlinks of a given webpage
            Console.WriteLine();
            foo2.AvgShortestPaths("National Geographic", foo);
            foo.PrintGraph();
            foo2.PrintGraph();
            //7. Determine the critical servers of the remaining Internet
            Console.WriteLine();
            Console.WriteLine("Critical Servers: ");
            String[] criticals = foo.CritcialServers();
            foreach (String critical in criticals)
            {
                Console.WriteLine(critical);
            }
            //5. Remove hyperlinks between the webpages
            foo2.RemoveLink("Wikipedia", "YouTube");
            foo2.RemoveLink("Google", "Wikipedia");
            foo2.RemoveLink("Trent", "Wikipedia");
            //6. Remove webpages
            foo2.RemovePage("Wikipedia", foo);
            foo2.RemovePage("YouTube", foo);
            foo2.RemovePage("GitHub", foo);
            //6. Remove servers
            foo.RemoveServer("Asia", "Europe");
            foo.RemoveServer("Africa", "Canada");
            foo.PrintGraph();
            foo2.PrintGraph();
            //7. Determine the critical servers of the remaining Internet
            Console.WriteLine();
            Console.WriteLine("Critical Servers: ");
            String[] criticals2 = foo.CritcialServers();
            foreach (String critical2 in criticals2)
            {
                Console.WriteLine(critical2);
            }
            //6. Remove servers
            foo.RemoveServer("Antarctica", "Australia");
            foo.RemoveServer("America", "Canada");
            foo.RemoveServer("Australia", "America");
            foo.PrintGraph();
            foo2.PrintGraph();
            //7. Determine the critical servers of the remaining Internet
            Console.WriteLine();
            Console.WriteLine("Critical Servers: ");
            String[] criticals3 = foo.CritcialServers();
            foreach (String critical3 in criticals3)
            {
                Console.WriteLine(critical3);
            }
            //8. Calculate the average shortest distance to hyperlinks of a given webpage
            foo2.AvgShortestPaths("Google", foo);
            Console.WriteLine();
            foo2.AvgShortestPaths("Trent", foo);
            Console.WriteLine();
            foo2.AvgShortestPaths("Wikipedia", foo);
            Console.ReadLine();
        }
    }
}
