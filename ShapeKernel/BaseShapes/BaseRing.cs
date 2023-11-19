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
        public class BaseRing : BaseShape, IMeshBaseShape, ISurfaceBaseShape
        {
            protected uint              m_nPolarSteps;
            protected uint              m_nRadialSteps;
            protected float             m_fRingRadius;
            protected SurfaceModulation m_oRadiusModulation;
            protected LocalFrame        m_oFrame;

            /// <summary>
            /// Initialises a torus-ring based on a local frame and 2 dimensions.
            /// The shape has no spine.
            /// </summary>
            public BaseRing(LocalFrame oFrame, float fRingRadius = 50, float fRadius = 5) : base()
            {
                SetRadialSteps(360);
                SetPolarSteps(360);

                m_oFrame            = oFrame;
                m_fRingRadius       = fRingRadius;
                m_oRadiusModulation = new SurfaceModulation(fRadius);
                m_bTransformed      = false;
            }


            //settings
            public void SetRadius(SurfaceModulation oModulation)
            {
                m_oRadiusModulation = oModulation;
            }

            public void SetRadialSteps(uint nRadialSteps)
            {
                m_nRadialSteps = Math.Max(5, nRadialSteps);
            }

            public void SetPolarSteps(uint nPolarSteps)
            {
                m_nPolarSteps = Math.Max(5, nPolarSteps);
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
                Mesh oMesh          = new Mesh();
                float fRadiusRatio  = 1f;
                for (int iAlphaStep = 1; iAlphaStep < m_nRadialSteps; iAlphaStep++)
                {
                    float fAlphaRatio1 = fGetAlphaRatioFromStep(iAlphaStep - 1);
                    float fAlphaRatio2 = fGetAlphaRatioFromStep(iAlphaStep);

                    for (int iPhiStep = 1; iPhiStep < m_nPolarSteps; iPhiStep++)
                    {
                        float fPhiRatio1 = fGetPhiRatioFromStep(iPhiStep - 1);
                        float fPhiRatio2 = fGetPhiRatioFromStep(iPhiStep);

                        Vector3 vec0 = vecGetSurfacePoint(fAlphaRatio1, fPhiRatio1, fRadiusRatio);
                        Vector3 vec1 = vecGetSurfacePoint(fAlphaRatio2, fPhiRatio1, fRadiusRatio);
                        Vector3 vec2 = vecGetSurfacePoint(fAlphaRatio2, fPhiRatio2, fRadiusRatio);
                        Vector3 vec3 = vecGetSurfacePoint(fAlphaRatio1, fPhiRatio2, fRadiusRatio);

                        oMesh.nAddTriangle(vec0, vec1, vec2);
                        oMesh.nAddTriangle(vec0, vec2, vec3);
                    }
                }
                return oMesh;
            }


            //step conversions
            protected float fGetAlphaRatioFromStep(int iAlphaStep)
            {
                float fAlpha = (1f) / (m_nRadialSteps - 1) * (iAlphaStep);
                return fAlpha;
            }

            protected float fGetPhiRatioFromStep(int iPhiStep)
            {
                float fPhi = (1f) / (m_nPolarSteps - 1) * (iPhiStep);
                return fPhi;
            }

            /// <summary>
            /// Returns a point on the shape surface if radius ratio = 1.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// All ratios go from 0 to 1.
            /// </summary>
            public Vector3 vecGetSurfacePoint(float fAlphaRatio, float fPhiRatio, float fRadiusRatio)
            {
                float fAlpha    = 2f * MathF.PI * fAlphaRatio;
                float fPhi      = 2f * MathF.PI * fPhiRatio;

                float fX        = m_fRingRadius * MathF.Cos(fAlpha);
                float fY        = m_fRingRadius * MathF.Sin(fAlpha);

                Vector3 vecSpine    =
                    m_oFrame.vecGetPosition() +
                    fX * m_oFrame.vecGetLocalX() +
                    fY * m_oFrame.vecGetLocalY();

                Vector3 vecCentre   = m_oFrame.vecGetPosition();

                Vector3 vecLocalX   = (vecSpine - vecCentre);
                vecLocalX           = vecLocalX.Normalize();
                Vector3 vecLocalY   = m_oFrame.vecGetLocalZ();
                Vector3 vecLocalZ   = Vector3.Cross(vecLocalY, vecLocalX);

                float fRadius       = fRadiusRatio * fGetRadius(fPhi, fAlpha);

                float fLocalX       = fRadius * MathF.Cos(fPhi);
                float fLocalY       = fRadius * MathF.Sin(fPhi);
                Vector3 vecPt       = vecSpine + fLocalX * vecLocalX + fLocalY * vecLocalY;

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