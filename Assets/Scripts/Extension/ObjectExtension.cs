using System;
using UnityEngine;

namespace BlueNoah.Utility
{
    public static class ComponentExtension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
            }
            return t;
        }

        public static Component GetOrAddComponent(this GameObject go, System.Type type)
        {
            Component comp = go.GetComponent(type);
            if (comp == null)
            {
                comp = go.AddComponent(type);
            }
            return comp;
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T t = component.gameObject.GetComponent<T>();
            if (t == null)
            {
                t = component.gameObject.AddComponent<T>();
            }
            return t;
        }

        public static void Find<T>(this Transform transform, string path, ref T comp, bool isForce = false) where T : Component
        {
            if (comp == null || isForce)
            {
                Debug.Log(path);
                comp = transform.Find(path).GetComponent<T>();
            }
        }

        public static void GetPath(this Transform transform, Component comp, ref string resultPath)
        {
            if (comp == null)
                return;
            string path = "/" + comp.gameObject.name;
            Transform trans = transform;
            GameObject obj = comp.gameObject;
            while (obj.transform.parent != null)
            {
                if (trans == obj.transform.parent)
                {
                    break;
                }
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            path = path.Remove(0, 1);
            resultPath = path;
        }
    }
}

