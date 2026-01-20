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
        public partial class Sh
        {            
            public static int Preview(
                Mesh        oMesh,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.4f,
                float       fRoughness      = 0.7f)
            {
                int iNextGroupId    = (int)nNumberOfGroups;
                ColorFloat clr      = new ColorFloat(clrColor, fTransparency);
                Library.oViewer().SetGroupMaterial(iNextGroupId,
                                            clr,
                                            fMetallic,
                                            fRoughness);
                Library.oViewer().Add(oMesh, iNextGroupId);
                nNumberOfGroups++;
                return (int)(nNumberOfGroups - 1);
            }

            public static int Preview(
                Voxels      oVoxels,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.4f,
                float       fRoughness      = 0.7f)
            {
                int iNextGroupId    = (int)nNumberOfGroups;
                ColorFloat clr      = new ColorFloat(clrColor, fTransparency);
                Library.oViewer().SetGroupMaterial(iNextGroupId,
                                            clr,
                                            fMetallic,
                                            fRoughness);
                Library.oViewer().Add(oVoxels, iNextGroupId);
                nNumberOfGroups++;
                return (int)(nNumberOfGroups - 1);
            }

            public static int Preview(
                Lattice     oLattice,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.4f,
                float       fRoughness      = 0.7f)
            {
                int iNextGroupId    = (int)nNumberOfGroups;
                ColorFloat clr      = new ColorFloat(clrColor, fTransparency);
                Library.oViewer().SetGroupMaterial(iNextGroupId,
                                            clr,
                                            fMetallic,
                                            fRoughness);
                Voxels oVoxels = new Voxels(oLattice);
                Library.oViewer().Add(oVoxels, iNextGroupId);
                nNumberOfGroups++;
                return (int)(nNumberOfGroups - 1);
            }

            public static void Preview(
                Vector3     vecPt,
                float       fBeam,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.4f,
                float       fRoughness      = 0.7f)
            {
                BaseSphere oBall = new (new (vecPt), fBeam);
                PreviewMesh(oBall.mshConstruct(), clrColor, fTransparency, fMetallic, fRoughness);
            }

            public static void Preview(
                List<Vector3>   aPoints,
                ColorFloat      clrColor)
            {
                int iNextGroupId = (int)nNumberOfGroups;
                PolyLine oLine = new PolyLine(clrColor);
                for (int i = 0; i < aPoints.Count; i++)
                {
                    oLine.nAddVertex(aPoints[i]);
                }
                Library.oViewer().Add(oLine, iNextGroupId);
                nNumberOfGroups++;
            }

            public static void Preview(
                LocalFrame  oFrame,
                float       fSize)
            {
                PolyLine oXLine = new(Cp.clrRed);
                oXLine.nAddVertex(oFrame.vecGetPosition());
                oXLine.nAddVertex(oFrame.vecGetPosition() + fSize * oFrame.vecGetLocalX());
                oXLine.AddArrow(0.2f * fSize);

                PolyLine oYLine = new(Cp.clrGreen);
                oYLine.nAddVertex(oFrame.vecGetPosition());
                oYLine.nAddVertex(oFrame.vecGetPosition() + fSize * oFrame.vecGetLocalY());
                oYLine.AddArrow(0.2f * fSize);

                PolyLine oZLine = new(Cp.clrBlue);
                oZLine.nAddVertex(oFrame.vecGetPosition());
                oZLine.nAddVertex(oFrame.vecGetPosition() + fSize * oFrame.vecGetLocalZ());
                oZLine.AddArrow(0.2f * fSize);

                PreviewPoint(oFrame.vecGetPosition(), 0.5f, Cp.clrBlack);
                Library.oViewer().Add(oXLine);
                Library.oViewer().Add(oYLine);
                Library.oViewer().Add(oZLine);
            }

            public static void Preview(
                Frames  aFrames,
                float   fSize)
            {
                float fTotalLength  = SplineOperations.fGetTotalLength(aFrames.aGetPoints(100));
                uint nSamples       = (uint)(2f * fTotalLength / fSize);
                for (int i = 0; i < nSamples; i++)
                {
                    float fLR           = 1f / (nSamples - 1f) * i;
                    LocalFrame oFrame   = aFrames.oGetLocalFrame(fLR);
                    PreviewFrame(oFrame, fSize);
                }
            }

            public static void Preview(
                BaseCylinder    oCyl,
                ColorFloat      clrColor,
                uint            nRadialSamples = 4,
                uint            nLengthSamples = 10)
            {
                // length samples
                for (uint nLengthSample = 0; nLengthSample < nLengthSamples; nLengthSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 1f;
                    float fLengthRatio      = 1f / (float)(nLengthSamples - 1) * nLengthSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fPhiRatio     = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }

                // radial samples
                for (uint nRadialSample = 0; nRadialSample < nRadialSamples; nRadialSample++)
                {
                    uint nSamples           = 1000;
                    float fRadiusRatio      = 1f;
                    float fPhiRatio         = 1f / (float)(nRadialSamples) * nRadialSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fLengthRatio  = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }
            }

            public static void Preview(
                BasePipe    oCyl,
                ColorFloat  clrColor,
                uint        nRadialSamples = 4,
                uint        nLengthSamples = 10)
            {
                // length samples
                for (uint nLengthSample = 0; nLengthSample < nLengthSamples; nLengthSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 1f;
                    float fLengthRatio      = 1f / (float)(nLengthSamples - 1) * nLengthSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fPhiRatio     = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }

                // length samples
                for (uint nLengthSample = 0; nLengthSample < nLengthSamples; nLengthSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 0f;
                    float fLengthRatio      = 1f / (float)(nLengthSamples - 1) * nLengthSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fPhiRatio     = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }

                // radial samples
                for (uint nRadialSample = 0; nRadialSample < nRadialSamples; nRadialSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 1f;
                    float fPhiRatio         = 1f / (float)(nRadialSamples - 1) * nRadialSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fLengthRatio  = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }

                // radial samples
                for (uint nRadialSample = 0; nRadialSample < nRadialSamples; nRadialSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 0f;
                    float fPhiRatio         = 1f / (float)(nRadialSamples - 1) * nRadialSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fLengthRatio  = 1f / (float)(nSamples - 1) * i;
                        aPoints.Add(oCyl.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clrColor);
                }
            }

            public static void Preview(
                BaseBox     oBox,
                ColorFloat  clrColor)
            {
                // lower face
                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, -1, 0),
                    oBox.vecGetSurfacePoint(-1,  1, 0),
                    oBox.vecGetSurfacePoint( 1,  1, 0),
                    oBox.vecGetSurfacePoint( 1, -1, 0),
                    oBox.vecGetSurfacePoint(-1, -1, 0)
                }, clrColor);


                // upper face
                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, -1, 1),
                    oBox.vecGetSurfacePoint(-1,  1, 1),
                    oBox.vecGetSurfacePoint( 1,  1, 1),
                    oBox.vecGetSurfacePoint( 1, -1, 1),
                    oBox.vecGetSurfacePoint(-1, -1, 1)
                }, clrColor);

                // side pillars
                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, -1, 0),
                    oBox.vecGetSurfacePoint(-1, -1, 1),
                }, clrColor);

                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, 1, 0),
                    oBox.vecGetSurfacePoint(-1, 1, 1),
                }, clrColor);

                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(1, 1, 0),
                    oBox.vecGetSurfacePoint(1, 1, 1),
                }, clrColor);

                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(1, -1, 0),
                    oBox.vecGetSurfacePoint(1, -1, 1),
                }, clrColor);
            }

            public static void Preview(
                BBox3       oBBox,
                ColorFloat  clrColor)
            {
                Vector3 vecBase = VecOperations.vecSetZ(0.5f * (oBBox.vecMax + oBBox.vecMin), oBBox.vecMin.Z);
                float fLength   = (oBBox.vecMax - oBBox.vecMin).Z;
                float fWidth    = (oBBox.vecMax - oBBox.vecMin).X;
                float fDepth    = (oBBox.vecMax - oBBox.vecMin).Y;
                BaseBox oBox    = new BaseBox(new LocalFrame(vecBase), fLength, fWidth, fDepth);
                PreviewBoxWireframe(oBox, clrColor);
            }

            public static void Preview(
                BaseRing    oRing, 
                ColorFloat  clr,
                uint        nRadialSamples = 4,
                uint        nLengthSamples = 10)
            {
                // length samples
                for (uint nLengthSample = 0; nLengthSample < nLengthSamples; nLengthSample++)
                {
                    uint nSamples           = 100;
                    float fRadiusRatio      = 1f;
                    float fLengthRatio      = 1f / (nLengthSamples - 1f) * nLengthSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fPhiRatio     = 1f / (nSamples - 1f) * i;
                        aPoints.Add(oRing.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clr);
                }

                // radial samples
                for (uint nRadialSample = 0; nRadialSample < nRadialSamples; nRadialSample++)
                {
                    uint nSamples           = 500;
                    float fRadiusRatio      = 1f;
                    float fPhiRatio         = 1f / (nRadialSamples - 1f) * nRadialSample;
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint i = 0; i < nSamples; i++)
                    {
                        float fLengthRatio  = 1f / (nSamples - 1f) * i;
                        aPoints.Add(oRing.vecGetSurfacePoint(fLengthRatio, fPhiRatio, fRadiusRatio));
                    }
                    PreviewLine(aPoints, clr);
                }
            }
        }
    }
}