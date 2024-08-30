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
        public class BaseRevolve : BaseShape
        {
            protected uint           m_nLengthSteps;
            protected uint           m_nPolarSteps;
            protected uint           m_nRadialSteps;
            protected Frames         m_aFrames;
            protected LocalFrame     m_oFrame;
            protected LineModulation m_oOuterRadiusModulation;
            protected LineModulation m_oInnerRadiusModulation;


            /// <summary>
            /// Base shape that has always a spine.
            /// Inward and Outward radius are counted positive from the spine away.
            /// Can only rotate around the Z-axis.
            /// </summary>
            public BaseRevolve(
                LocalFrame  oFrame,
                Frames      aFrames,
                float       fInwardRadius   = 3,
                float       fOutwardRadius  = 3)
            {
                m_oFrame                 = oFrame;
                SetRadialSteps(100);
                SetPolarSteps(360);
                SetLengthSteps(500);
                m_oOuterRadiusModulation = new LineModulation(fOutwardRadius);
                m_oInnerRadiusModulation = new LineModulation(fInwardRadius);
                m_aFrames                = aFrames;
            }

            public void SetRadius(LineModulation oInnerRadiusOverCylinder, LineModulation oOuterRadiusOverCylinder)
            {
                m_oInnerRadiusModulation = oInnerRadiusOverCylinder;
                m_oOuterRadiusModulation = oOuterRadiusOverCylinder;
            }

            public void SetRadialSteps(uint nRadialSteps)
            {
                m_nRadialSteps = Math.Max(5, nRadialSteps);
            }

            public void SetPolarSteps(uint nPolarSteps)
            {
                m_nPolarSteps = Math.Max(5, nPolarSteps);
            }

            public void SetLengthSteps(uint nLengthSteps)
            {
                m_nLengthSteps = Math.Max(5, nLengthSteps);
            }

            protected float fGetInnerRadius(float fLengthRatio)
            {
                float fRadius = m_oInnerRadiusModulation.fGetModulation(fLengthRatio);
                return fRadius;
            }

            protected float fGetOuterRadius(float fLengthRatio)
            {
                float fRadius = m_oOuterRadiusModulation.fGetModulation(fLengthRatio);
                return fRadius;
            }

            //construction
            public override Voxels voxConstruct()
            {
                Mesh oMesh = mshConstruct();
                return new Voxels(oMesh);
            }

            public Mesh mshConstruct()
            {
                Mesh oMesh = new Mesh();
                AddTopSurface(ref oMesh);
                AddInnerMantle(ref oMesh);
                AddOuterMantle(ref oMesh);
                AddBottomSurface(ref oMesh);
                return oMesh;
            }

            protected void AddTopSurface(ref Mesh oMesh)
            {
                //iterate across phi and radius
                int iLengthStep = (int)m_nLengthSteps - 1;
                float fLengthRatio = fGetLengthRatioFromStep(iLengthStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhi1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhi2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio, fPhi1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio, fPhi1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio, fPhi2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio, fPhi2, fRadiusRatio1);
                        oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                    }
                }
            }

            protected void AddBottomSurface(ref Mesh oMesh)
            {
                //iterate across phi and radius
                int iLengthStep = 0;
                float fLengthRatio = fGetLengthRatioFromStep(iLengthStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhi1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhi2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio, fPhi1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio, fPhi1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio, fPhi2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio, fPhi2, fRadiusRatio1);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                        oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                    }
                }
            }

            protected void AddOuterMantle(ref Mesh oMesh)
            {
                //iterate across phi and length
                int iRadiusStep = (int)m_nRadialSteps - 1;
                float fRadiusRatio = fGetRadiusRatioFromStep(iRadiusStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhi1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhi2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                    {
                        float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                        float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio1, fPhi1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio2, fPhi1, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio2, fPhi2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio1, fPhi2, fRadiusRatio);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                        oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                    }
                }
            }

            protected void AddInnerMantle(ref Mesh oMesh)
            {
                //iterate across phi and length
                int iRadiusStep = 0;
                float fRadiusRatio = fGetRadiusRatioFromStep(iRadiusStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhi1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhi2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                    {
                        float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                        float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio1, fPhi1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio2, fPhi1, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio2, fPhi2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio1, fPhi2, fRadiusRatio);
                        oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                    }
                }
            }


            //step conversions
            protected float fGetRadiusRatioFromStep(int iRadiusStep)
            {
                float fRadiusRatio = (1f) / (m_nRadialSteps - 1) * (iRadiusStep);
                return fRadiusRatio;
            }

            protected float fGetPhiRatioFromStep(int iPhiStep)
            {
                float fPhiRatio = (1f) / (m_nPolarSteps - 1) * (iPhiStep);
                return fPhiRatio;
            }

            protected float fGetLengthRatioFromStep(int iLengthStep)
            {
                float fLengthRatio = (1f) / (m_nLengthSteps - 1) * (iLengthStep);
                return fLengthRatio;
            }


            public Vector3 vecGetSurfacePoint(float fLengthRatio, float fPhiRatio, float fRadiusRatio)
            {
                Vector3 vecSpinePos     = vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX       = vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalY       = vecGetLocalYAlongLength(fLengthRatio);

                float fPhi              = (2f * MathF.PI) * fPhiRatio;

                float fOutwardRadius    = fGetOuterRadius(fLengthRatio);
                float fInwardRadius     = -fGetInnerRadius(fLengthRatio);
                float fRadius           = fRadiusRatio * (fOutwardRadius - fInwardRadius) + fInwardRadius;

                Vector3 vecPt           = vecSpinePos + fRadius * vecLocalX;
                vecPt                   = VecOperations.vecRotateAroundAxis(vecPt, fPhi, m_oFrame.vecGetLocalZ(), m_oFrame.vecGetPosition());

                return m_fnTrafo(vecPt);
            }

            public Vector3 vecGetSpineAlongLength(float fLengthRatio)
            {
                Vector3 vecVector = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                return vecVector;
            }

            protected Vector3 vecGetLocalXAlongLength(float fLengthRatio)
            {
                Vector3 vecVector = m_aFrames.vecGetLocalXAlongLength(fLengthRatio);
                return vecVector;
            }

            protected Vector3 vecGetLocalYAlongLength(float fLengthRatio)
            {
                Vector3 vecVector = m_aFrames.vecGetLocalYAlongLength(fLengthRatio);
                return vecVector;
            }

            //access
            public Vector3 vecGetOuterSurfacePoint(float fPhi, float fLengthRatio)
            {
                return vecGetSurfacePoint(fLengthRatio, fPhi / (2f * MathF.PI), 1f);
            }

            public Vector3 vecGetInnerSurfacePoint(float fPhi, float fLengthRatio)
            {
                return vecGetSurfacePoint(fLengthRatio, fPhi / (2f * MathF.PI), 0f);
            }

            public static Frames aGetFramesFromContour(GenericContour oContour, LocalFrame? oFrame = null)
            {
                if (oFrame == null)
                {
                    oFrame = new LocalFrame();
                }

                uint nSamples = 500;
                List<Vector3> aPoints = new List<Vector3>();
                for (int i = 0; i < nSamples; i++)
                {
                    float fLR = 1f / (nSamples - 1f) * i;
                    float fZ = fLR * oContour.m_fTotalLength;
                    float fRadius = oContour.m_oModulation.fGetModulation(fLR);
                    Vector3 vecRel = new Vector3(fRadius, 0, fZ);
                    Vector3 vecPt = VecOperations.vecTranslatePointOntoFrame(oFrame, vecRel);
                    aPoints.Add(vecPt);
                }
                Frames aFrames = new Frames(aPoints, Frames.EFrameType.CYLINDRICAL, 0.5f);
                return aFrames;
            }
        }
    }
}