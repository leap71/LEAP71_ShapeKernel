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
        public class BaseCylinder : BaseShape, IMeshBaseShape, ISurfaceBaseShape
        {
            protected uint              m_nLengthSteps;
            protected uint              m_nPolarSteps;
            protected uint              m_nRadialSteps;
            protected SurfaceModulation m_oRadiusModulation;
            protected Frames            m_aFrames;

            /// <summary>
            /// Initialises a cylinder based on a local frame and 2 dimensions.
            /// The shape has no spine.
            /// </summary>
            public BaseCylinder(
                LocalFrame oFrame,
                float fLength = 20,
                float fRadius = 10) : base()
            {
                m_aFrames = new Frames(fLength, oFrame);
                SetPolarSteps(360);
                SetRadialSteps(5);
                SetLengthSteps(5);

                m_oRadiusModulation = new SurfaceModulation(fRadius);
                m_bTransformed      = false;
            }

            /// <summary>
            /// Initialises a cylinder based on a spine (frames) and 1 dimension.
            /// The spine replaces the length dimension.
            /// </summary>
            public BaseCylinder(
                Frames aFrames,
                float fRadius = 10) : base()
            {
                m_aFrames = aFrames;
                SetPolarSteps(360);
                SetRadialSteps(5);
                SetLengthSteps(500);

                m_oRadiusModulation = new SurfaceModulation(fRadius);
                m_bTransformed      = false;
            }


            //settings
            public void SetRadius(SurfaceModulation oModulation)
            {
                m_oRadiusModulation = oModulation;
                SetLengthSteps(500);
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


            //construction
            public override Voxels voxConstruct()
            {
                Mesh oMesh      = mshConstruct();
                Voxels oVoxels  = new Voxels(oMesh);
                return oVoxels;
            }

            public Mesh mshConstruct()
            {
                Mesh oMesh = new Mesh();
                AddTopSurface(ref oMesh);
                AddOuterMantle(ref oMesh);
                AddBottomSurface(ref oMesh);
                return oMesh;
            }


            //sides
            protected void AddTopSurface(ref Mesh oMesh)
            {
                //iterate across phi and radius
                int iLengthStep     = (int)m_nLengthSteps - 1;
                float fLengthRatio  = fGetLengthRatioFromStep(iLengthStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio, fPhiRatio1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio, fPhiRatio1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio, fPhiRatio2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio, fPhiRatio2, fRadiusRatio1);
                        oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                    }
                }
            }

            protected void AddBottomSurface(ref Mesh oMesh)
            {
                //iterate across phi and radius
                int iLengthStep     = 0;
                float fLengthRatio  = fGetLengthRatioFromStep(iLengthStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio, fPhiRatio1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio, fPhiRatio1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio, fPhiRatio2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio, fPhiRatio2, fRadiusRatio1);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                        oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                    }
                }
            }

            protected void AddOuterMantle(ref Mesh oMesh)
            {
                //iterate across phi and length
                int iRadiusStep     = (int)m_nRadialSteps - 1;
                float fRadiusRatio  = fGetRadiusRatioFromStep(iRadiusStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                    {
                        float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                        float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio2, fRadiusRatio);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                        oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
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
                float fPhi = (1f) / (m_nPolarSteps - 1) * (iPhiStep);
                return fPhi;
            }

            protected float fGetLengthRatioFromStep(int iLengthStep)
            {
                float fLengthRatio = (1f) / (m_nLengthSteps - 1) * (iLengthStep);
                return fLengthRatio;
            }

            /// <summary>
            /// Returns a point on the shape surface if one of the ratios is at the limit.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// All ratios go from 0 to 1.
            /// </summary>
            public Vector3 vecGetSurfacePoint(float fLengthRatio, float fPhiRatio, float fRadiusRatio)
            {
                float fPhi = 2f * MathF.PI * fPhiRatio;

                Vector3 vecSpinePos = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX   = m_aFrames.vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalY   = m_aFrames.vecGetLocalYAlongLength(fLengthRatio);

                float fRadius       = fRadiusRatio * fGetRadius(fPhi, fLengthRatio);

                float fX = fRadius * MathF.Cos(fPhi);
                float fY = fRadius * MathF.Sin(fPhi);
                Vector3 vecPt = vecSpinePos + fX * vecLocalX + fY * vecLocalY;

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }

            protected float fGetRadius(float fPhi, float fLengthRatio)
            {
                float fRadius = m_oRadiusModulation.fGetModulation(fPhi, fLengthRatio);
                return fRadius;
            }
        }
    }
}