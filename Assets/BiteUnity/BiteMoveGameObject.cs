using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bite.Compiler;
using Bite.Runtime.CodeGen;
using UnityEngine;

public class BiteMoveGameObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObjectToMove;
    
    private BiteProgram program; 
    void Start()
    {
        IEnumerable < string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestMoveGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        program = compiler.Compile(files.Select(File.ReadAllText));
        
       
    }

    private void Update()
    {
        program.Run(new Dictionary < string, object >()
        {
            { "GameObjectFromUnity", m_GameObjectToMove },
            //{ "b", 2 }
        });
    }
}
