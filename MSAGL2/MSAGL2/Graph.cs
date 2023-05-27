using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
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

        //dict to store full names
        IDictionary<string, string> nameFullNameDict = new Dictionary<string, string>();

        //dict to store colors
        IDictionary<string, Microsoft.Msagl.Drawing.Color> colorDict = new Dictionary<string, Microsoft.Msagl.Drawing.Color>();

        public Graph(string tree) //constructor for Unity
        {
            InitializeComponent();

            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

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
                //tidy up the parent details
                string[] parentArray = entry.Key.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

                //parent-----

                //getting the name of the key, Ex; table_1
                string parentName = parentArray[0];

                //add the name to the dropdown
                comboBox1.Items.Add(parentName);

                //coordinates
                string[] coordinatesParent = parentArray[1].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string coordsX = coordinatesParent[0];
                string coordsY = coordinatesParent[1];
                string coordsZ = coordinatesParent[2];

                float X = float.Parse(coordsX);
                float Y = float.Parse(coordsY);
                float Z = float.Parse(coordsZ);

                coordsX = X + "x";
                coordsY = Y + "y";
                coordsZ = Z + "z";

                //rotations
                string[] rotationsParent = parentArray[3].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string rotationX = rotationsParent[0] + "° x";
                string rotationY = rotationsParent[1] + "° y";
                string rotationZ = rotationsParent[2] + "° z";

                //name, coordinates and rotation
                string nameCoordsRotationParent = parentName + "\n"
                    + coordsX + ", " + coordsY + ", " + coordsZ + "\n"
                    + rotationX + ", " + rotationY + ", " + rotationZ;

                //add name and full name to dictionary
                nameFullNameDict.Add(parentName,nameCoordsRotationParent);

                //if the node does not have children, there is no need for an edge
                if (entry.Value != "")
                {
                    //split the values into an array of values by the '|' and ' ' characters
                    string[] objectsWithPreposition = entry.Value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    //split the objects from the prepositions
                    foreach (string obj in objectsWithPreposition)
                    {
                        //split of the object from the preposition from the '-' character
                        string[] childArray = obj.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

                        //child-----

                        //getting the name of the key, Ex; table_1
                        string childName = childArray[0];

                        //coordinates
                        string[] coordinatesChild = childArray[1].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string coordsx = coordinatesChild[0];
                        string coordsy = coordinatesChild[1];
                        string coordsz = coordinatesChild[2];

                        float x = float.Parse(coordsx);
                        float y = float.Parse(coordsy);
                        float z = float.Parse(coordsz);

                        coordsx = x + "x";
                        coordsy = y + "y";
                        coordsz = z + "z";

                        //rotations
                        string[] rotationsChild = childArray[3].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string rotationx = rotationsChild[0] + "° x";
                        string rotationy = rotationsChild[1] + "° y";
                        string rotationz = rotationsChild[2] + "° z";

                        //name, coordinates and rotation
                        string nameCoordsRotationChild = childName + "\n"
                            + coordsx + ", " + coordsy + ", " + coordsz + "\n"
                            + rotationx + ", " + rotationy + ", " + rotationz;

                        //label text
                        string labelText = childArray[2] + ", " + childArray[4];

                        //creating the nodes
                        Microsoft.Msagl.Drawing.Node parentNode = graph.AddNode(nameCoordsRotationParent);
                        Microsoft.Msagl.Drawing.Node childNode = graph.AddNode(nameCoordsRotationChild);

                        parentNode.Label.FontSize = 14;

                        parentNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
                        parentNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;

                        //if value already exist in the dict
                        if (colorDict.ContainsKey(nameCoordsRotationParent))
                        {
                            colorDict[nameCoordsRotationParent] = Microsoft.Msagl.Drawing.Color.LightBlue;
                        }
                        else
                        {
                            //add a new record
                            colorDict.Add(nameCoordsRotationParent, Microsoft.Msagl.Drawing.Color.LightBlue);
                        }

                        //check if the childNode already exists
                        if (graph.FindNode(nameCoordsRotationParent) == null)
                        {
                            childNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                            childNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;

                            //if value already exist in the dict
                            if (colorDict.ContainsKey(childName))
                            {
                                colorDict[childName] = Microsoft.Msagl.Drawing.Color.PaleGreen;
                            }
                            else
                            {
                                //add a new record
                                colorDict.Add(childName, Microsoft.Msagl.Drawing.Color.PaleGreen);
                            }
                        }

                        childNode.Label.FontSize = 14;

                        //Add an edge to the graph
                        Microsoft.Msagl.Drawing.Edge edge = graph.AddEdge(nameCoordsRotationParent, nameCoordsRotationChild);

                        //change edge label
                        edge.LabelText = labelText;

                        //change the arrowhead direction
                        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
                        edge.Attr.ArrowheadAtSource = ArrowStyle.Normal;
                    }
                }
                else
                {
                    //if it has no children, only the node is created
                    Microsoft.Msagl.Drawing.Node parentNode = graph.AddNode(nameCoordsRotationParent);

                    parentNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                    parentNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;
                    parentNode.Label.FontSize = 14;

                    //if value already exist in the dict
                    if (colorDict.ContainsKey(nameCoordsRotationParent))
                    {
                        colorDict[nameCoordsRotationParent] = Microsoft.Msagl.Drawing.Color.PaleGreen;
                    }
                    else
                    {
                        //add a new record
                        colorDict.Add(nameCoordsRotationParent, Microsoft.Msagl.Drawing.Color.PaleGreen);
                    }

                    //graph.AddNode(nameCoordsRotationParent);
                }

            }

            //set the selected item
            comboBox1.SelectedIndex = 0;

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

            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            //TreeString = "[cube_1,]";
            //TreeString = "[Gerard,Chair-on|Book-under|Laptop-right_of]&[Chair,Box-left_of]&[Book,Cat-left_of|Mouse-behind|Stapler-infront]&[Laptop,Monitor-on]&[Box,Man-under]&[Cat ]&[Mouse,Cheese-on]&[Stapler,]&[Monitor,]&[Man,]&[Cheese,]";
            TreeString = "[table_1*0v0.785v0*null*0v0v0*null,cup_1*0v1.771055v0*on*0v0v0*0]&[cup_1*0v1.771055v0*on*0v0v0*0,]";

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

            //TreeDataStructureString = TreeString.Split(new[] { '[', ']', '&' }, StringSplitOptions.RemoveEmptyEntries)//the characters which will be removed from the array
            //   .Select(part => part.Split(',')) //divide the arrays to keys & values by the ',' character
            //   .ToDictionary(split => split[0], split => split[1]);

            ////characters that will split the String
            //char[] delimiters = new[] { '[', ']', '&' };

            ////split the string into records
            //string[] recordStrings = TreeString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            //                            .Select(part => part.Trim())
            //                            .Where(part => !string.IsNullOrEmpty(part))
            //                            .ToArray();

            ////split the string into key and value(s) into a dictionary with the ',' symbol
            //TreeDataStructureString = recordStrings.Select(s => s.Split(',')) 
            //                             .ToDictionary(s => s[0], s => s[1]);


            ////iterate through the dictionary
            //foreach (KeyValuePair<string, string> entry in TreeDataStructureString)
            //{
            //    //tidy up the parent
            //    string[] parentArray = entry.Key.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            //    //parent-----

            //    //getting the name of the key, Ex; table_1
            //    string parentName = parentArray[0];

            //    //add the name to the dropdown
            //    comboBox1.Items.Add(parentName);

            //    //coordinates
            //    string[] coordinatesParent = parentArray[1].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    string coordsX = coordinatesParent[0];
            //    string coordsY = coordinatesParent[1];
            //    string coordsZ = coordinatesParent[2];

            //    float X = float.Parse(coordsX);
            //    float Y = float.Parse(coordsY);
            //    float Z = float.Parse(coordsZ);

            //    coordsX = X + "x";
            //    coordsY = Y + "y";
            //    coordsZ = Z + "z";

            //    //rotations
            //    string[] rotationsParent = parentArray[3].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    string rotationX = rotationsParent[0] + "° x";
            //    string rotationY = rotationsParent[1] + "° y";
            //    string rotationZ = rotationsParent[2] + "° z";

            //    //name, coordinates and rotation
            //    string nameCoordsRotationParent = parentName + "\n"
            //        + coordsX + ", " + coordsY + ", " + coordsZ + "\n"
            //        + rotationX + ", " + rotationY + ", " + rotationZ;

            //    //add name and full name to dictionary
            //    nameFullNameDict.Add(parentName, nameCoordsRotationParent);

            //    //if the node does not have children, there is no need for an edge
            //    if (entry.Value != "")
            //    {
            //        //split the values into an array of values by the '|' and ' ' characters
            //        string[] objectsWithPreposition = entry.Value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //        //split the objects from the prepositions
            //        foreach (string obj in objectsWithPreposition)
            //        {
            //            //split of the object from the preposition from the '-' character
            //            string[] childArray = obj.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            //            //child-----

            //            //getting the name of the key, Ex; table_1
            //            string childName = childArray[0];

            //            //coordinates
            //            string[] coordinatesChild = childArray[1].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //            string coordsx = coordinatesChild[0];
            //            string coordsy = coordinatesChild[1];
            //            string coordsz = coordinatesChild[2];

            //            float x = float.Parse(coordsx);
            //            float y = float.Parse(coordsy);
            //            float z = float.Parse(coordsz);

            //            coordsx = x + "x";
            //            coordsy = y + "y";
            //            coordsz = z + "z";

            //            //rotations
            //            string[] rotationsChild = childArray[3].Split(new[] { 'v', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //            string rotationx = rotationsChild[0] + "° x";
            //            string rotationy = rotationsChild[1] + "° y";
            //            string rotationz = rotationsChild[2] + "° z";

            //            //name, coordinates and rotation
            //            string nameCoordsRotationChild = childName + "\n"
            //                + coordsx + ", " + coordsy + ", " + coordsz + "\n"
            //                + rotationx + ", " + rotationy + ", " + rotationz;

            //            //label text
            //            string labelText = childArray[2] + ", " + childArray[4];

            //            //creating the nodes
            //            Microsoft.Msagl.Drawing.Node parentNode = graph.AddNode(nameCoordsRotationParent);
            //            Microsoft.Msagl.Drawing.Node childNode = graph.AddNode(nameCoordsRotationChild);

            //            parentNode.Label.FontSize = 14;

            //            parentNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
            //            parentNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;

            //            //if value already exist in the dict
            //            if (colorDict.ContainsKey(nameCoordsRotationParent))
            //            {
            //                colorDict[nameCoordsRotationParent] = Microsoft.Msagl.Drawing.Color.LightBlue;
            //            }
            //            else
            //            {
            //                //add a new record
            //                colorDict.Add(nameCoordsRotationParent, Microsoft.Msagl.Drawing.Color.LightBlue);
            //            }


            //            //check if the childNode already exists
            //            if (graph.FindNode(nameCoordsRotationParent) == null) {
            //                childNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //                childNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;

            //                //if value already exist in the dict
            //                if (colorDict.ContainsKey(childName))
            //                {
            //                    colorDict[childName] = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //                }
            //                else {
            //                    //add a new record
            //                    colorDict.Add(childName, Microsoft.Msagl.Drawing.Color.PaleGreen);
            //                }
            //            }

            //            childNode.Label.FontSize = 14;

            //            //Add an edge to the graph
            //            Microsoft.Msagl.Drawing.Edge edge = graph.AddEdge(nameCoordsRotationParent, nameCoordsRotationChild);

            //            //change edge label
            //            edge.LabelText = labelText;

            //            //change the arrowhead direction
            //            edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
            //            edge.Attr.ArrowheadAtSource = ArrowStyle.Normal;
            //        }
            //    }
            //    else {
            //        //if it has no children, only the node is created
            //        Microsoft.Msagl.Drawing.Node parentNode = graph.AddNode(nameCoordsRotationParent);

            //        parentNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //        parentNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;
            //        parentNode.Label.FontSize = 14;

            //        //if value already exist in the dict
            //        if (colorDict.ContainsKey(nameCoordsRotationParent))
            //        {
            //            colorDict[nameCoordsRotationParent] = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //        }
            //        else
            //        {
            //            //add a new record
            //            colorDict.Add(nameCoordsRotationParent, Microsoft.Msagl.Drawing.Color.PaleGreen);
            //        }

            //        graph.AddNode(nameCoordsRotationParent);
            //    }

            //}
            ////graph.FindNode("Gerard").GeometryNode.Center = new Microsoft.Msagl.Core.Geometry.Point(100, 100);

            ////var n = graph.FindNode("Gerard");
            ////n.Label.FontSize = 2;
            ////n.GeometryNode.Center = new Point(0, 0);

            ////set the selected item
            //comboBox1.SelectedIndex = 0;

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

        //method called after pressing search
        private void button1_Click(object sender, EventArgs e)
        {
            //the chosen option in the dropdown
            try
            {
                if (comboBox1.SelectedItem != null) {
                    string option = comboBox1.SelectedItem.ToString();

                    //remove spaces
                    option = option.Replace(" ", "");

                    //make it lower case
                    option = option.ToLower();

                    string fullname = null;

                    try
                    {
                        //store the fullname
                        fullname = nameFullNameDict[option];

                        //reset all the nodes to how they should be 
                        foreach (KeyValuePair<string, Microsoft.Msagl.Drawing.Color> entry in colorDict)
                        {
                            //find the node
                            Microsoft.Msagl.Drawing.Node colorNode = graph.FindNode(entry.Key);

                            //change the color of the node
                            colorNode.Attr.FillColor = entry.Value;
                        }

                        //find the searched node
                        Microsoft.Msagl.Drawing.Node parentNode = graph.FindNode(fullname);

                        //change color to red
                        parentNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

                        //update view
                        viewer.Invalidate();
                    }
                    catch (KeyNotFoundException)
                    {
                        //print an error message
                        Console.WriteLine("Key not found");
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                // code to handle the exception
                Console.WriteLine("NullReferenceException caught: " + ex.Message);
            }
        }
    }
}