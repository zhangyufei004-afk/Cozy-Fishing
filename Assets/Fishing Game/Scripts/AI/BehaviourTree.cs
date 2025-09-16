using UnityEngine;

namespace FishingGame.AI
{
    /// <summary>
    /// Behaviour Tree class to represent a Behaviour Tree in Unity. The class stores the root node (and each node has reference
    /// to its own children). The Tree is setup on Start(), and executes every Update and Fixed Update.
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        #region Private Fields

        private TreeNode _root;

        #endregion

        protected void Start()
        {
            _root = SetupTree();
            _root.Initialize();
        }

        protected virtual void Update()
        {
            _root?.RunNode(); // Null Propagation
        }

        protected virtual void FixedUpdate()
        {
            _root?.RunPhysics();
        }

        protected abstract TreeNode SetupTree();
    }
}