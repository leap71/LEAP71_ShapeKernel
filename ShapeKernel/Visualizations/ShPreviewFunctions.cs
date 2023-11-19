//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2023 by LEAP 71
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
            public static uint nNumberOfGroups = 0;
            public static void PreviewMesh(
                Mesh        oMesh,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
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
            }

            public static int PreviewVoxels(
                Voxels      oVoxels,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
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

            public static void PreviewLattice(
                Lattice     oLattice,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
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
            }

            public static void PreviewPoint(
                Vector3     vecPt,
                float       fBeam,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
                float       fRoughness      = 0.7f)
            {
                Lattice oLattice = new Lattice();
                oLattice.AddSphere(vecPt, fBeam);
                PreviewLattice(oLattice, clrColor, fTransparency, fMetallic, fRoughness);
            }

            public static void PreviewBeam(
                Vector3     vecPt1,
                float       fBeam1,
                Vector3     vecPt2,
                float       fBeam2,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
                float       fRoughness      = 0.7f)
            {
                Lattice oLattice = new Lattice();
                oLattice.AddBeam(vecPt1, fBeam1, vecPt2, fBeam2, true);
                PreviewLattice(oLattice, clrColor, fTransparency, fMetallic, fRoughness);
            }

            public static void PreviewLine(
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

            public static void PreviewGrid(
                List<List<Vector3>> aGrid,
                ColorFloat          clrColor)
            {
                for (int i = 0; i < aGrid.Count; i++)
                {
                    PreviewLine(aGrid[i], clrColor);
                }
                aGrid = GridOperations.aGetInverseGrid(aGrid);
                for (int i = 0; i < aGrid.Count; i++)
                {
                    PreviewLine(aGrid[i], clrColor);
                }
            }

            public static void PreviewEdges(
                List<List<Vector3>> aEdges,
                ColorFloat          clrColor)
            {
                for (int i = 0; i < aEdges.Count; i++)
                {
                    PreviewLine(aEdges[i], clrColor);
                }
            }

            public static void PreviewPointCloud(
                List<Vector3>   aPoints,
                float           fBeam,
                ColorFloat      clrColor,
                float           fTransparency   = 0.9f,
                float           fMetallic       = 0.3f,
                float           fRoughness      = 0.7f)
            {
                Lattice oLattice = new Lattice();
                for (int i = 0; i < aPoints.Count; i++)
                {
                    oLattice.AddBeam(aPoints[i], fBeam, aPoints[i], fBeam);
                }
                PreviewLattice(oLattice, clrColor, fTransparency, fMetallic, fRoughness);
            }

            public static void PreviewFrame(
                LocalFrame  oFrame,
                float       fSize)
            {
                Lattice oXLattice = new Lattice();
                Lattice oYLattice = new Lattice();
                Lattice oZLattice = new Lattice();

                PreviewFrame(
                    oFrame,
                    fSize,
                    ref oXLattice,
                    ref oYLattice,
                    ref oZLattice);

                PreviewLattice(oXLattice, Cp.clrRed);
                PreviewLattice(oYLattice, Cp.clrGreen);
                PreviewLattice(oZLattice, Cp.clrBlue);
            }

            public static void PreviewFrame(
                LocalFrame  oFrame,
                float       fSize,
                ref Lattice oXLattice,
                ref Lattice oYLattice,
                ref Lattice oZLattice)
            {
                float fBeam1        = 0.4f;
                float fBeam2        = 0.1f;
                Vector3 vecCentre   = oFrame.vecGetPosition();
                Vector3 vecTipX     = vecCentre + fSize * oFrame.vecGetLocalX();
                Vector3 vecTipY     = vecCentre + fSize * oFrame.vecGetLocalY();
                Vector3 vecTipZ     = vecCentre + fSize * oFrame.vecGetLocalZ();

                oXLattice.AddBeam(vecCentre, fBeam1, vecTipX, fBeam2);
                oYLattice.AddBeam(vecCentre, fBeam1, vecTipY, fBeam2);
                oZLattice.AddBeam(vecCentre, fBeam1, vecTipZ, fBeam2);
            }

            public static void PreviewFrames(
                Frames  oFrames,
                float   fSize)
            {
                Lattice oXLattice = new Lattice();
                Lattice oYLattice = new Lattice();
                Lattice oZLattice = new Lattice();

                for (float fLengthRatio = 0; fLengthRatio <= 1f; fLengthRatio += 0.01f)
                {
                    LocalFrame oFrame = oFrames.oGetLocalFrame(fLengthRatio);
                    PreviewFrame(
                        oFrame,
                        fSize,
                        ref oXLattice,
                        ref oYLattice,
                        ref oZLattice);
                }

                PreviewLattice(oXLattice, Cp.clrRed);
                PreviewLattice(oYLattice, Cp.clrGreen);
                PreviewLattice(oZLattice, Cp.clrBlue);
            }

            public static void PreviewCircleSection(
                LocalFrame  oFrame,
                float       fRadius,
                ColorFloat  clrColor,
                float       fTransparency   = 0.9f,
                float       fMetallic       = 0.3f,
                float       fRoughness      = 0.7f)
            {
                Lattice oLattice    = new Lattice();
                Vector3 vecPt1      = oFrame.vecGetPosition();
                Vector3 vecPt2      = vecPt1 + 1f * oFrame.vecGetLocalZ();
                oLattice.AddBeam(vecPt1, fRadius, vecPt2, fRadius, false);
                PreviewLattice(oLattice, clrColor, fTransparency, fMetallic, fRoughness);
            }

            public static void PreviewCircle(
                LocalFrame  oFrame,
                float       fRadius,
                ColorFloat  clrColor)
            {
                List<Vector3> aPoints   = new List<Vector3>();
                uint nSamples           = 100;
                for (uint i = 0; i < nSamples; i++)
                {
                    float fPhi      = 2f * MathF.PI / (float)(nSamples - 1) * i;
                    Vector3 vecRel  = VecOperations.vecGetCylPoint(fRadius, fPhi, 0f);
                    Vector3 vecPt   = VecOperations.vecTranslatePointOntoFrame(oFrame, vecRel);
                    aPoints.Add(vecPt);
                }
                PreviewLine(aPoints, clrColor);
            }

            public static void PreviewCircle(
                Vector3     vecCentre,
                float       fRadius,
                ColorFloat  clrColor)
            {
                LocalFrame oFrame = new LocalFrame(vecCentre);
                PreviewCircle(oFrame, fRadius, clrColor);
            }

            public static void PreviewCylinderWireframe(
                BaseCylinder    oCyl,
                ColorFloat      clrColor,
                uint            nRadialSamples = 5,
                uint            nLengthSamples = 10)
            {
                //length samples
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

                //radial samples
                for (uint nRadialSample = 0; nRadialSample < nRadialSamples; nRadialSample++)
                {
                    uint nSamples           = 1000;
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
            }

            public static void PreviewPipeWireframe(
                BasePipe    oCyl,
                ColorFloat  clrColor,
                uint        nRadialSamples = 5,
                uint        nLengthSamples = 10)
            {
                //length samples
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

                //radial samples
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
            }

            public static void PreviewBoxWireframe(
                BaseBox     oBox,
                ColorFloat  clrColor)
            {
                //lower face
                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, -1, 0),
                    oBox.vecGetSurfacePoint(-1, 1, 0),
                    oBox.vecGetSurfacePoint(1, 1, 0),
                    oBox.vecGetSurfacePoint(1, -1, 0),
                    oBox.vecGetSurfacePoint(-1, -1, 0)
                }, clrColor);


                //upper face
                PreviewLine(new List<Vector3>()
                {
                    oBox.vecGetSurfacePoint(-1, -1, 1),
                    oBox.vecGetSurfacePoint(-1, 1, 1),
                    oBox.vecGetSurfacePoint(1, 1, 1),
                    oBox.vecGetSurfacePoint(1, -1, 1),
                    oBox.vecGetSurfacePoint(-1, -1, 1)
                }, clrColor);

                //side pillars
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

            public static void PreviewBoxWireframe(
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
        }
    }
}