using UnityEngine;

namespace Raele.Util
{
    /// <summary>
    /// Scriptable objects that inherit from SingletonScriptableObject are guaranteed to exist only a single instance in
    /// the scene. A new instace of the object is only allowed to exist after the previous instance is disabled, most
    /// likely due to being destroyed or unloaded.
    ///
    /// A static instance is not provided by this class, so you still have to assign the one asset instance to the mono
    /// behaviour or scriptable object that uses it; if for any reason a second asset of this type is created (most
    /// likely due to someone mistakenly creating a second asset of this type), an error will occur as soon as both
    /// instances got loaded at the same time during runtime.
    /// </summary>
    public class SingletonScriptableObject : ScriptableObject
    {
        public const string DUPLICATE_INSTANCE_ERROR_MESSAGE = "Two objects of type {0} were loaded, but it were supposed to be a singleton. (inherits from SingletonScriptableObject). Did someone mistakenly created a asset of this type? Existing isntance is: [{1}]. The new instance is: [{2}]. Deleting the new instance from the scene for now. This may cause unpredicted behaviour.";

        private static SingletonScriptableObject s_instance;

        private void OnEnable()
        {
            if (s_instance != null)
            {
                if (s_instance != this)
                {
                    Debug.LogErrorFormat(DUPLICATE_INSTANCE_ERROR_MESSAGE, this.GetType(), s_instance.name, this.name);
                    Destroy(this);
                }
            }
            else
            {
                s_instance = this;
            }
        }

        private void OnDisable()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }
    }
}