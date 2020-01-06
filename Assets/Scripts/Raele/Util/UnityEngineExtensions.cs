using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Raele.Util
{
    public static class UnityEngineExtensions
    {
        public const string LOG_MESSAGE_WITH_CONTEXT = "{4}: '{2}' (of type '{3}')\n\nLogged by GameObject named '{0}' (component {1})\n";

        private static EventSystem EventSystemComponent = null;
        
		// -------------------------------------------------------------------------------------------------------------
		// VECTOR 3
		// -------------------------------------------------------------------------------------------------------------

        public static readonly Vector3 Vector3_XY = Vector3.right + Vector3.up;
        public static readonly Vector3 Vector3_XZ = Vector3.right + Vector3.forward;
        public static readonly Vector3 Vector3_YZ = Vector3.up + Vector3.forward;

        public static Vector3 Multiply(this Vector3 vector, Vector3 other)
            => Vector3.Scale(vector, other);

        public static Vector3 Floor(this Vector3 vector)
            => new Vector3(
                Mathf.Floor(vector.x),
                Mathf.Floor(vector.y),
                Mathf.Floor(vector.z)
            );

        public static Vector2 XZTo2D(this Vector3 source)
            => new Vector2(source.x, source.z);

        public static Vector2 XYTo2D(this Vector3 source)
            => new Vector2(source.x, source.y);

        public static Vector2 ZYTo2D(this Vector3 source)
            => new Vector2(source.z, source.y);

        public static Vector3 To3DXZ(this Vector2 source, float y = 0.0f)
            => new Vector3(source.x, y, source.y);

        public static Vector3 To3DXY(this Vector2 source, float z = 0.0f)
            => new Vector3(source.x, source.y, z);

        public static Vector3 To3DZY(this Vector2 source, float x = 0.0f)
            => new Vector3(x, source.y, source.x);
        
		// -------------------------------------------------------------------------------------------------------------
		// GAME OBJECT
		// -------------------------------------------------------------------------------------------------------------

		public static GameObject InstantiateThis(this GameObject prototype, Transform parent = null, bool worldSpace = false)
		{
			return GameObject.Instantiate(prototype, parent, worldSpace);
		}

		public static GameObject InstantiateThis(this GameObject prototype, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
		{
			return GameObject.Instantiate(prototype, position, rotation, parent);
		}

		public static GameObject WithName(this GameObject obj, string name, params object[] format)
		{
			obj.name = name.FormatWith(format);
			return obj;
		}

        public static T CopyTo<T>(this T original, GameObject target) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = target.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
        {
            if (gameObject != null)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    yield return gameObject.transform.GetChild(i).gameObject;
                }
            }
        }

        public static IEnumerable<GameObject> GetChildrenHierarchy(this GameObject gameObject)
        {
            List<GameObject> childLayer = gameObject.GetChildren().ToList();
            List<GameObject> result = new List<GameObject>();
            
            while (!childLayer.IsEmpty())
            {
                result.AddRange(childLayer);
                childLayer = childLayer.SelectMany(obj => obj.GetChildren()).ToList();
            }

            return result;
        }

        public static GameObject FindChildInHierarchyByName(this GameObject gameObject, string targetObjectName)
        {
            return gameObject.GetChildrenHierarchy()
                    .ToList()
                    .Find(obj => obj.name.Equals(targetObjectName));
        }

        public static GameObject FindChildInHierarchyByName(this GameObject gameObject, Regex targetObjectName)
        {
            return targetObjectName.IsDefault()
                ? null
                : gameObject.GetChildrenHierarchy()
                        .ToList()
                        .Find(obj => targetObjectName.IsMatch(obj.name));
        }

		// -------------------------------------------------------------------------------------------------------------
		// Debugging
		// -------------------------------------------------------------------------------------------------------------

        private static T BaseLog<T>(Action<string> logFunction, T obj, Component context, string message, object[] format)
        {
            logFunction(
                LOG_MESSAGE_WITH_CONTEXT.FormatWith(
                    context?.gameObject.name,
                    context?.GetType().Name,
                    obj,
                    typeof(T),
                    (message ?? "").FormatWith(format)
                )
            );
            return obj;
        }

        public static T Log<T>(this T obj, Component context, string message = "", params object[] format)
            => BaseLog(Debug.Log, obj, context, message, format);
        
        public static T LogWarning<T>(this T obj, Component context, string message = "", params object[] format)
            => BaseLog(Debug.LogWarning, obj, context, message, format);

        public static T LogError<T>(this T obj, Component context, string message = "", params object[] format)
            => BaseLog(Debug.LogError, obj, context, message, format);
        
		// -------------------------------------------------------------------------------------------------------------
		// RAYCASTING
		// -------------------------------------------------------------------------------------------------------------

        public static List<RaycastResult> Raycast(this GraphicRaycaster raycaster, Vector2 screenPosition, EventSystem eventSystem = null)
        {
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem es = UnityEngineExtensions.EventSystemComponent
                ?? (UnityEngineExtensions.EventSystemComponent = eventSystem)
                ?? (UnityEngineExtensions.EventSystemComponent = GameObject.FindObjectOfType<EventSystem>());
            PointerEventData eventData = new PointerEventData(es);
            eventData.position = screenPosition;
            raycaster.Raycast(eventData, result);
            return result;
        }

        public static Ray CreateRay(this Transform source)
            => new Ray(source.position, source.forward);

        public static RaycastHit? Raycast(this Transform source, float range = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
            => Physics.Raycast(source.CreateRay(), out RaycastHit result, range, layerMask)
                ? result
                : null as RaycastHit?;
    }
}
