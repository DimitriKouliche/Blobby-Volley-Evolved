using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

namespace Cubequad.Tentacles2D
{
    [InitializeOnLoad]
    public static class TentacleEditorHelper
    {
        private static GameObject tentacle;
        public static bool IsNewUnityVersion { get; private set; }

        static TentacleEditorHelper()
        {
            LoadTentacle();
            IsNewUnityVersion = IsVersionNewerThan();
        }

        private static void LoadTentacle()
        {
            var pathes = System.IO.Directory.GetFiles(Application.dataPath, "Tentacle.prefab", System.IO.SearchOption.AllDirectories);
            if (pathes.Length < 1)
                Debug.Log("Tentacle.prefab file wasn't found, the Tools->Tentacles2D->AddTentacle won't work.");
            else
            {
                var split = pathes[0].Split(new string[] { "Assets" }, StringSplitOptions.None);
                var path = $"Assets{split[split.Length - 1].Replace("\\", "/")}";
                tentacle = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            }
        }

        [MenuItem("Tools/Tentacles2D/Add Tentacle")]
        private static void AddTentacle()
        {
            if (tentacle != null)
            {
                var clone = (GameObject)PrefabUtility.InstantiatePrefab(tentacle, EditorSceneManager.GetActiveScene());
                clone.name = clone.name.Replace("(Clone)", string.Empty);
                clone.transform.SetAsLastSibling();
                clone.transform.position = SceneView.lastActiveSceneView == null ? Vector3.zero : new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0);
                Undo.RegisterCreatedObjectUndo(clone, "New Tentacle Spawned");
                EditorGUIUtility.PingObject(clone);
                //EditorSceneManager.OpenScene(activeScene.path);
                //EditorApplication.delayCall += delegate { FixUpdate(clone); };
                //SceneView.RepaintAll();
                //EditorWindow.GetWindow<SceneView>().Repaint();
                //UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                //SceneView.lastActiveSceneView.Repaint();
            }
            else
                LoadTentacle();
        }

        private static bool IsVersionNewerThan(float version = 2018.3f)
        {
            var split = Application.unityVersion.Split('.');
            var installedVersion = Convert.ToSingle($"{split[0]},{split[1]}");
            if (installedVersion >= version) return true;
            else return false;
        }

        //private static void FixUpdate(GameObject clone)
        //{
        //    Debug.Log("delayed call");
        //    clone.transform.position = Vector3.zero;
        //    SceneView.RepaintAll();
        //    EditorWindow.GetWindow<SceneView>().Repaint();
        //    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        //}
    }

    [CustomEditor(typeof(Tentacle)), CanEditMultipleObjects]
    public class TentacleEditor : Editor
    {
        private SerializedProperty material, color, textureType, smoothness, pivotCapSmoothness, tipCapSmoothness, width, shape, reduction, tentacleTarget, tentacleTargetRigidbody, speed, animation, frequency, amplitude, animationDelay;
        private SerializedProperty debugOutline, debugTriangles, debugUVs;
        private Texture2D logoTexture, bgTexture;
        private TentacleData[] tentacleData;

        private GUIStyle header;
        private string[] sortingLayerNames;
        private PropertyModification[] modifications;

        private void OnEnable()
        {
            //GenerateTentacle();

            logoTexture = LoadTexture("logo.png");
            bgTexture = LoadTexture("bg.png");

            color = serializedObject.FindProperty("color");
            material = serializedObject.FindProperty("material");
            textureType = serializedObject.FindProperty("textureType");
            smoothness = serializedObject.FindProperty("smoothness");
            pivotCapSmoothness = serializedObject.FindProperty("pivotCapSmoothness");
            tipCapSmoothness = serializedObject.FindProperty("tipCapSmoothness");
            width = serializedObject.FindProperty("width");
            shape = serializedObject.FindProperty("shape");
            reduction = serializedObject.FindProperty("reduction");
            tentacleTarget = serializedObject.FindProperty("target");
            tentacleTargetRigidbody = serializedObject.FindProperty("targetRigidbody");
            speed = serializedObject.FindProperty("speed");
            animation = serializedObject.FindProperty("animation");
            frequency = serializedObject.FindProperty("frequency");
            amplitude = serializedObject.FindProperty("amplitude");
            animationDelay = serializedObject.FindProperty("animationDelay");

            debugOutline = serializedObject.FindProperty("debugOutline");
            debugTriangles = serializedObject.FindProperty("debugTriangles");
            debugUVs = serializedObject.FindProperty("debugUVs");

            CreateTentacleData();
            
            InitializeSortingLayers();
            InitializeStyles();

            EditorApplication.update += RenewTarget;
            Undo.undoRedoPerformed += RenewEditor;
        }

        private void OnDisable()
        {
            EditorApplication.update -= RenewTarget;
            Undo.undoRedoPerformed -= RenewEditor;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTexture(logoTexture);

            DrawHeader("Rendering", "This section includes all visual settings.");
            DrawRenderingControls();

            DrawHeader("Colliders", "This section includes all settings for the colliders and their types.");
            DrawColliderControls();

            DrawHeader("Behaviour", "This section includes all settings for physics and AI.");
            DrawBehaviourControls();

            DrawHeader("Debug", "For debug purposes only, this will not be rendered.");
            DrawDebugControls();

            serializedObject.ApplyModifiedProperties();
        }

        //private void GenerateTentacle()
        //{
        //    for (int i = 0; i < targets.Length; i++)
        //    {
        //        var tentacle = (Tentacle)targets[i];
        //        if (tentacle.GetComponents<Component>().Length == 1 && tentacle.transform.childCount == 0)
        //        {
        //            var pivot = CreateSegment(tentacle.gameObject, "Pivot");
        //            CreateMeshRenderer(pivot);
        //            CreateRigidbody(pivot, RigidbodyType2D.Static);
        //            CreateCircleCollider(pivot);

        //            var segments = new Rigidbody2D[2];
        //            for (int j = 0; j < 2; j++)
        //            {
        //                var segment = CreateSegment(tentacle.gameObject, "Segment" + j);
        //                CreateRigidbody(segment);
        //                CreateCircleCollider(segment);
        //                CreateaSpringJoint(segment);
        //            }

        //            var tip = CreateSegment(tentacle.gameObject, "Tip");
        //            CreateRigidbody(tip);
        //            CreatePolygonCollider(tip);
        //            CreateCircleCollider(tip);
        //            CreateaSpringJoint(tip);
        //        }
        //    }
        //}

        //private GameObject CreateSegment(GameObject tentacle, string name)
        //{
        //    var segment = new GameObject($"{tentacle.name}{name}");
        //    segment.transform.parent = tentacle.transform;
        //    segment.transform.position = tentacle.transform.position; // TODO: that's crapy
        //    return segment;
        //}
        
        //private void CreateMeshRenderer(GameObject go)
        //{
        //    var meshRenderer = go.AddComponent<MeshRenderer>();
        //    meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        //    meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        //    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //    meshRenderer.receiveShadows = false;
        //    meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        //    meshRenderer.allowOcclusionWhenDynamic = false;

        //    go.AddComponent<MeshFilter>();
        //}

        //private void CreateRigidbody(GameObject go, RigidbodyType2D type = RigidbodyType2D.Dynamic)
        //{
        //    var rigidbody = go.AddComponent<Rigidbody2D>();
        //    rigidbody.bodyType = type;
        //}

        //private void CreateCircleCollider(GameObject go)
        //{
        //    var collider = go.AddComponent<CircleCollider2D>();
        //}

        //private void CreatePolygonCollider(GameObject go)
        //{
        //    var collider = go.AddComponent<PolygonCollider2D>();
        //}

        //private void CreateaSpringJoint(GameObject go)
        //{
        //    var joint = go.AddComponent<SpringJoint2D>();
        //}

        private void CreateTentacleData()
        {
            tentacleData = new TentacleData[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                tentacleData[i] = new TentacleData((Tentacle)targets[i]);
        }

        private void DrawRenderingControls()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(color, new GUIContent("Color", "Tint color of the mesh."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetColor(color.colorValue);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(material, new GUIContent("Material", "Will render the mesh with the material applied."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].meshRenderer.material = (Material)material.objectReferenceValue;

            EditorGUILayout.PropertyField(textureType, new GUIContent("Draw Mode", "Choose the method to set the UVs."));
            EditorGUILayout.IntSlider(smoothness, 4, 128, new GUIContent("Smoothness", "The number of segments of a mesh."));
            EditorGUILayout.IntSlider(pivotCapSmoothness, 0, 64, new GUIContent("Pivot Cap Smoothness", "Defines roundness of the mesh at the start of the tentacle. 0 = no cap."));
            EditorGUILayout.IntSlider(tipCapSmoothness, 0, 64, new GUIContent("Tip Cap Smoothness", "Defines roundness of the mesh at the end of the tentacle. 0 = no cap."));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(width, new GUIContent("Width", "The width multiplier of the tentacle's mesh."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].UpdateCircleColliders(width.floatValue, shape.animationCurveValue);

            DrawLength();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(shape, new GUIContent("Shape", "The shape of a mesh. To change width use Width field above."), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].UpdateCircleColliders(width.floatValue, shape.animationCurveValue);

            DrawSortingLayers();
            DrawSortingOrder();
        }

        private void DrawColliderControls()
        {
            DrawColliderType();

            DrawIsTrigger();

            var isPolyColliderEnabled = DrawPolygonalCollider();
            DrawReduction(isPolyColliderEnabled);
            DrawIsPolygonalTrigger(isPolyColliderEnabled);

            //if (type == TentacleData.ColliderType.polygonal && !isTrigger)
            //{
            //    EditorGUILayout.HelpBox("Procedural non-trigger polygon collider will not collide correctly, expect wierd behaviour. Use cirlce colliders instead or mark it as trigger.", MessageType.Warning);
            //}
        }

        private void DrawBehaviourControls()
        {
            DrawBodyType();
            DrawTarget();
            DrawCatchReleaseButtons();
            DrawAnimations();
            EditorGUILayout.PropertyField(speed, new GUIContent("Speed", "Gonna reach target with this force. Set None to disable."));
            DrawMass();
            DrawDrag();
            DrawGravity();
            DrawStiffness();
            //DrawRandomize();
        }

        private void DrawDebugControls()
        {
            EditorGUILayout.PropertyField(debugOutline, new GUIContent("Mesh outline", "Will highlight the mesh."));
            EditorGUILayout.PropertyField(debugTriangles, new GUIContent("Final mesh", "Show triangles and outline used to build the mesh."));
            EditorGUILayout.PropertyField(debugUVs, new GUIContent("Draw UVs", "Show UVs created for the mesh (at the center of the scene)."));
            DrawHierarchyType();
        }

        private void DrawHeader(string text, string description = "")
        {
            EditorGUILayout.Space();
            var rect = DrawTexture(bgTexture, false);
            EditorGUI.LabelField(rect, new GUIContent(text, description), header);
        }

        private bool isDraggedHasRigidbody = false;
        private void DrawTarget()
        {
            if (DragAndDrop.objectReferences.Length > 0 && isDraggedHasRigidbody || DragAndDrop.objectReferences.Length == 0 && tentacleTargetRigidbody.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(tentacleTargetRigidbody, new GUIContent("Target", "Tentacle will be trying to reach this target."));
                if (EditorGUI.EndChangeCheck())
                    tentacleTarget.objectReferenceValue = ((Rigidbody2D)tentacleTargetRigidbody.objectReferenceValue)?.transform;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(tentacleTarget, new GUIContent("Target", "Tentacle will be trying to reach this target."));
                if (EditorGUI.EndChangeCheck())
                    tentacleTargetRigidbody.objectReferenceValue = null;
            }
        }
        private void RenewTarget()
        {
            var draggedGOs = DragAndDrop.objectReferences;
            if (draggedGOs != null && draggedGOs.Length > 0)
            {
                if (draggedGOs[0] is GameObject draggedGO)
                {
                    if (draggedGO.GetComponent<Rigidbody2D>() != null)
                        isDraggedHasRigidbody = true;
                    else
                        isDraggedHasRigidbody = false;
                    //Repaint();
                }
                Repaint(); // TODO: fix this
            }
        }

        private void DrawSortingLayers()
        {
            var haveDifferentValues = false;
            var sortingLayerName = tentacleData[0].meshRenderer.sortingLayerName;
            for (int i = 1; i < tentacleData.Length; i++)
                if (sortingLayerName != tentacleData[i].meshRenderer.sortingLayerName)
                {
                    haveDifferentValues = true;
                    break;
                }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            int value;
            if (IsPropertyModified(typeof(MeshRenderer), "SortingLayer"))
            {
                BeginBoldLabels();
                EditorStyles.popup.fontStyle = FontStyle.Bold;
                value = EditorGUILayout.Popup(new GUIContent("Sorting Layer", "Name of the renderer's sorting layer."), haveDifferentValues ? 0 : Array.IndexOf(sortingLayerNames, sortingLayerName), sortingLayerNames);
                EditorStyles.popup.fontStyle = FontStyle.Normal;
                EndBoldLabels();
            }
            else
            {
                value = EditorGUILayout.Popup(new GUIContent("Sorting Layer", "Name of the renderer's sorting layer."), haveDifferentValues ? 0 : Array.IndexOf(sortingLayerNames, sortingLayerName), sortingLayerNames);
            }

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetSortingLayerName(sortingLayerNames[value]);

            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawSortingOrder()
        {
            var haveDifferentValues = false;
            var sortingOrder = tentacleData[0].meshRenderer.sortingOrder;
            for (int i = 1; i < tentacleData.Length; i++)
                if (sortingOrder != tentacleData[i].meshRenderer.sortingOrder)
                {
                    haveDifferentValues = true;
                    break;
                }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            int value;
            if (IsPropertyModified(typeof(MeshRenderer), "SortingOrder"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.IntField(new GUIContent("Order in Layer", "Renderer's order within the sorting layer."), sortingOrder);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.IntField(new GUIContent("Order in Layer", "Renderer's order within the sorting layer."), sortingOrder);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetSortingOrder(value);

            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private bool DrawIsTrigger()
        {
            var haveDifferentValues = false;
            var isTrigger = tentacleData[0].IsTrigger;
            if (isTrigger.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (isTrigger.Item1 != tentacleData[i].IsTrigger.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            bool value;
            if (IsPropertyModified(typeof(CircleCollider2D), "IsTrigger"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.Toggle(new GUIContent("Is Trigger", "Whether the collider behaves as a trigger or not."), isTrigger.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.Toggle(new GUIContent("Is Trigger", "Whether the collider behaves as a trigger or not."), isTrigger.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                {
                    tentacleData[i].SetTrigger(value);
                }

            if (haveDifferentValues) EditorGUI.showMixedValue = false;

            return value;
        }

        private bool DrawPolygonalCollider()
        {
            var haveDifferentValues = false;
            var isPolygonalEnabled = tentacleData[0].PolygonalEnabled;
            for (int i = 1; i < tentacleData.Length; i++)
                if (isPolygonalEnabled != tentacleData[i].PolygonalEnabled)
                {
                    haveDifferentValues = true;
                    break;
                }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            bool value;
            if (IsPropertyModified(typeof(PolygonCollider2D), "Enabled"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.Toggle(new GUIContent("Polygonal Collider", "Whether the polygonal collider enabled or not."), isPolygonalEnabled);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.Toggle(new GUIContent("Polygonal Collider", "Whether the polygonal collider enabled or not."), isPolygonalEnabled);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].PolygonalEnabled = value;

            if (haveDifferentValues) EditorGUI.showMixedValue = false;

            return value;
        }

        private void DrawIsPolygonalTrigger(bool enabled)
        {
            if (enabled)
            {
                GUI.enabled = false;
                EditorGUILayout.Toggle(new GUIContent("   Is Trigger", "Polygonal collider is building a the realtime so it can only by set as trigger, otherwise it won't work with Unity physics correctly."), true);
                GUI.enabled = true;
            }
        }

        private void DrawColliderType()
        {
            var haveDifferentValues = false;
            var currentType = tentacleData[0].CurrentColliderType;
            if (currentType == (TentacleData.ColliderType)(-1))
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (currentType != tentacleData[i].CurrentColliderType)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            TentacleData.ColliderType value;
            if (IsPropertyModified(typeof(CircleCollider2D), "Enabled"))
            {
                BeginBoldLabels();
                value = (TentacleData.ColliderType)EditorGUILayout.EnumPopup(new GUIContent("Collider", "Type of the collider the tentacle will use."), currentType);
                EndBoldLabels();
            }
            else
                value = (TentacleData.ColliderType)EditorGUILayout.EnumPopup(new GUIContent("Collider", "Type of the collider the tentacle will use."), currentType);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetCollider(value);

            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawReduction(bool enabled)
        {
            if (enabled)
                EditorGUILayout.IntSlider(reduction, 0, 32, new GUIContent("   Reduction", "Simplify polygonal collider for better performance. 0 = no reduction."));
        }

        private void DrawMass()
        {
            var haveDifferentValues = false;
            var mass = tentacleData[0].Mass;
            if (mass.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (mass.Item1 != tentacleData[i].Mass.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (IsPropertyModified(typeof(Rigidbody2D), "Mass"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Mass", "The mass of an each segment of this tentacle."), mass.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Mass", "The mass of an each segment of this tentacle."), mass.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetMass(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawGravity()
        {
            var haveDifferentValues = false;
            var gravity = tentacleData[0].Gravity;
            if (gravity.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (gravity.Item1 != tentacleData[i].Gravity.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (IsPropertyModified(typeof(Rigidbody2D), "GravityScale"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Gravity", "How much gravity affects the tentacle."), gravity.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Gravity", "How much gravity affects the tentacle."), gravity.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetGravity(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawDrag()
        {
            var haveDifferentValues = false;
            var drag = tentacleData[0].Drag;
            if (drag.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (drag.Item1 != tentacleData[i].Drag.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (IsPropertyModified(typeof(Rigidbody2D), "LinearDrag"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Drag", "The drag of an each segment of this tentacle."), drag.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Drag", "The drag of an each segment of this tentacle."), drag.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetDrag(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawLength()
        {
            var haveDifferentValues = false;
            var length = tentacleData[0].Length;
            if (length.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (length.Item1 != tentacleData[i].Length.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (IsPropertyModified(typeof(SpringJoint2D), "Distance"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Length", "The length of the tentacle."), length.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Length", "The length of the tentacle."), length.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetLength(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawStiffness()
        {
            var haveDifferentValues = false;
            var stiffness = tentacleData[0].Stiffness;
            if (stiffness.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (stiffness.Item1 != tentacleData[i].Stiffness.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (IsPropertyModified(typeof(SpringJoint2D), "Frequency"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Stiffness", "The stiffness of an each segment of this tentacle."), stiffness.Item1);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Stiffness", "The stiffness of an each segment of this tentacle."), stiffness.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].SetStiffness(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawBodyType()
        {
            var haveDifferentValues = false;
            var currentType = tentacleData[0].TentacleBodyType;
            if (currentType == (TentacleData.BodyType)(-1))
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (currentType != tentacleData[i].TentacleBodyType)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            TentacleData.BodyType value;
            if (IsPropertyModified(typeof(SpringJoint2D), "Frequency"))
            {
                BeginBoldLabels();
                value = (TentacleData.BodyType)EditorGUILayout.EnumPopup(new GUIContent("Attached To", "Whether tentacle attached or detached."), currentType);
                EndBoldLabels();
            }
            else
                value = (TentacleData.BodyType)EditorGUILayout.EnumPopup(new GUIContent("Attached To", "Whether tentacle attached or detached."), currentType);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetBodyType(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;

            if (value == TentacleData.BodyType.rigidbody)
            {
                DrawParentBody();
                DrawParentBodyOffset();
            }
        }

        private void DrawParentBody()
        {
            var bodyHasDifferentValues = false;
            var currentBody = tentacleData[0].ParentBody;
            for (int i = 1; i < tentacleData.Length; i++)
                if (currentBody != tentacleData[i].ParentBody)
                {
                    bodyHasDifferentValues = true;
                    break;
                }
            if (bodyHasDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            Rigidbody2D body;
            if (IsPropertyModified(typeof(HingeJoint2D), "ConnectedRigidBody"))
            {
                BeginBoldLabels();
                body = (Rigidbody2D)EditorGUILayout.ObjectField(new GUIContent("   Parent Body", "Attach tentacle to this parent."), currentBody, typeof(Rigidbody2D), true);
                EndBoldLabels();
            }
            else
                body = (Rigidbody2D)EditorGUILayout.ObjectField(new GUIContent("   Parent Body", "Attach tentacle to this parent."), currentBody, typeof(Rigidbody2D), true);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetParentBody(body);
            if (bodyHasDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawParentBodyOffset()
        {
            var hasDifferentValues = false;
            var currentOffset = tentacleData[0].ParentBodyOffset;
            for (int i = 1; i < tentacleData.Length; i++)
                if (currentOffset != tentacleData[i].ParentBodyOffset)
                {
                    hasDifferentValues = true;
                    break;
                }
            if (hasDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            Vector2 value;
            if (IsPropertyModified(typeof(HingeJoint2D), "ConnectedAnchor.x") || IsPropertyModified(typeof(HingeJoint2D), "ConnectedAnchor.y"))
            {
                BeginBoldLabels();
                value = EditorGUILayout.Vector2Field(new GUIContent("   Parent Body Offset", "."), currentOffset);
                EndBoldLabels();
            }
            else
                value = EditorGUILayout.Vector2Field(new GUIContent("   Parent Body Offset", "."), currentOffset);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].SetParentBodyOffset(value);
            if (hasDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawCatchReleaseButtons()
        {
            if (!EditorApplication.isPlaying) return;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Catch target", "Catch the target set in the above field. For test purposes. Active in the playmode only.")))
            {
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].Tentacle.Catch();
            }
            if (GUILayout.Button(new GUIContent("Release target", "Release catched target set in the above field. For test purposes. Active in the playmode only.")))
            {
                for (int i = 0; i < tentacleData.Length; i++)
                    tentacleData[i].Tentacle.Release();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHierarchyType()
        {
            var haveDifferentValues = false;
            var hideFlags = tentacleData[0].HierarchyType;
            if (hideFlags.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < tentacleData.Length; i++)
                    if (hideFlags.Item1 != tentacleData[i].HierarchyType.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            var value = EditorGUILayout.Toggle(new GUIContent("Show Segments", "Show childs of this tentacle."), hideFlags.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < tentacleData.Length; i++) tentacleData[i].ChangeHiererachy(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawAnimations()
        {
            EditorGUILayout.PropertyField(animation, new GUIContent("Animation", "Additional physics-based animation of the tentacle."));
            //var enumValue = animation.enumValueIndex;
            if (animation.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(frequency, new GUIContent("   Frequency", "Frequency of the tentacle's animations."));
                EditorGUILayout.PropertyField(amplitude, new GUIContent("   Amplitude", "Strength of the tentacle's animations."));
                EditorGUILayout.PropertyField(animationDelay, new GUIContent("   Delay", "Delay to start the animation (for randomization purposes)."));
            }
        }

        private void RenewEditor()
        {
            CreateTentacleData();
            Repaint();
        }

        private bool IsPropertyModified(Type type, string property)
        {
            if (EditorApplication.isPlaying) return false;
            if (tentacleData.Length == 1)
            {
                var modifications = PrefabUtility.GetPropertyModifications(tentacleData[0].Tentacle);
                if (modifications != null)
                    for (int j = 0; j < modifications.Length; j++)
                    {
                        var modification = modifications[j];
                        if (modification.target.GetType() == type && modification.propertyPath == $"m_{property}")
                            return true;
                    }
            }
            return false;
        }

        private Rect DrawTexture(Texture texture, bool fit = true)
        {
            var height = fit ? texture.height * EditorGUIUtility.currentViewWidth / texture.width : EditorGUIUtility.singleLineHeight + 4f;
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
            var guiColor = GUI.color;
            GUI.color = Color.clear;
            EditorGUI.DrawTextureTransparent(rect, texture);
            GUI.color = guiColor;
            return rect;
        }

        private Texture2D LoadTexture(string name)
        {
            var monoscript = MonoScript.FromScriptableObject(this);
            var editorPath = AssetDatabase.GetAssetPath(monoscript);
            var scriptsFolder = System.IO.Path.GetDirectoryName(editorPath);
            var editorFolder = System.IO.Path.GetDirectoryName(scriptsFolder);
            var texture = (Texture2D)AssetDatabase.LoadAssetAtPath(editorFolder + $"/Textures/{name}", typeof(Texture2D));
            if (texture == null)
            {
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, new Color32(69, 85, 118, 255));
                texture.Apply();
            }
            return texture;
        }

        private void InitializeSortingLayers()
        {
            sortingLayerNames = new string[SortingLayer.layers.Length];
            for (int i = 0; i < SortingLayer.layers.Length; i++)
                sortingLayerNames[i] = SortingLayer.layers[i].name;
            //sortingLayerNames = new GUIContent[SortingLayer.layers.Length];
            //for (int i = 0; i < SortingLayer.layers.Length; i++)
            //    sortingLayerNames[i] = new GUIContent(SortingLayer.layers[i].name);
        }

        private void InitializeStyles()
        {
            header = new GUIStyle();
            header.normal.textColor = Color.white;
            header.alignment = TextAnchor.MiddleCenter;
            header.fontStyle = FontStyle.Bold;
        }

        private void DrawPrefabModifiedStyle()
        {
            if (TentacleEditorHelper.IsNewUnityVersion)
            {
                var rect = GUILayoutUtility.GetLastRect();
                rect.width = 3f;
                rect.height += 2f;
                rect.position = new Vector2(rect.position.x - 15f, rect.position.y - 1f);
                EditorGUI.DrawRect(rect, new Color(52f / 255f, 166f / 255f, 228f / 255f));
            }
        }

        private void BeginBoldLabels()
        {
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorStyles.textField.fontStyle = FontStyle.Bold;
            EditorStyles.popup.fontStyle = FontStyle.Bold;
            EditorStyles.objectField.fontStyle = FontStyle.Bold;
        }
        private void EndBoldLabels()
        {
            EditorStyles.label.fontStyle = FontStyle.Normal;
            EditorStyles.textField.fontStyle = FontStyle.Normal;
            EditorStyles.popup.fontStyle = FontStyle.Normal;
            EditorStyles.objectField.fontStyle = FontStyle.Normal;
            DrawPrefabModifiedStyle();
        }

        private struct TentacleData
        {
            public Tentacle Tentacle { get; private set; }

            public MeshRenderer meshRenderer;
            private PolygonCollider2D polygonCollider;
            private CircleCollider2D[] circleColliders;
            private Rigidbody2D[] segments;
            private SpringJoint2D[] joints;
            private HingeJoint2D pivotHingeJoint;
            private MaterialPropertyBlock materialBlock;
            //private HingeJoint2D tipHingeJoint; // TODO: redundant

            public enum ColliderType { /*polygonal, */circles, circleOnTip, none };
            public ColliderType CurrentColliderType { get; private set; }
            public Tuple<bool, bool> IsTrigger { get; private set; }
            public bool PolygonalEnabled { get { return polygonCollider.enabled; } set { Undo.RecordObject(polygonCollider, "Set Polygon Collider"); polygonCollider.enabled = value; } }
            public Tuple<float, bool> Mass { get; private set; }
            public Tuple<float, bool> Gravity { get; private set; }
            public Tuple<float, bool> Drag { get; private set; }
            public Tuple<float, bool> Length { get; private set; }
            public Tuple<float, bool> Stiffness { get; private set; }
            public Tuple<bool, bool> HierarchyType { get; private set; }
            public enum BodyType { world, rigidbody, detached };
            public BodyType TentacleBodyType { get; private set; }
            public Rigidbody2D ParentBody { get; private set; }
            public Vector2 ParentBodyOffset { get; private set; }

            public TentacleData(Tentacle tentacle)
            {
                Tentacle = tentacle;
                var transform = tentacle.transform;
                var pivot = transform.GetChild(0).GetComponent<Rigidbody2D>();
                var tip = transform.GetChild(3).GetComponent<Rigidbody2D>();

                meshRenderer = pivot.GetComponent<MeshRenderer>();
                materialBlock = new MaterialPropertyBlock();
                polygonCollider = tip.GetComponent<PolygonCollider2D>();

                circleColliders = new CircleCollider2D[4];
                circleColliders[0] = pivot.GetComponent<CircleCollider2D>();
                for (int i = 1; i < circleColliders.Length - 1; i++)
                    circleColliders[i] = transform.GetChild(i).GetComponent<CircleCollider2D>();
                circleColliders[circleColliders.Length - 1] = tip.GetComponent<CircleCollider2D>();

                bool /*isPolygonEnabled = polygonCollider.enabled,*/
                    isPivotEnabled = circleColliders[0].enabled,
                    isSegment1Enabled = circleColliders[1].enabled,
                    isSegment2Enabled = circleColliders[2].enabled,
                    isTipEnabled = circleColliders[3].enabled;
                //if (!isPivotEnabled && !isSegment1Enabled && !isSegment2Enabled && !isTipEnabled && isPolygonEnabled)
                //    CurrentColliderType = ColliderType.polygonal;
                if (isPivotEnabled && isSegment1Enabled && isSegment2Enabled && isTipEnabled /*&& !isPolygonEnabled*/)
                    CurrentColliderType = ColliderType.circles;
                else if (!isPivotEnabled && !isSegment1Enabled && !isSegment2Enabled && isTipEnabled /*&& !isPolygonEnabled*/)
                    CurrentColliderType = ColliderType.circleOnTip;
                else if (!isPivotEnabled && !isSegment1Enabled && !isSegment2Enabled && !isTipEnabled /*&& !isPolygonEnabled*/)
                    CurrentColliderType = ColliderType.none;
                else
                    CurrentColliderType = (ColliderType)(-1);

                var isTrigger = circleColliders[0].isTrigger;
                var isTriggerHasMixedValues = false;
                //if (polygonCollider.isTrigger != isTrigger)
                //{
                //    isTriggerHasMixedValues = true;
                //}
                //else
                for (int i = 1; i < circleColliders.Length; i++)
                    if (isTrigger != circleColliders[i].isTrigger)
                    {
                        isTriggerHasMixedValues = true;
                        break;
                    }
                IsTrigger = new Tuple<bool, bool>(isTrigger, isTriggerHasMixedValues);

                //IsPolygonalEnabled = new Tuple<bool, bool>()

                segments = new Rigidbody2D[4];
                for (int i = 0; i < segments.Length; i++)
                    segments[i] = circleColliders[i].GetComponent<Rigidbody2D>();

                var mass = segments[0].mass;
                var massHasMixedValues = false;
                for (int i = 1; i < segments.Length; i++)
                    if (mass != segments[i].mass)
                    {
                        massHasMixedValues = true;
                        break;
                    }
                Mass = new Tuple<float, bool>(mass * segments.Length, massHasMixedValues);

                var gravity = segments[0].gravityScale;
                var gravityHaveMixedValues = false;
                for (int i = 1; i < segments.Length; i++)
                    if (gravity != segments[i].gravityScale)
                    {
                        gravityHaveMixedValues = true;
                        break;
                    }
                Gravity = new Tuple<float, bool>(gravity, gravityHaveMixedValues);

                var drag = segments[0].drag;
                var dragHaveMixedValues = false;
                for (int i = 1; i < segments.Length; i++)
                    if (drag != segments[i].drag)
                    {
                        dragHaveMixedValues = true;
                        break;
                    }
                Drag = new Tuple<float, bool>(drag, dragHaveMixedValues);

                joints = new SpringJoint2D[3];
                for (int i = 0; i < joints.Length; i++)
                    joints[i] = circleColliders[i + 1].GetComponent<SpringJoint2D>();

                var length = joints[0].distance;
                var lengthHasMixedValues = false;
                for (int i = 1; i < joints.Length; i++)
                    if (length != joints[i].distance)
                    {
                        lengthHasMixedValues = true;
                        break;
                    }
                Length = new Tuple<float, bool>(length * joints.Length, lengthHasMixedValues);

                var stiffness = joints[0].frequency;
                var stiffnessHasMixedValues = false;
                for (int i = 1; i < joints.Length; i++)
                    if (stiffness != joints[i].frequency)
                    {
                        stiffnessHasMixedValues = true;
                        break;
                    }
                Stiffness = new Tuple<float, bool>(stiffness, stiffnessHasMixedValues);

                pivotHingeJoint = segments[0].GetComponent<HingeJoint2D>();
                var bodyType = segments[0].bodyType;
                var pivotHingeJointEnabled = pivotHingeJoint.enabled;
                if (bodyType == RigidbodyType2D.Dynamic && !pivotHingeJointEnabled)
                    //TentacleBodyType = new Tuple<BodyType, Rigidbody2D>(BodyType.detached, rigidbody);
                    TentacleBodyType = BodyType.detached;
                else if (bodyType == RigidbodyType2D.Static && !pivotHingeJointEnabled)
                    TentacleBodyType = BodyType.world;
                else if (bodyType == RigidbodyType2D.Dynamic && pivotHingeJointEnabled)
                    TentacleBodyType = BodyType.rigidbody;
                else
                    TentacleBodyType = (BodyType)(-1);
                ParentBody = pivotHingeJoint.connectedBody;
                ParentBodyOffset = pivotHingeJoint.connectedAnchor;

                var hideFlags = segments[0].gameObject.hideFlags;
                var hideFlagsHasMixedValues = false;
                for (int i = 1; i < segments.Length; i++)
                    if (hideFlags != segments[i].gameObject.hideFlags)
                    {
                        hideFlagsHasMixedValues = true;
                        break;
                    }
                HierarchyType = new Tuple<bool, bool>(hideFlags == HideFlags.None ? true : false, hideFlagsHasMixedValues);
            }

            public void SetSortingLayerName(string name)
            {
                Undo.RecordObject(meshRenderer, "Set MeshRenderer SortingLayerName");
                meshRenderer.sortingLayerName = name;
            }

            public void SetSortingOrder(int order)
            {
                Undo.RecordObject(meshRenderer, "Set MeshRenderer SortingOrder");
                meshRenderer.sortingOrder = order;
            }

            public void SetCollider(ColliderType type)
            {
                Undo.RecordObjects(circleColliders, "Set Circle Colliders");

                switch (type)
                {
                    //case ColliderType.polygonal:
                    //    for (int i = 0; i < circleColliders.Length; i++)
                    //        circleColliders[i].enabled = false;
                    //    polygonCollider.enabled = true;
                    //    break;
                    case ColliderType.circles:
                        for (int i = 0; i < circleColliders.Length; i++)
                            circleColliders[i].enabled = true;
                        //polygonCollider.enabled = false;
                        break;
                    case ColliderType.circleOnTip:
                        for (int i = 0; i < circleColliders.Length - 1; i++)
                            circleColliders[i].enabled = false;
                        //polygonCollider.enabled = false;
                        circleColliders[circleColliders.Length - 1].enabled = true;
                        break;
                    case ColliderType.none:
                        for (int i = 0; i < circleColliders.Length; i++)
                            circleColliders[i].enabled = false;
                        //polygonCollider.enabled = false;
                        break;
                    default:
                        Debug.LogWarning("Unexpected colliders setup detected.");
                        break;
                }
                CurrentColliderType = type;
            }

            //public void SetPolygonalCollider(bool enabled)
            //{
            //    Undo.RecordObject(polygonCollider, "Set Polygon Collider");
            //    polygonCollider.enabled = enabled;
            //}

            public void SetTrigger(bool isTrigger)
            {
                Undo.RecordObjects(circleColliders, "Set CircleColliders2D isTrigger");
                //Undo.RecordObject(polygonCollider, "Set PolygonCollider2D isTrigger");
                for (int i = 0; i < circleColliders.Length; i++)
                    circleColliders[i].isTrigger = isTrigger;
                //polygonCollider.isTrigger = isTrigger;
                IsTrigger = new Tuple<bool, bool>(isTrigger, false);
            }

            public void UpdateCircleColliders(float width, AnimationCurve shape)
            {
                Undo.RecordObjects(circleColliders, "Circle Colliders Radius");
                for (int i = 0; i < circleColliders.Length; i++)
                    circleColliders[i].radius = width * shape.Evaluate(.3f * i);
            }

            public void SetMass(float mass)
            {
                Undo.RecordObjects(segments, "Set Segments Rigidbody2D Mass");
                var segmentMass = mass / segments.Length;
                for (int i = 0; i < segments.Length; i++)
                    segments[i].mass = segmentMass; // TODO: consider curve
                Mass = new Tuple<float, bool>(mass, false);
            }

            public void SetGravity(float gravity)
            {
                Undo.RecordObjects(segments, "Set Segments Rigidbody2D Gravity");
                for (int i = 0; i < segments.Length; i++)
                    segments[i].gravityScale = gravity;
                Gravity = new Tuple<float, bool>(gravity, false);
            }

            public void SetDrag(float drag)
            {
                Undo.RecordObjects(segments, "Set Segments Rigidbody2D Drag");
                for (int i = 0; i < segments.Length; i++)
                    segments[i].drag = drag;
                Drag = new Tuple<float, bool>(drag, false);
            }

            public void SetLength(float length)
            {
                Undo.RecordObjects(joints, "Set SpringJoint2D Length");
                var segmentLength = length / (segments.Length - 1);
                for (int i = 0; i < joints.Length; i++)
                    joints[i].distance = segmentLength;
                Length = new Tuple<float, bool>(length, false);

                if (!Application.isPlaying)
                {
                    for (int i = 0; i < joints.Length; i++)
                    {
                        var transform = joints[i].transform;
                        Undo.RecordObject(transform, $"Set Joint{i} Transform Length");
                        var position = transform.localPosition;
                        position.y = segmentLength * (i + 1);
                        transform.localPosition = position;
                    }
                }
            }

            public void SetStiffness(float stiffness)
            {
                Undo.RecordObjects(joints, "Set SpringJoints2D Frequency");
                for (int i = 0; i < joints.Length; i++)
                    joints[i].frequency = stiffness;
                Stiffness = new Tuple<float, bool>(stiffness, false);
            }

            public void SetBodyType(BodyType type)
            {
                Undo.RecordObjects(segments, "Set Segments BodyType");
                Undo.RecordObject(pivotHingeJoint, "Set Pivot HingeJoint BodyType");
                switch (type)
                {
                    case BodyType.world:
                        segments[0].bodyType = RigidbodyType2D.Static;
                        pivotHingeJoint.enabled = false;
                        break;
                    case BodyType.rigidbody:
                        segments[0].bodyType = RigidbodyType2D.Dynamic;
                        pivotHingeJoint.enabled = true;
                        break;
                    case BodyType.detached:
                        segments[0].bodyType = RigidbodyType2D.Dynamic;
                        pivotHingeJoint.enabled = false;
                        break;
                    default:
                        break;
                }
                TentacleBodyType = type;
            }

            public void SetParentBody(Rigidbody2D rigidbody)
            {
                Undo.RecordObject(pivotHingeJoint, "Pivot HingeJoint2D ConnectedBody");
                pivotHingeJoint.connectedBody = rigidbody;
                ParentBody = rigidbody;
            }

            public void SetParentBodyOffset(Vector2 offset)
            {
                Undo.RecordObject(pivotHingeJoint, "Pivot HingeJoint2D ConnectedAnchor");
                pivotHingeJoint.connectedAnchor = offset;
                ParentBodyOffset = offset;

                if (!Application.isPlaying)
                {
                    Undo.RecordObject(segments[0], $"Set ParentBodyOffset on Pivot rigidbody");
                    segments[0].position = ParentBody.position + ParentBodyOffset;
                }
            }

            public void SetColor(Color color)
            {
                Undo.RecordObject(meshRenderer, "MeshRenderer Color");
                materialBlock.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(materialBlock);
            }

            public void ChangeHiererachy(bool show)
            {
                Undo.RecordObjects(segments, "Segments HideFlags");
                if (show)
                    for (int i = 0; i < segments.Length; i++)
                        segments[i].gameObject.hideFlags = HideFlags.None;
                else
                    for (int i = 0; i < segments.Length; i++)
                        segments[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
                HierarchyType = new Tuple<bool, bool>(segments[0].gameObject.hideFlags == HideFlags.None ? true : false, false);
                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}