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
        public class BaseLens : BaseShape, IMeshBaseShape, ISurfaceBaseShape
        {
            protected uint              m_nRadialSteps;
            protected uint              m_nPolarSteps;
            protected uint              m_nHeightSteps;
            protected float             m_fInnerRadius;
            protected float             m_fOuterRadius;
            protected SurfaceModulation m_oUpperModulation;
            protected SurfaceModulation m_oLowerModulation;
            protected LocalFrame        m_oFrame;

            /// <summary>
            /// Initialises a lense based on a local frame and 2 dimensions.
            /// For the radial dimension, an inner and an outer radius can be specified.
            /// The shape has no spine.
            /// </summary>
            public BaseLens(
                LocalFrame oFrame,
                float fHeight,
                float fInnerRadius,
                float fOuterRadius) : base()
            {
                m_oFrame = oFrame;
                SetRadialSteps(5);
                SetPolarSteps(360);
                SetHeightSteps(5);

                m_fInnerRadius      = fInnerRadius;
                m_fOuterRadius      = fOuterRadius;
                m_oLowerModulation  = new SurfaceModulation(0);
                m_oUpperModulation  = new SurfaceModulation(fHeight);
                m_bTransformed      = false;
            }


            //settings
            public void SetHeight(SurfaceModulation oLowerModulation, SurfaceModulation oUpperModulation)
            {
                m_oLowerModulation = oLowerModulation;
                m_oUpperModulation = oUpperModulation;
                SetRadialSteps(500);
            }

            public void SetRadialSteps(uint nRadialSteps)
            {
                m_nRadialSteps = Math.Max(5, nRadialSteps);
            }

            public void SetPolarSteps(uint nPolarSteps)
            {
                m_nPolarSteps = Math.Max(5, nPolarSteps);
            }

            public void SetHeightSteps(uint nHeightSteps)
            {
                m_nHeightSteps = Math.Max(5, nHeightSteps);
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
                AddBottomSurface(ref oMesh, true);
                AddInnerMantle(ref oMesh);
                AddOuterMantle(ref oMesh, true);
                return oMesh;
            }


            //sides
            protected void AddTopSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across phi and radius
                float fHeightRatio = 1f;

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fHeightRatio, fPhiRatio1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fHeightRatio, fPhiRatio1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fHeightRatio, fPhiRatio2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fHeightRatio, fPhiRatio2, fRadiusRatio1);

                        if (bFlip == false)
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                        }
                        else
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                            oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                        }
                    }
                }
            }

            protected void AddBottomSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across phi and radius
                float fHeightRatio = 0f;

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fHeightRatio, fPhiRatio1, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fHeightRatio, fPhiRatio1, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fHeightRatio, fPhiRatio2, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fHeightRatio, fPhiRatio2, fRadiusRatio1);

                        if (bFlip == false)
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                        }
                        else
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                            oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                        }
                    }
                }
            }

            protected void AddInnerMantle(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across phi and height
                int iRadiusStep = 0;
                float fRadiusRatio = fGetRadiusRatioFromStep(iRadiusStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iHeightStep = 1; iHeightStep < m_nHeightSteps; iHeightStep++)
                    {
                        float fHeightRatio1 = fGetHeightRatioFromStep(iHeightStep - 1);
                        float fHeightRatio2 = fGetHeightRatioFromStep(iHeightStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fHeightRatio1, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fHeightRatio2, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fHeightRatio2, fPhiRatio2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fHeightRatio1, fPhiRatio2, fRadiusRatio);

                        if (bFlip == false)
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                        }
                        else
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                            oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                        }
                    }
                }
            }

            protected void AddOuterMantle(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across phi and height
                int iRadiusStep     = (int)m_nRadialSteps - 1;
                float fRadiusRatio  = fGetRadiusRatioFromStep(iRadiusStep);

                for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                {
                    float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                    float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                    for (int iHeightStep = 1; iHeightStep < m_nHeightSteps; iHeightStep++)
                    {
                        float fHeightRatio1 = fGetHeightRatioFromStep(iHeightStep - 1);
                        float fHeightRatio2 = fGetHeightRatioFromStep(iHeightStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fHeightRatio1, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fHeightRatio2, fPhiRatio1, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fHeightRatio2, fPhiRatio2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fHeightRatio1, fPhiRatio2, fRadiusRatio);

                        if (bFlip == false)
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                        }
                        else
                        {
                            oMesh.nAddTriangle(vecPt0, vecPt2, vecPt1);
                            oMesh.nAddTriangle(vecPt0, vecPt3, vecPt2);
                        }
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

            protected float fGetHeightRatioFromStep(int iHeightStep)
            {
                float fHeightRatio = (1f) / (m_nHeightSteps - 1) * (iHeightStep);
                return fHeightRatio;
            }

            /// <summary>
            /// Returns a point on the shape surface if one of the ratios is at the limit.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// All ratios go from 0 to 1.
            /// </summary>
            public Vector3 vecGetSurfacePoint(float fHeightRatio, float fPhiRatio, float fRadiusRatio)
            {
                float fPhi      = 2f * MathF.PI * fPhiRatio;
                float fRadius   = (m_fOuterRadius - m_fInnerRadius) * fRadiusRatio + m_fInnerRadius;

                float fZ = fGetHeight(fHeightRatio, fPhi, fRadiusRatio);
                float fX = fRadius * MathF.Cos(fPhi);
                float fY = fRadius * MathF.Sin(fPhi);

                Vector3 vecPt = m_oFrame.vecGetPosition()
                    + fX * m_oFrame.vecGetLocalX()
                    + fY * m_oFrame.vecGetLocalY()
                    + fZ * m_oFrame.vecGetLocalZ();

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }

            protected float fGetHeight(float fHeightRatio, float fPhi, float fRadiusRatio)
            {
                float fLowerHeight  = m_oLowerModulation.fGetModulation(fPhi, fRadiusRatio);
                float fUpperHeight  = m_oUpperModulation.fGetModulation(fPhi, fRadiusRatio);
                float fHeight       = fLowerHeight + fHeightRatio * (fUpperHeight - fLowerHeight);
                return fHeight;
            }
        }
    }
}