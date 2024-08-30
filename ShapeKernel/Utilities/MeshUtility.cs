//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2024 by LEAP 71
// https://leap71.com
//
// Computational Engineering will profoundly change our physical world in the
// years ahead. Thank you for being part of the journey.
//
// We have developed this library to be used widely, for both commercial and
// non-commercial projects alike. Therefore, have released it under a permissive
// open-source license.
// 
// The LEAP 71 ShapeKernel is based on the PicoGK compact computational geometry 
// framework. See https://picogk.org for more information.
//
// LEAP 71 licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, THE SOFTWARE IS
// PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
//
// See the License for the specific language governing permissions and
// limitations under the License.   
//


using System.Numerics;
using PicoGK;


namespace Leap71
{
	namespace ShapeKernel
	{
		public static class MeshUtility
		{
            /// <summary>
            /// Creates a mesh object from a regularly arranged point grid.
            /// </summary>
            public static Mesh mshFromGrid(List<List<Vector3>> aGrid)
            {
                Mesh oMesh = new Mesh();
                for (int i = 1; i < aGrid.Count; i++)
                {
                    for (int j = 1; j < aGrid[i].Count; j++)
                    {
                        Vector3 vecPt1 = aGrid[i - 1][j - 1];
                        Vector3 vecPt2 = aGrid[i - 1][j];
                        Vector3 vecPt3 = aGrid[i][j];
                        Vector3 vecPt4 = aGrid[i][j - 1];
                        oMesh.nAddTriangle(vecPt4, vecPt1, vecPt2);
                        oMesh.nAddTriangle(vecPt2, vecPt3, vecPt4);
                    }
                }
                return oMesh;
            }

            /// <summary>
            /// Creates a mesh object from four points that form a quad shape.
            /// </summary>
            public static Mesh mshFromQuad(
                Vector3 vecPt1,
                Vector3 vecPt2,
                Vector3 vecPt3,
                Vector3 vecPt4)
            {
                Mesh oMesh = new Mesh();
                oMesh.nAddTriangle(vecPt4, vecPt1, vecPt2);
                oMesh.nAddTriangle(vecPt2, vecPt3, vecPt4);
                return oMesh;
            }

            /// <summary>
            /// Uses the referenced mesh object and adds four points that form a quad shape.
            /// </summary>
            public static void AddQuad(
                ref Mesh oMesh,
                Vector3 vecPt1,
                Vector3 vecPt2,
                Vector3 vecPt3,
                Vector3 vecPt4)
            {
                oMesh.nAddTriangle(vecPt4, vecPt1, vecPt2);
                oMesh.nAddTriangle(vecPt2, vecPt3, vecPt4);
            }

            /// <summary>
            /// Creates a new mesh by applying a transformation function to each vertex of the input mesh.
            /// </summary>
            public static Mesh mshApplyTransformation(  Mesh oMesh,
                                                        BaseShape.fnVertexTransformation fnTrafo)
            {
                Mesh oNewMesh       = new Mesh();
                int nTriangles      = oMesh.nTriangleCount();
                for (int i = 0; i < nTriangles; i++)
                {
                    Triangle oTri   = oMesh.oTriangleAt(i);
                    int iIndexA     = oTri.A;
                    int iIndexB     = oTri.B;
                    int iIndexC     = oTri.C;
                    Vector3 vecA    = oMesh.vecVertexAt(iIndexA);
                    Vector3 vecB    = oMesh.vecVertexAt(iIndexB);
                    Vector3 vecC    = oMesh.vecVertexAt(iIndexC);
                    Vector3 vecNewA = fnTrafo(vecA);
                    Vector3 vecNewB = fnTrafo(vecB);
                    Vector3 vecNewC = fnTrafo(vecC);
                    oNewMesh.nAddTriangle(vecNewA, vecNewB, vecNewC);
                }
                return oNewMesh;
            }

            /// <summary>
            /// Returns a mesh that has been translated from the input to the output frame.
            /// </summary>
            public static Mesh mshTranslateMeshOntoFrame(Mesh oMesh,
                                                         LocalFrame oInputFrame,
                                                         LocalFrame oOutputFrame)
            {
                return Mover.mshTranslateMeshOntoFrame(oMesh, oInputFrame, oOutputFrame);
            }

            protected class Mover
            {
                LocalFrame m_oInputFrame;
                LocalFrame m_oOutputFrame;

                /// <summary>
                /// Returns a mesh that has been translated from the input to the output frame.
                /// </summary>
                public static Mesh mshTranslateMeshOntoFrame(Mesh msh, LocalFrame oInputFrame, LocalFrame oOutputFrame)
                {
                    Mover oMover  = new(oInputFrame, oOutputFrame);
                    Mesh mshMoved = mshApplyTransformation(msh, oMover.vecTrafo);
                    return mshMoved;
                }

                protected Mover(LocalFrame oInputFrame,
                                LocalFrame oOutputFrame)
                {
                    m_oInputFrame  = oInputFrame;
                    m_oOutputFrame = oOutputFrame;
                }

                public Vector3 vecTrafo(Vector3 vecPt)
                {
                    Vector3 vecRel = VecOperations.vecExpressPointInFrame(m_oInputFrame, vecPt);
                    Vector3 vecNew = VecOperations.vecTranslatePointOntoFrame(m_oOutputFrame, vecRel);
                    return vecNew;
                }
            }
        }
    }
}