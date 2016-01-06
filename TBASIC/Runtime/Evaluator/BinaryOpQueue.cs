using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Runtime
{
    internal class BinaryOperator : IComparable<BinaryOperator>
    {

        /// <summary>
        /// This method gets the precedence of a binary operator
        /// </summary>
        /// <param name="strOp"></param>
        /// <returns></returns>
        private static int OperatorPrecedence(string strOp)
        {
            switch (strOp) {
                case "*":
                case "/":
                case "MOD": return 0;
                case "+":
                case "-": return 1;
                case ">>":
                case "<<": return 2;
                case "<":
                case "=<":
                case "<=":
                case ">":
                case "=>":
                case ">=": return 3;
                case "==":
                case "=":
                case "<>":
                case "!=": return 4;
                case "&": return 5;
                case "^": return 6;
                case "|": return 7;
                case "AND": return 8;
                case "OR": return 9;
            }
            throw new ArgumentException("operator '" + strOp + "' not defined.");
        }

        public string OperatorString { get; private set; }
        public int Precedence { get; private set; }

        public BinaryOperator(string strOp)
        {
            OperatorString = strOp.ToUpper();
            Precedence = OperatorPrecedence(OperatorString);
        }

        public int CompareTo(BinaryOperator other)
        {
            return Precedence.CompareTo(other.Precedence);
        }
    }


    internal class UnaryOperator
    {
        public string OperatorString { get; private set; }

        public UnaryOperator(string strOp)
        {
            OperatorString = strOp;
        }
    }

    internal class BinaryOpQueue
    {
        private LinkedList<BinOpNodePair> _oplist = new LinkedList<BinOpNodePair>();

        public BinaryOpQueue(LinkedList<object> expressionlist)
        {
            LinkedListNode<object> i = expressionlist.First;
            while (i != null) {
                Enqueue(new BinOpNodePair(i));
                i = i.Next;
            }
        }

        public void Enqueue(BinOpNodePair nodePair)
        {
            if (nodePair.Operator == null) {
                return;
            }

            for (var currentNode = _oplist.First; currentNode != null; currentNode = currentNode.Next) {
                if (currentNode.Value.Operator.Precedence > nodePair.Operator.Precedence) {
                    _oplist.AddBefore(currentNode, nodePair);
                    return;
                }
            }
            _oplist.AddLast(nodePair);
        }

        public BinOpNodePair Dequeue()
        {
            if (_oplist.Count == 0) {
                return null;
            }
            BinOpNodePair ret = _oplist.First.Value;
            _oplist.RemoveFirst();
            return ret;
        }

        public int Count
        {
            get { return _oplist.Count; }
        }

        public class BinOpNodePair
        {
            private LinkedListNode<object> node;
            private BinaryOperator op;

            public BinaryOperator Operator
            {
                get
                {
                    return op;
                }
            }

            public LinkedListNode<object> Node
            {
                get
                {
                    return node;
                }
                set
                {
                    node = value;
                    op = node.Value as BinaryOperator;
                }
            }

            public BinOpNodePair(LinkedListNode<object> node)
            {
                Node = node;
            }
        }
    }

}
