using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICreateUtility {

	public static void PlaceUIElementRoot (GameObject element, MenuCommand menuCommand = null)
	{
		GameObject parent = null;
		if(menuCommand!=null)
		    parent = menuCommand.context as GameObject;
		if (parent == null || parent.GetComponentInParent<Canvas> () == null) {
			parent = GetOrCreateCanvasGameObject ();
		}

		string uniqueName = GameObjectUtility.GetUniqueNameForSibling (parent.transform, element.name);
		element.name = uniqueName;
		Undo.RegisterCreatedObjectUndo (element, "Create " + element.name);
		Undo.SetTransformParent (element.transform, parent.transform, "Parent " + element.name);
		GameObjectUtility.SetParentAndAlign (element, parent);
		Selection.activeGameObject = element;
	}

	// Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
	static public GameObject GetOrCreateCanvasGameObject ()
	{
		GameObject selectedGo = Selection.activeGameObject;

		// Try to find a gameobject that is the selected GO or one if its parents.
		Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas> () : null;
		if (canvas != null && canvas.gameObject.activeInHierarchy)
			return canvas.gameObject;

		// No canvas in selection or its parents? Then use just any canvas..
		canvas = Object.FindObjectOfType (typeof(Canvas)) as Canvas;
		if (canvas != null && canvas.gameObject.activeInHierarchy)
			return canvas.gameObject;

		// No canvas in the scene at all? Then create a new one.
		return CreateNewUI ();
	}

	private static void CreateEventSystem (bool select, GameObject parent)
	{
		var esys = Object.FindObjectOfType<EventSystem> ();
		if (esys == null) {
			var eventSystem = new GameObject ("EventSystem");
			GameObjectUtility.SetParentAndAlign (eventSystem, parent);
			esys = eventSystem.AddComponent<EventSystem> ();
			eventSystem.AddComponent<StandaloneInputModule> ();

			Undo.RegisterCreatedObjectUndo (eventSystem, "Create " + eventSystem.name);
		}

		if (select && esys != null) {
			Selection.activeGameObject = esys.gameObject;
		}
	}

	private static void CreateEventSystem (bool select)
	{
		CreateEventSystem (select, null);
	}

	static public GameObject CreateNewUI ()
	{
		//Root for the UI
		var root = new GameObject ("Canvas");
		root.layer = LayerMask.NameToLayer ("UI");
		Canvas canvas = root.AddComponent<Canvas> ();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		CanvasScaler canvasScaler = root.AddComponent<CanvasScaler> ();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		string[] res = UnityStats.screenRes.Split('x');
		canvasScaler.referenceResolution = new Vector2(int.Parse(res[0]),int.Parse(res[1]));
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.matchWidthOrHeight = 0;
		root.AddComponent<GraphicRaycaster> ();
		Undo.RegisterCreatedObjectUndo (root, "Create " + root.name);
		// if there is no event system add one...
		CreateEventSystem (false);
		return root;
	}

}
