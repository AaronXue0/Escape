using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TargetMode
{
    None,
    Move,
    PingPongMove,
    Rotate,
    PingPongRotate,
    PingPongMoveAndRotate,
    Transport,
    Trigger,
}

public class MapObjectCore : MonoBehaviour
{

    public TargetMode mode;
    public bool testEvniorment = false;
    /// <summary>
    /// If Time Scale Effact.
    /// </summary>
    [SerializeField] bool unscaledTime = false;
    [SerializeField] float startTime = 1f;
    [SerializeField] float duration = 1f;


    [Header("Move")]
    [HideInInspector]
    [SerializeField] float interval = 1.0f;

    [HideInInspector]
    [SerializeField] Vector2 movePos = Vector2.zero;


    [Header("Rotate")]
    [HideInInspector]
    [SerializeField] float rotateAngle = 0;
    [HideInInspector]
    [SerializeField] int rotateTimes = -1;

    [Header("Transport")]
    [HideInInspector]
    [SerializeField] Transform destination = null;
    
    [Header("Trigger")]
    [HideInInspector]
    [SerializeField] GameObject activeObject = null;
    [HideInInspector]
    [SerializeField] UnityEvent activeFunction = null;


    private void Awake()
    {
        if (testEvniorment)
        {
            Init();
        }
    }

    public void Init()
    {
        switch (mode)
        {
            case TargetMode.Move:
                Invoke("DoMove", startTime);
                break;
            case TargetMode.PingPongMove:
                Invoke("DoPingPongMove", startTime);
                break;
            case TargetMode.Rotate:
                Invoke("DoRotate", startTime);
                break;
            case TargetMode.PingPongRotate:
                Invoke("DoRotate", startTime);
                break;
            case TargetMode.PingPongMoveAndRotate:
                DoPingPongMoveAndRotate();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            switch (mode)
            {
                case TargetMode.Transport:
                    other.transform.position = destination.position;
                    break;
                case TargetMode.Trigger:
                    if (activeObject != null)
                    {
                        activeObject.SetActive(false);
                    }
                    else if (activeFunction != null)
                    {
                        activeFunction.Invoke();
                    }
                    break;
            } 
        }
    }

    public void DoMove()
    {
    }
    public void DoPingPongMove()
    {
        Vector2 _pos = transform.position;
        float _dur = (duration) / 2 - interval;
        transform.DOMove(movePos, _dur).OnComplete(
            () => this.AbleToDo( interval,
            () => transform.DOMove(_pos, _dur))
        ).SetUpdate(unscaledTime).SetEase(Ease.Linear).SetRelative();
        this.AbleToDo(duration + interval,() => DoMove());
    }
    public void DoRotate()
    {
        float _dur = duration;
        Vector3 _rot = new Vector3(0,0,rotateAngle);
        transform.DORotate(_rot, _dur).SetEase(Ease.Linear).SetRelative().SetLoops(rotateTimes).SetUpdate(unscaledTime);
    }
    public void DoPingPongRotate()
    {
        float _dur = duration / 2;
        Vector3 _currentRot = new Vector3(0,0, transform.localRotation.z);
        Vector3 _rot = new Vector3(0, 0, rotateAngle);
        
        transform.DORotate(_rot, _dur).OnComplete(
            () => this.AbleToDo( interval,
            () => transform.DORotate(_currentRot, _dur))
        ).SetEase(Ease.Linear).SetRelative().SetUpdate(unscaledTime);
    }
    public void DoPingPongMoveAndRotate()
    {
        InvokeRepeating("DoPingPongMove", startTime, duration + interval);
        Invoke("DoRotate", startTime);
    }
}


 #if UNITY_EDITOR
[CustomEditor(typeof(MapObjectCore)), CanEditMultipleObjects]
public class MapObjectCoreEditor : Editor
{
    MapObjectCore mapObjectCore;
    public SerializedProperty interval, movePos;
    public SerializedProperty rotateAngle, rotateTimes;
    public SerializedProperty destination;
    public SerializedProperty activeObject, activeFunction;

    void OnEnable()
    {
        mapObjectCore = (MapObjectCore)target;

        interval = serializedObject.FindProperty("interval");
        movePos = serializedObject.FindProperty("movePos");

        rotateAngle = serializedObject.FindProperty("rotateAngle");
        rotateTimes = serializedObject.FindProperty("rotateTimes");
        
        destination = serializedObject.FindProperty("destination");
        
        activeObject = serializedObject.FindProperty("activeObject");
        activeFunction = serializedObject.FindProperty("activeFunction");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        
        switch (mapObjectCore.mode)
        {
            case TargetMode.Move:
            case TargetMode.PingPongMove:
                EditorGUILayout.LabelField("PingPong Move");
                DrawMove();
                break;
            case TargetMode.Rotate:
            case TargetMode.PingPongRotate:
                EditorGUILayout.LabelField("Rotate");
                DrawRotate();
                break;
            case TargetMode.PingPongMoveAndRotate:
                EditorGUILayout.LabelField("PingPong Move & Rotate");
                DrawMove();
                DrawRotate();
                break;
            case TargetMode.Transport:
                DrawTransport();
                break;
            case TargetMode.Trigger:
                DrawTrigger();
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

    void DrawTransport()
    {
        EditorGUILayout.PropertyField(destination, new GUIContent("Destination"));
    }

    void DrawTrigger()
    {
        EditorGUILayout.PropertyField(activeObject, new GUIContent("ActiveObject"));
        EditorGUILayout.PropertyField(activeFunction, new GUIContent("ActiveFunction"));
    }
}
#endif