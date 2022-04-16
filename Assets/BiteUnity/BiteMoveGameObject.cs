using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bite.Compiler;
using Bite.Modules.Callables;
using Bite.Runtime;
using Bite.Runtime.CodeGen;
using BiteUnity;
using UnityEditor;
using UnityEngine;

public class DeltaTimeGetter
{
    public float DeltaTime => Time.deltaTime;
}
public class BiteMoveGameObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObjectToMove;
    
    [SerializeField]
    private Transform m_TransformTarget;


    private BiteProgram program;
    private BiteVm vm = new BiteVm();

    void Start()
    {
        Debug.Log("Started BiteMoveGameObject!");
        vm.InitVm();
        vm.SynchronizationContext = SynchronizationContext.Current;
      
        vm.RegisterExternalGlobalObjects(new Dictionary<string, object>()
        {
            { "Camera", m_GameObjectToMove },
            { "TransformTarget", m_TransformTarget },
        });

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif

        IEnumerable< string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestMoveGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        program = compiler.Compile(files.Select(File.ReadAllText));

        program.TypeRegistry.RegisterType<Vector3>();
        program.TypeRegistry.RegisterType<GameObject>();
        program.TypeRegistry.RegisterType<Transform>();
        vm.RegisterSystemModuleCallables( program.TypeRegistry );
        vm.RegisterCallable( "UnityDeltaTime", new UnityDeltaTimeVmCallable() );
        Task.Run(() =>
        {
            Debug.Log("Running program");
            vm.Interpret(program);
        }).ContinueWith(t =>
        {
            Debug.Log("Stopped!");
            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception.InnerException.Message);
                Debug.LogError(t.Exception.InnerException.StackTrace);
                Debug.LogError(t.Exception.StackTrace);
            }
        });
    }
    
#if UNITY_EDITOR
    private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        if(obj == PlayModeStateChange.ExitingPlayMode)
        {
           vm.Stop();
        }
    }
#endif
    
}
