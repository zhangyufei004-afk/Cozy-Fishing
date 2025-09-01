using System.Collections.Generic;
using UnityEngine.UIElements;

namespace FishingGame.AI
{
    /// <summary>
    /// Enum representation of the state of a Tree Node.
    /// </summary>
    public enum ETreeNodeState : byte
    {
        Running,
        Success,
        Failure
    }
    
    /// <summary>
    /// Tree Node class representing a generic tree node. Tree Nodes can either be composites (like Selector, Sequence) or they can
    /// be concrete actions (leaf nodes).
    /// </summary>
    public class TreeNode
    {
        public TreeNode Parent => _parent;
        
        protected ETreeNodeState State;
        protected List<TreeNode> Children = new();
        
        private Dictionary<string, object> _data = new();
        private TreeNode _parent;
        
        public TreeNode()
        {
            _parent = null;
        }

        public TreeNode(List<TreeNode> children)
        {
            foreach (TreeNode treeNode in children)
            {
                Attach(treeNode);
            }
        }

        /// <summary>
        /// Attach this Behaviour Tree node to another Behaviour Tree Node, making this node the new parent.
        /// </summary>
        /// <param name="treeNode">The node to make the new child of this Node.</param>
        protected void Attach(TreeNode treeNode)
        {
            treeNode._parent = this;
            Children.Add(treeNode);
        }

        public virtual ETreeNodeState RunNode() => ETreeNodeState.Failure;

        public virtual ETreeNodeState RunPhysics() => ETreeNodeState.Failure;

        /// <summary>
        /// Sets Data in the Data Dictionary for this node and any of its children to access. Data can be stored at any node level,
        /// though convention is to store it as close to the node that needs it, to avoid unnecessary tree traversal.
        /// </summary>
        /// <param name="dataKey">The key for the Data, which will later be used to retrieve it.</param>
        /// <param name="value">The object we are storing against the dataKey</param>
        public void SetData(string dataKey, object value)
        {
            TreeNode nodeParent = Parent;
            TreeNode currentNode = this;
            while (nodeParent != null)
            {
                currentNode = nodeParent;
                nodeParent = nodeParent.Parent;
            }

            currentNode._data[dataKey] = value;
        }

        /// <summary>
        /// Get the Data stored under dataKey, in either this TreeNode or a Node up the Tree.
        /// </summary>
        /// <param name="dataKey">The Key for the Data we want to retrieve</param>
        /// <returns>The Data as a generic <c>object</c> type.</returns>
        public object GetData(string dataKey)
        {
            object value = null;
            if (_data.TryGetValue(dataKey, out value))
            {
                return value;
            }

            TreeNode treeNode = Parent;
            while (treeNode != null)
            {
                value = treeNode.GetData(dataKey);
                if (value != null)
                {
                    return value;
                }

                treeNode = treeNode.Parent;
            }

            return null;
        } 
        
        /// <summary>
        /// Removes the Data stored under <c>dataKey</c> from the Data dictionary.
        /// </summary>
        /// <param name="dataKey">The Key at which the data we want to remove is stored.</param>
        /// <returns>True if the Data was successfully removed, false otherwise</returns>
        public bool ClearData(string dataKey)
        {
            bool removedData = false;
            if (_data.ContainsKey(dataKey))
            {
                removedData = _data.Remove(dataKey);
                return removedData;
            }

            TreeNode treeNode = Parent;
            while (treeNode != null)
            {
                removedData = treeNode.ClearData(dataKey);
                if (removedData)
                {
                    return removedData;
                }

                treeNode = treeNode.Parent;
            }

            return false;
        }
        
        private bool TryGetData(string dataKey, out object outputObject)
        {
            outputObject = GetData(dataKey);
            return outputObject != null;
        }
    }
}