using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObjectToMove;
    
    [SerializeField]
    private Transform m_TransformTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_GameObjectToMove.transform.LookAt( m_TransformTarget );
        m_GameObjectToMove.transform.Translate( Vector3.right * Time.deltaTime );
    }
}
