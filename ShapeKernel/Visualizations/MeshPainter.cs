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
        using static BaseShape;

        public class MeshPainter
		{
            public delegate float ColorScaleFunc(Vector3 vecA, Vector3 vecB, Vector3 vecC);

            /// <summary>
            /// Divides the specified mesh into multiple sub-meshes from triangles that share a similar overhang angle.
            /// Each sub-mesh is previewed in a color that represents its overhang angle on the specified color scale.
            /// Overhang angles are specified in deg. Zero deg is vertical (minimum), 90 deg is horizontal (maximum).
            /// </summary>
            public static void PreviewOverhangAngle(        Mesh            oMesh,
                                                            IColorScale     xScale,
                                                            bool            bShowOnlyDownFacing,
                                                            uint            nClasses = 30)
            {
                Mesh[] aSubMeshes   = new Mesh[nClasses];
                float fMinAngle     = xScale.fGetMinValue();
                float fMaxAngle     = xScale.fGetMaxValue();
                float dAngle        = (fMaxAngle - fMinAngle) / (nClasses - 1f);

                for (int i = 0; i < nClasses; i++)
                {
                    aSubMeshes[i] = new Mesh();
                }

                uint nNumberOfTriangles  = (uint)oMesh.nTriangleCount();
                for (int i = 0; i < nNumberOfTriangles; i++)
                {
                    oMesh.GetTriangle(i, out Vector3 vecA, out Vector3 vecB, out Vector3 vecC);
                    Vector3 vecNormal    = Vector3.Cross(vecA - vecB, vecC - vecB).Normalize();

                    float dR             = MathF.Sqrt(vecNormal.X * vecNormal.X + vecNormal.Y * vecNormal.Y);
                    float dZ             = MathF.Abs(vecNormal.Z);
                    float fOverhangAngle = MathF.Atan2(dZ, dR) / MathF.PI * 180f;
                    fOverhangAngle       = Uf.fLimitValue(fOverhangAngle, 0f, 90f);

                    //only show downfacing
                    if (bShowOnlyDownFacing == true &&
                        vecNormal.Z < 0)
                    {
                        fOverhangAngle = 0;
                    }

                    float fRatio         = (fOverhangAngle - fMinAngle) / (fMaxAngle - fMinAngle);

                    uint nSubMeshIndex   = (uint)(fRatio * (nClasses - 1));
                    aSubMeshes[nSubMeshIndex].nAddTriangle(vecA, vecB, vecC);
                }

                for (int i = 0; i < nClasses; i++)
                {
                    ColorFloat clr      = xScale.clrGetColor(fMinAngle + (i * dAngle));
                    try
                    {
                        Sh.PreviewMesh(aSubMeshes[i], clr);
                    }
                    catch { }
                }
            }

            /// <summary>
            /// Divides the specified mesh into multiple sub-meshes from triangles that share a similar custom properties.
            /// Each sub-mesh is previewed in a color that represents its custom property on the specified color scale.
            /// </summary>
            public static void PreviewCustomProperty(       Mesh            oMesh,
                                                            IColorScale     xScale,
                                                            ColorScaleFunc  oColorFunc,
                                                            uint            nClasses = 30)
            {
                Mesh[] aSubMeshes   = new Mesh[nClasses];
                float fMinValue     = xScale.fGetMinValue();
                float fMaxValue     = xScale.fGetMaxValue();
                float dValue        = (fMaxValue - fMinValue) / (nClasses - 1f);

                for (int i = 0; i < nClasses; i++)
                {
                    aSubMeshes[i] = new Mesh();
                }

                uint nNumberOfTriangles = (uint)oMesh.nTriangleCount();
                for (int i = 0; i < nNumberOfTriangles; i++)
                {
                    oMesh.GetTriangle(i, out Vector3 vecA, out Vector3 vecB, out Vector3 vecC);

                    float fValue        = oColorFunc(vecA, vecB, vecC);
                    float fRatio        = (fValue - fMinValue) / (fMaxValue - fMinValue);
                    fRatio              = Uf.fLimitValue(fRatio, 0f, 1f);

                    uint nSubMeshIndex  = (uint)(fRatio * (nClasses - 1));
                    aSubMeshes[nSubMeshIndex].nAddTriangle(vecA, vecB, vecC);
                }

                for (int i = 0; i < nClasses; i++)
                {
                    ColorFloat clr      = xScale.clrGetColor(fMinValue + (i * dValue));
                    try
                    {
                        Sh.PreviewMesh(aSubMeshes[i], clr);
                    }
                    catch { }
                }
            }

            /// <summary>
            /// Creates a new mesh that is deformed by the vertex-wise application of the trafo func.
            /// Divides this new mesh into multiple sub-meshes based on old mesh triangles that share a similar custom properties.
            /// Each sub-mesh is previewed in a color that represents its custom property on the specified color scale.
            /// </summary>
            public static void PreviewCustomDeformation(    Mesh                    oMesh,
                                                            IColorScale             xScale,
                                                            ColorScaleFunc          oColorFunc,
                                                            fnVertexTransformation  fnTrafo,
                                                            uint                    nClasses = 30)
            {
                Mesh[] aSubMeshes   = new Mesh[nClasses];
                float fMinValue     = xScale.fGetMinValue();
                float fMaxValue     = xScale.fGetMaxValue();
                float dValue        = (fMaxValue - fMinValue) / (nClasses - 1f);

                for (int i = 0; i < nClasses; i++)
                {
                    aSubMeshes[i] = new Mesh();
                }

                uint nNumberOfTriangles = (uint)oMesh.nTriangleCount();
                for (int i = 0; i < nNumberOfTriangles; i++)
                {
                    oMesh.GetTriangle(i, out Vector3 vecA, out Vector3 vecB, out Vector3 vecC);

                    float fValue        = oColorFunc(vecA, vecB, vecC);
                    float fRatio        = (fValue - fMinValue) / (fMaxValue - fMinValue);
                    fRatio              = Uf.fLimitValue(fRatio, 0f, 1f);

                    uint nSubMeshIndex  = (uint)(fRatio * (nClasses - 1));
                    aSubMeshes[nSubMeshIndex].nAddTriangle(vecA, vecB, vecC);
                }

                for (int i = 0; i < nClasses; i++)
                {
                    ColorFloat clr       = xScale.clrGetColor(fMinValue + (i * dValue));
                    try
                    {
                        Mesh msh         = aSubMeshes[i];
                        Mesh mshDeformed = MeshUtility.mshApplyTransformation(msh, fnTrafo);
                        Sh.PreviewMesh(mshDeformed, clr);
                    }
                    catch { }
                }
            }
        }
	}
}