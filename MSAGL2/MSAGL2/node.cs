using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSAGL2
{
    public class node
    {
        protected String Value; //value of the node, Example: Table, Mouse, Book, etc.

        protected List<node> Children = new List<node>();
        //objects that the node is related to, Example if the node is a table and their is a laptop on the table, the laptop will be a child of the table

        protected Boolean visited = false; //if the node has been visited or not
        protected string Preposition; //preposition of the node with its parent
        protected int Level; //number to show how many parents the node has
        //protected TreeNode Position; //the position of the node


        public node(String value)//constructor for the node object
        {
            Value = value;
        }

        public void addChild(node child, String preposition)
        {
            Children.Add(child);
            Preposition = preposition;
        }

        public override String ToString()
        {
            return Value;
        }

        public String ToStringPrep()
        {
            return Value + getPreposition();
        }

        public List<node> getChildren()
        {
            return Children;
        }

        public void setLevel(int level)
        {
            Level = level;
        }

        public int getLevel()
        {
            return Level;
        }

        //public void setPosition(TreeNode position)
        //{
        //    Position = position;
        //}
        public string getPreposition()
        {
            return "-" + Preposition;
        }

        public void setPreposition(string preposition)
        {
            Preposition = preposition;
        }
    }
}
