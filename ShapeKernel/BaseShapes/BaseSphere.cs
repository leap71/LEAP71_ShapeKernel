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
        public class BaseSphere : BaseShape, IMeshBaseShape, ISurfaceBaseShape
        {
            protected uint              m_nAzimuthalSteps;
            protected uint              m_nPolarSteps;
            protected SurfaceModulation m_oRadiusModulation;
            protected LocalFrame        m_oFrame;

            /// <summary>
            /// Initialises a sphere based on a local frame and 1 dimensions.
            /// The shape has no spine.
            /// </summary>
            public BaseSphere(LocalFrame oFrame, float fRadius = 10) : base()
            {
                //phi is the azimuthal angle
                //theta is the polar angle
                SetAzimuthalSteps(360);
                SetPolarSteps(180);
                m_oFrame            = oFrame;
                m_oRadiusModulation = new SurfaceModulation(fRadius);
                m_bTransformed      = false;
            }


            //settings
            public void SetRadius(SurfaceModulation oModulation)
            {
                m_oRadiusModulation = oModulation;
            }

            public void SetAzimuthalSteps(uint nAzimuthalSteps)
            {
                m_nAzimuthalSteps = nAzimuthalSteps;
            }

            public void SetPolarSteps(uint nPolarSteps)
            {
                m_nPolarSteps = nPolarSteps;
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
                for (int iThetaStep = 1; iThetaStep < m_nAzimuthalSteps; iThetaStep++)
                {
                    float fThetaRatio1 = (1f) / (m_nAzimuthalSteps - 1) * (iThetaStep - 1);
                    float fThetaRatio2 = (1f) / (m_nAzimuthalSteps - 1) * (iThetaStep);

                    for (int iPhiStep = 0; iPhiStep < m_nPolarSteps; iPhiStep++)
                    {
                        float fPhiRatio1 = (1f) / (m_nPolarSteps - 1) * (iPhiStep - 1);
                        float fPhiRatio2 = (1f) / (m_nPolarSteps - 1) * (iPhiStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fPhiRatio1, fThetaRatio1, fRadiusRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fPhiRatio1, fThetaRatio2, fRadiusRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fPhiRatio2, fThetaRatio2, fRadiusRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fPhiRatio2, fThetaRatio1, fRadiusRatio);
                        oMesh.nAddTriangle(vecPt0, vecPt1, vecPt2);
                        oMesh.nAddTriangle(vecPt0, vecPt2, vecPt3);
                    }
                }
                return oMesh;
            }

            protected float fGetRadius(float fPhi, float fLengthRatio)
            {
                float fRadius = m_oRadiusModulation.fGetModulation(fPhi, fLengthRatio);
                return fRadius;
            }

            /// <summary>
            /// Returns a point on the shape surface if radius ratio = 1.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// All ratios go from 0 to 1.
            /// </summary>
            public Vector3 vecGetSurfacePoint(float fPhiRatio, float fThetaRatio, float fRadiusRatio)
            {
                float fTheta    = 1f * MathF.PI * fThetaRatio;
                float fPhi      = 2f * MathF.PI * fPhiRatio;

                Vector3 vecPos  = m_oFrame.vecGetPosition();
                float fRadius   = fRadiusRatio * fGetRadius(fPhi, fTheta);
                float fX        = fRadius * MathF.Cos(fPhi) * MathF.Sin(fTheta);
                float fY        = fRadius * MathF.Sin(fPhi) * MathF.Sin(fTheta);
                float fZ        = fRadius * MathF.Cos(fTheta);
                Vector3 vecPt   = vecPos
                    + fX * m_oFrame.vecGetLocalX()
                    + fY * m_oFrame.vecGetLocalY()
                    + fZ * m_oFrame.vecGetLocalZ();

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }
        }
    }
}