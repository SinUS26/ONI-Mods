using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ModifyTelepadPackages
{
    public static class GameObject_Extensions
    {
        public static string GetName(this GameObject obj)
        {
            if ((UnityEngine.Object)obj == (UnityEngine.Object)null)
                return "null object";
            try
            {
                return obj.name;
            }
            catch (Exception ex)
            {
                return "disposed object";
            }
        }

        public static void ResetAnchoredPosition(this GameObject go) => go.rectTransform().anchoredPosition = Vector2.zero;

        public static Transform Parent(this GameObject go) => go.transform.parent;

        public static GameObject ParentGO(this GameObject go) => go.transform.parent.gameObject;

        public static bool HasParent(this GameObject go, GameObject parent)
        {
            if ((UnityEngine.Object)go == (UnityEngine.Object)null || (UnityEngine.Object)go == (UnityEngine.Object)parent || ((UnityEngine.Object)go.transform.parent == (UnityEngine.Object)null || (UnityEngine.Object)parent == (UnityEngine.Object)null))
                return false;
            return (UnityEngine.Object)go.transform.parent == (UnityEngine.Object)parent || go.transform.parent.gameObject.HasParent(parent);
        }

        public static void CopyAnchorAlignment(this GameObject go, GameObject origin)
        {
            RectTransform orAddComponent1 = go.FindOrAddComponent<RectTransform>();
            RectTransform orAddComponent2 = origin.FindOrAddComponent<RectTransform>();
            orAddComponent1.anchoredPosition = orAddComponent2.anchoredPosition;
            orAddComponent1.anchorMin = orAddComponent2.anchorMin;
            orAddComponent1.anchorMax = orAddComponent2.anchorMax;
            orAddComponent1.pivot = orAddComponent2.pivot;
            orAddComponent1.localScale = orAddComponent2.localScale;
        }

        public static List<GameObject> GetChildren(this GameObject obj)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            if ((UnityEngine.Object)obj != (UnityEngine.Object)null)
            {
                for (int index = 0; index < obj.transform.childCount; ++index)
                    gameObjectList.Add(obj.transform.GetChild(index).gameObject);
            }
            return gameObjectList;
        }

        public static void SetAnchoredPosition(
          this GameObject go,
          Vector2 anchoredPosition,
          Vector2 anchorMin,
          Vector2 anchorMax)
        {
            RectTransform rectTransform = go.rectTransform();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        public static void SetAnchoredPosition(
          this GameObject go,
          Vector2 anchoredPosition,
          Vector2 anchorMin,
          Vector2 anchorMax,
          Vector2 pivot)
        {
            RectTransform rectTransform = go.rectTransform();
            go.SetAnchoredPosition(anchoredPosition, anchorMin, anchorMax);
            Vector2 vector2 = pivot;
            rectTransform.pivot = vector2;
        }

        public static void PrintChildrenOnce(this GameObject go, bool printComponents = true) => go.PrintChildren(printComponents, once: true);

        public static void PrintChildren(
          this GameObject go,
          bool printComponents = true,
          int offset = 0,
          bool once = false)
        {
            if (offset == 0)
                go.PrintComponents(offset);
            for (int index = 0; index < go.transform.childCount; ++index)
            {
                GameObject gameObject = go.transform.GetChild(index).gameObject;
                Logger.Print(string.Format("{0}child {1}: {2} {3} (parent: {4} {5})", (object)new string('\t', offset), (object)index, (object)gameObject.name, (object)gameObject.GetType(), (object)gameObject.transform.parent.name, (object)gameObject.transform.parent.GetType()));
                if (printComponents)
                {
                    gameObject.PrintComponents(offset);
                }
                else
                {
                    gameObject.PrintAnchoredPosition(offset);
                    gameObject.PrintLayout(offset);
                    gameObject.LayoutGroup<HorizontalLayoutGroup>(offset);
                    gameObject.LayoutGroup<VerticalLayoutGroup>(offset);
                }
                if (!once)
                    gameObject.PrintChildren(printComponents, offset++);
                Logger.Print("");
            }
        }

        public static void PrintHeight(this GameObject go)
        {
            LayoutElement component = go.GetComponent<LayoutElement>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                Logger.Print(string.Format("preferredHeight {0} minHeight {1} flexibleHeight {2}", (object)component.preferredHeight, (object)component.minHeight, (object)component.flexibleHeight));
            else
                Logger.Print("no LayoutElement");
        }

        public static void PrintComponents(this GameObject go, int offset = 0)
        {
            string str = new string('\t', offset);
            Logger.Print(string.Format("{0}go: {1} {2}", (object)str, (object)go.name, (object)go.GetType()));
            go.PrintAnchoredPosition(offset);
            go.PrintLayout(offset);
            go.LayoutGroup<HorizontalLayoutGroup>(offset);
            go.LayoutGroup<VerticalLayoutGroup>(offset);
            foreach (Component component in go.transform.GetComponents<Component>())
                Logger.Print(string.Format("{0}component: {1} {2}", (object)str, (object)component.name, (object)component.GetType()));
        }

        public static void PrintAnchoredPosition(this GameObject go, int offset = 0)
        {
            string str = new string('\t', offset);
            RectTransform rectTransform = go.rectTransform();
            if ((UnityEngine.Object)rectTransform == (UnityEngine.Object)null)
                Logger.Print(str + "No rect transform");
            else
                Logger.Print(string.Format("{0}anchoredPosition {1} anchorMin {2} anchorMax {3} pivot {4} sizeDelta {5} position {6}", (object)str, (object)rectTransform.anchoredPosition, (object)rectTransform.anchorMin, (object)rectTransform.anchorMax, (object)rectTransform.pivot, (object)rectTransform.sizeDelta, (object)rectTransform.position));
        }

        public static void LayoutGroup<T>(this GameObject go, int offset = 0) where T : HorizontalOrVerticalLayoutGroup
        {
            T component = go.GetComponent<T>();
            string str = new string('\t', offset);
            if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                return;
            Logger.Print(string.Format("{0}{1} childAlignment {2} layoutPriority {3} ", (object)str, (object)typeof(T), (object)component.childAlignment, (object)component.layoutPriority) + string.Format("childControlHeight {0} childControlWidth {1} ", (object)component.childControlHeight, (object)component.childControlWidth) + string.Format("childForceExpandHeight {0} childForceExpandWidth {1} ", (object)component.childForceExpandHeight, (object)component.childForceExpandWidth) + string.Format("flexibleHeight {0} flexibleWidth {1} ", (object)component.flexibleHeight, (object)component.flexibleWidth) + string.Format("minHeight {0} minWidth {1} ", (object)component.minHeight, (object)component.minWidth) + string.Format("preferredHeight {0} preferredWidth {1} ", (object)component.preferredHeight, (object)component.preferredWidth) + string.Format("spacing {0} padding {1}", (object)component.spacing, (object)component.padding));
        }

        public static void PrintLayout(this GameObject go, int offset = 0)
        {
            LayoutElement component = go.GetComponent<LayoutElement>();
            string str = new string('\t', offset);
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                Logger.Print(string.Format("{0}LayoutElement minWidth {1} minHeight {2} preferredWidth {3} preferredHeight {4} flexibleWidth {5} flexibleHeight {6}", (object)str, (object)component.minWidth, (object)component.minHeight, (object)component.preferredWidth, (object)component.preferredHeight, (object)component.flexibleWidth, (object)component.flexibleHeight));
            else
                Logger.Print(str + "no LayoutElement");
        }

        public static void PrintComponents(this GameObject go)
        {
            Logger.Print(string.Format("go: {0} {1}", (object)go.name, (object)go.GetType()));
            foreach (Component component in go.transform.GetComponents<Component>())
                Logger.Print(string.Format("component: {0} {1}", (object)component.name, (object)component.GetType()));
        }

        public static bool HaveChild(this GameObject go, string name) => (UnityEngine.Object)go.GetChild(name) != (UnityEngine.Object)null;

        public static GameObject GetChild(this GameObject go, string name)
        {
            for (int index = 0; index < go.transform.childCount; ++index)
            {
                Transform child = go.transform.GetChild(index);
                if (child.gameObject.name == name)
                    return child.gameObject;
            }
            return (GameObject)null;
        }

        public static GameObject FindChildWithName(this GameObject comp, string name)
        {
            for (int index = 0; index < comp.transform.childCount; ++index)
            {
                Transform child = comp.transform.GetChild(index);
                if (child.name == name)
                    return child.gameObject;
            }
            for (int index = 0; index < comp.transform.childCount; ++index)
            {
                GameObject childWithName = comp.transform.GetChild(index).gameObject.FindChildWithName(name);
                if ((UnityEngine.Object)childWithName != (UnityEngine.Object)null)
                    return childWithName;
            }
            return (GameObject)null;
        }

        public static GameObject FindParentWithName(this GameObject go, string name)
        {
            for (Transform transform = go.transform; (UnityEngine.Object)transform.parent != (UnityEngine.Object)null; transform = transform.parent.transform)
            {
                if (transform.parent.name == name)
                    return transform.parent.gameObject;
            }
            return (GameObject)null;
        }

        public static string GetEntityName(this GameObject go)
        {
            KSelectable component = go.GetComponent<KSelectable>();
            return (UnityEngine.Object)component == (UnityEngine.Object)null ? string.Empty : component.GetName();
        }

        public static Chore GetCurrentChore(this GameObject go) => go.gameObject.GetComponent<ChoreConsumer>().choreDriver.GetCurrentChore();
    }
}
