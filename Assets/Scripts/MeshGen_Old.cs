//using System.Collections;§
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//public class MeshGen_Old : MonoBehaviour
//{
//    Mesh m;
//    MeshFilter mf;
//    public Vector3[] verticesArrayInput;
//    public Vector3[] verticesArray;
//    float verticesArrayCountFloat;
//    public int[] trianglesArray;
//    public int vertArrayCalc;
//    public int[] indexCalc;
//    // Start is called before the first frame update
//    void Start()
//    {
//        mf = GetComponent<MeshFilter>();
//        m = new Mesh();
//        mf.mesh = m;

//    }

    

//    void drawTriangle()
//    {
//        //Vector3[] verticesArray = new Vector3[4];
//        if (verticesArrayInput.Length <=3)
//        {
//            verticesArray = verticesArrayInput;
//        }
//        else if (verticesArrayInput.Length > 3)
//        {
//            vertArrayCalc = 3 + (3 * (verticesArrayInput.Length - 3));
//            indexCalc = new int[vertArrayCalc];
//            verticesArray = new Vector3[3+(3*(verticesArrayInput.Length -3))];
//            for (int i = 0; i < verticesArray.Length; i++)
//            {
//                if (i <= 2)
//                {
//                    //verticesArray[i] = verticesArrayInput[i];
//                    verticesArray[0] = verticesArrayInput[0];
//                    verticesArray[1] = verticesArrayInput[2];
//                    verticesArray[2] = verticesArrayInput[1];
//                    indexCalc[i] = i;
//                }
//                else
//                {
//                    if ((i + 1) % 3 == 0)
//                    {
//                        indexCalc[i] = (((i + 1) / 3)) +1;
//                        verticesArray[i] = verticesArrayInput[indexCalc[i]];
//                    }
//                    else if ((i + 1) % 3 == 1)
//                    {
//                        indexCalc[i] = (((i + 1) - 1) / 3);
//                        verticesArray[i] = verticesArrayInput[indexCalc[i]];
//                    }
//                    else if ((i + 1) % 3 == 2)
//                    {
//                        indexCalc[i] = (((i + 1) - 2) / 3) + 1;
//                        verticesArray[i] = verticesArrayInput[indexCalc[i]];
//                    }
//                }
                
//            }
//        }

//        trianglesArray = new int[verticesArray.Length];
//        for (int j = 0;  j < trianglesArray.Length; j++)
//        {
//            trianglesArray[j] = j;
//        }
        
//        m.vertices = verticesArray;
//        m.triangles = trianglesArray;

//    }
//    // Update is called once per frame
//    void Update()
//    {
//        drawTriangle();
//    }
//}
