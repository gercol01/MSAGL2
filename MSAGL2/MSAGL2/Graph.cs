using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.GraphmapsWithMesh;
using Microsoft.Msagl.Routing.ConstrainedDelaunayTriangulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace MSAGL2
{
    public partial class Graph : Form
    {
        //Setup
        //create a viewer object
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        //create a graph object
        Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

        string TreeString = null; //Tree data structure in string format
        IDictionary<string, string> TreeDataStructureString = new Dictionary<string, string>();

        public Graph(string tree) //constructor for Unity
        {
            InitializeComponent();
            TreeString = tree;

            viewer.UndoRedoButtonsVisible = false;
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.SaveAsImageEnabled = false;
            viewer.SaveAsMsaglEnabled = false;
            viewer.SaveInVectorFormatEnabled = false;


            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;
            //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            //Microsoft.Msagl.Drawing.Node d = graph.FindNode("D");

            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;

            TreeDataStructureString = TreeString.Split(new[] { '[', ']', '&' }, StringSplitOptions.RemoveEmptyEntries)//the characters which will be removed from the array
               .Select(part => part.Split(',')) //divide the arrays to keys & values by the ',' character
               .ToDictionary(split => split[0], split => split[1]);

            //iterate through the dictionary
            foreach (KeyValuePair<string, string> entry in TreeDataStructureString)
            {
                //if the node does not have children, there is no need for an edge
                if (entry.Value != "")
                {
                    //split the values into an array of values by the '|' and ' ' characters
                    string[] objectsWithPreposition = entry.Value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    //split the objects from the prepositions
                    foreach (string obj in objectsWithPreposition)
                    {
                        //split of the object from the preposition from the '-' character
                        string[] finalObj = obj.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        //Add an edge to the graph
                        graph.AddEdge(entry.Key, finalObj[0]).LabelText = finalObj[1];
                    }
                }
                else
                {
                    //if it has no children, only the node is created
                    graph.AddNode(entry.Key);
                }
            }

            //var n = graph.FindNode("Table");
            //n.Label.FontSize = 2;

            Console.WriteLine("test");

            //bind the graph to the viewer
            viewer.Graph = graph;

            //associate the viewer with the form
            this.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Controls.Add(viewer);
            this.ResumeLayout();
        }

        public Graph() //normal/default constructor for normal start
        {
            InitializeComponent();
            //TreeString = "[cube_1,]";
            //TreeString = "[Gerard,Chair-on|Book-under|Laptop-right_of]&[Chair,Box-left_of]&[Book,Cat-left_of|Mouse-behind|Stapler-infront]&[Laptop,Monitor-on]&[Box,Man-under]&[Cat ]&[Mouse,Cheese-on]&[Stapler,]&[Monitor,]&[Man,]&[Cheese,]";
            TreeString = "[Gerard,Chair-on|Book-under|cube_1-right_of]&[cube_1,]";

            viewer.UndoRedoButtonsVisible = false;
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.SaveAsImageEnabled = false;
            viewer.SaveAsMsaglEnabled = false;
            viewer.SaveInVectorFormatEnabled = false;


            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;
            //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            //Microsoft.Msagl.Drawing.Node d = graph.FindNode("D");

            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;

            TreeDataStructureString = TreeString.Split(new[] { '[', ']', '&' }, StringSplitOptions.RemoveEmptyEntries)//the characters which will be removed from the array
               .Select(part => part.Split(',')) //divide the arrays to keys & values by the ',' character
               .ToDictionary(split => split[0], split => split[1]);

            //iterate through the dictionary
            foreach (KeyValuePair<string, string> entry in TreeDataStructureString)
            {
                //if the node does not have children, there is no need for an edge
                if (entry.Value != "")
                {
                    //split the values into an array of values by the '|' and ' ' characters
                    string[] objectsWithPreposition = entry.Value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    //split the objects from the prepositions
                    foreach (string obj in objectsWithPreposition)
                    {
                        //split of the object from the preposition from the '-' character
                        string[] finalObj = obj.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        //Add an edge to the graph
                        graph.AddEdge(entry.Key, finalObj[0]).LabelText = finalObj[1];
                    }
                }
                else {
                    //if it has no children, only the node is created
                    graph.AddNode(entry.Key);
                }
                
            }
            //graph.FindNode("Gerard").GeometryNode.Center = new Microsoft.Msagl.Core.Geometry.Point(100, 100);

            //var n = graph.FindNode("Gerard");
            //n.Label.FontSize = 2;
            //n.GeometryNode.Center = new Point(0, 0);

            Console.WriteLine("test");

            //bind the graph to the viewer
            viewer.Graph = graph;

            //associate the viewer with the form
            this.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Controls.Add(viewer);
            this.ResumeLayout();
        }

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    viewer.UndoRedoButtonsVisible = false;
        //    viewer.EdgeInsertButtonVisible = false;
        //    viewer.LayoutAlgorithmSettingsButtonVisible = false;
        //    viewer.SaveGraphButtonVisible = false;
        //    viewer.SaveButtonVisible = false;
        //    viewer.NavigationVisible = false;
        //    viewer.SaveAsImageEnabled = false;
        //    viewer.SaveAsMsaglEnabled = false;
        //    viewer.SaveInVectorFormatEnabled = false;


        //    //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
        //    //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;
        //    //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
        //    //Microsoft.Msagl.Drawing.Node d = graph.FindNode("D");

        //    //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
        //    //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;

        //    TreeDataStructureString = TreeString.Split(new[] { '[', ']', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)//the characters which will be removed from the array
        //       .Select(part => part.Split(',')) //divide the arrays to keys & values by the ',' character
        //       .ToDictionary(split => split[0], split => split[1]);

        //    //iterate through the dictionary
        //    foreach (KeyValuePair<string, string> entry in TreeDataStructureString)
        //    {
        //        //if the node does not have children, there is no need for an edge
        //        if (entry.Value != " ")
        //        {
        //            //split the values into an array of values by the '|' and ' ' characters
        //            string[] objectsWithPreposition = entry.Value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //            //split the objects from the prepositions
        //            foreach (string obj in objectsWithPreposition)
        //            {
        //                //split of the object from the preposition from the '-' character
        //                string[] finalObj = obj.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

        //                //Add an edge to the graph
        //                graph.AddEdge(entry.Key, finalObj[0]).LabelText = finalObj[1];
        //            }
        //        }
        //    }

        //    //var n = graph.FindNode("Table");
        //    //n.Label.FontSize = 2;

        //    Console.WriteLine("test");

        //    //bind the graph to the viewer
        //    viewer.Graph = graph;

        //    //associate the viewer with the form
        //    this.SuspendLayout();
        //    viewer.Dock = System.Windows.Forms.DockStyle.Fill;
        //    panel1.Controls.Add(viewer);
        //    this.ResumeLayout();
        //}

        //method used to add nodes
        public void addChildren(IDictionary<node, node[]> tree, node parent, node child)
        {
            //child.setLevel(parent.getLevel() + 1);

            node[] temp = new node[] { }; //utility array
            //give the level here

            //Checking if their already exists a relationship
            if (tree.ContainsKey(parent))
            {
                //get the current children
                temp = tree[parent];

                //add the new child
                temp = temp.Append(child).ToArray();

                //overwrite the current children
                tree[parent] = temp;

                //create new record with null children
                tree[child] = new node[] { };
            }
            else
            {//if it is the first time adding a relationship
                tree[parent] = new node[] {  };
            }
        }

        //method used to turn a tree into a string
        public string TreeToString(IDictionary<node, node[]> tree)
        {
            //the new Tree which has a format of string,string
            IDictionary<string, string> Tree = new Dictionary<string, string>();

            string ValueArrayToString; //variable used to convert the values array into one continuous string
            string KeyToString; //variable used to convert Node key to string

            foreach (KeyValuePair<node, node[]> entry in tree)
            {
                ValueArrayToString = string.Join("|", Array.ConvertAll(entry.Value, item => item.ToStringPrep())); //turning the value array into a string divided by '|'
                KeyToString = entry.Key.ToString(); //turning the Node to a string
                Tree.Add(KeyToString, ValueArrayToString); //adding a record but in string format
            }

            //Method to convert the Semantic Tree into a String
            string TreeString = string.Join(Environment.NewLine, Tree);

            return TreeString;
        }
    }
}
