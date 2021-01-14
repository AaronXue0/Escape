using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TargetMode
{
    None,
    PingPongMove,
    Rotate,
    DoPingPongMoveAndRotate,
    Transport,
    Trigger,
}

public class MapObjectCore : MonoBehaviour
{

    public TargetMode mode;

    /// <summary>
    /// If Time Scale Effact.
    /// </summary>
    [SerializeField] bool unscaledTime = false;
    [SerializeField] float startTime = 1f;
    [SerializeField] float duration = 1f;


    [HideInInspector]
    [SerializeField] float interval = 1.0f;

    [HideInInspector]
    [SerializeField] Vector2 movePos = Vector2.zero;

    [HideInInspector]
    [SerializeField] float rotateAngle = 0;
    [HideInInspector]
    [SerializeField] int rotateTimes = -1;
    

    private void Awake() => this.Init();

    public void Init()
    {
        switch (mode)
        {
            case TargetMode.PingPongMove:
                InvokeRepeating("DoPingPongMove", startTime, duration + interval);
                break;
            case TargetMode.Rotate:
                Invoke("DoRotate", startTime);
                break;
        }
    }

    public void DoPingPongMove()
    {
        Vector2 _pos = transform.position;
        float _dur = (duration) / 2 - interval;
        transform.DOMove(movePos, _dur).OnComplete(
            () => this.AbleToDo(
                interval,
                () => transform.DOMove(_pos, _dur))
        ).SetUpdate(unscaledTime);
    }
    public void DoRotate()
    {
        float _dur = duration / 2 - interval;
        Vector3 _rot = new Vector3(0,0,rotateAngle);
        transform.DORotate(_rot, _dur).SetEase(Ease.Linear).SetRelative().SetLoops(rotateTimes).SetUpdate(unscaledTime);
    }
    public void DoPingPongMoveAndRotate()
    {
        
    }
}

public static class MapObjectExtension
{
}


 #if UNITY_EDITOR
[CustomEditor(typeof(MapObjectCore)), CanEditMultipleObjects]
public class MapObjectCoreEditor : Editor
{
    MapObjectCore mapObjectCore;
    public SerializedProperty interval, movePos;
    public SerializedProperty rotateAngle,rotateTimes;

    void OnEnable()
    {
        mapObjectCore = (MapObjectCore)target;

        interval = serializedObject.FindProperty("interval");
        movePos = serializedObject.FindProperty("movePos");

        rotateAngle = serializedObject.FindProperty("rotateAngle");
        rotateTimes = serializedObject.FindProperty("rotateTimes");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        
        switch (mapObjectCore.mode)
        {
            case TargetMode.PingPongMove:
                EditorGUILayout.LabelField("PingPong Move");
                DrawMove();
                break;
            case TargetMode.Rotate:
                EditorGUILayout.LabelField("Rotate");
                DrawRotate();
                break;
            case TargetMode.DoPingPongMoveAndRotate:
                EditorGUILayout.LabelField("PingPong Move & Rotate");
                DrawMove();
                DrawRotate();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
    void DrawMove()
    {
        EditorGUILayout.PropertyField(interval, new GUIContent("Interval"));
        EditorGUILayout.PropertyField(movePos, new GUIContent("Move Pos"));
    }

    void DrawRotate()
    {
        EditorGUILayout.PropertyField(rotateAngle, new GUIContent("Rotate Angle"));
        EditorGUILayout.PropertyField(rotateTimes, new GUIContent("Rotate Times"));
    }
    
}
#endif