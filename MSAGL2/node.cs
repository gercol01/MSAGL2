namespace MSAGL2
{
    public class node
    {
        protected string Value; //value of the node, Example: Table, Mouse, Book, etc.

        protected string Preposition; //preposition of the node with its parent

        public override string ToString()
        {
            return Value;
        }

        public string ToStringPrep()
        {
            return Value + getPreposition();
        }

        public string getPreposition()
        {
            return "-" + Preposition;
        }
    }
}
