using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bite.Compiler;
using Bite.Runtime.CodeGen;
using UnityEngine;

public class BiteCreateGameObject : MonoBehaviour
{
    void Start()
    {
        IEnumerable < string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestCreateGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        BiteProgram program = compiler.Compile(files.Select(File.ReadAllText));
        
        program.Run(/*new Dictionary < string, object >()
        {
            //{ "a", 1 },
            //{ "b", 2 }
        }*/);
    }
}
