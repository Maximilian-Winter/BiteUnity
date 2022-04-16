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

public class BiteExampleLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObjectToMove;
    
    [SerializeField]
    private Transform m_TransformTarget;

    
    [SerializeField]
    private GameObject m_GameObjectToInstantiate;

    private BiteProgram m_MoveCameraBiteProgram;
    private BiteVm m_MoveCameraBiteVm = new BiteVm();
    
    private BiteProgram m_CreateGameObjectBiteProgram;
    private BiteVm m_CreateGameObjectBiteVm = new BiteVm();

    void Start()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif

        //StartCreateGameObject();
        StartCameraMovement();
    }

    public void StartCameraMovement()
    {
        m_MoveCameraBiteVm.InitVm();
        m_MoveCameraBiteVm.SynchronizationContext = SynchronizationContext.Current;
      
        m_MoveCameraBiteVm.RegisterExternalGlobalObjects(new Dictionary<string, object>()
        {
            { "Camera", m_GameObjectToMove },
            { "TransformTarget", m_TransformTarget },
        });
        
        IEnumerable< string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestMoveGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        m_MoveCameraBiteProgram = compiler.Compile(files.Select(File.ReadAllText));

        m_MoveCameraBiteProgram.TypeRegistry.RegisterType<Vector3>();
        m_MoveCameraBiteProgram.TypeRegistry.RegisterType<GameObject>();
        m_MoveCameraBiteProgram.TypeRegistry.RegisterType<Transform>();
        m_MoveCameraBiteVm.RegisterSystemModuleCallables( m_MoveCameraBiteProgram.TypeRegistry );
        m_MoveCameraBiteVm.RegisterCallable( "UnityDeltaTime", new UnityDeltaTimeVmCallable() );
        Task.Run(() =>
        {
            m_MoveCameraBiteVm.Interpret(m_MoveCameraBiteProgram);
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception.InnerException.Message);
                Debug.LogError(t.Exception.InnerException.StackTrace);
                Debug.LogError(t.Exception.StackTrace);
            }
        });
    }

    public void StopMoveCameraVm()
    {
        m_MoveCameraBiteVm.Stop();
    }

    public void StartCreateGameObject()
    {
        m_CreateGameObjectBiteVm.InitVm();
        m_CreateGameObjectBiteVm.SynchronizationContext = SynchronizationContext.Current;
      
        m_CreateGameObjectBiteVm.RegisterExternalGlobalObjects(new Dictionary<string, object>()
        {
            { "PrefabFromUnity", m_GameObjectToInstantiate },
        });
        
        IEnumerable< string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestCreateGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        m_CreateGameObjectBiteProgram = compiler.Compile(files.Select(File.ReadAllText));

        m_CreateGameObjectBiteProgram.TypeRegistry.RegisterType<Vector3>();
        m_CreateGameObjectBiteProgram.TypeRegistry.RegisterType<GameObject>();
        m_CreateGameObjectBiteProgram.TypeRegistry.RegisterType<Transform>();
        m_CreateGameObjectBiteVm.RegisterSystemModuleCallables( m_CreateGameObjectBiteProgram.TypeRegistry );
        m_CreateGameObjectBiteVm.RegisterCallable( "UnityDeltaTime", new UnityDeltaTimeVmCallable() );
        m_CreateGameObjectBiteVm.RegisterCallable( "UnityInstantiate", new UnityInstantiate() );
        Task.Run(() =>
        {
            m_CreateGameObjectBiteVm.Interpret(m_CreateGameObjectBiteProgram);
        }).ContinueWith(t =>
        {
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
            m_MoveCameraBiteVm.Stop();
            m_CreateGameObjectBiteVm.Stop();
        }
    }
#endif
    
}
