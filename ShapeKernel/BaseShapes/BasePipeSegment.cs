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
        public class BasePipeSegment : BasePipe
        {
            public enum EMethod         { START_END, MID_RANGE };
            protected EMethod           m_eMethod;
            protected LineModulation    m_oStartModulation;
            protected LineModulation    m_oEndModulation;
            protected LineModulation    m_oRangeModulation;
            protected LineModulation    m_oMidModulation;

            /// <summary>
            /// Initialises a pipe segment based on a local frame and 3 dimensions.
            /// For the radial dimension, an inner and an outer radius can be specified.
            /// The circumferential dimension is defined via two line modulation.
            /// The shape has no spine.
            /// </summary>
            public BasePipeSegment(
                LocalFrame      oFrame,
                float           fLength,
                float           fInnerRadius,
                float           fOuterRadius,
                LineModulation  oStartOrMidModulation,
                LineModulation  oEndOrRangeModulation,
                EMethod eMethod) : base(oFrame, fLength, fInnerRadius, fOuterRadius)
            {
                m_eMethod = eMethod;
                if (m_eMethod == EMethod.START_END)
                {
                    m_oStartModulation  = oStartOrMidModulation;
                    m_oEndModulation    = oEndOrRangeModulation;
                    m_oMidModulation    = new LineModulation(1f);
                    m_oRangeModulation  = new LineModulation(1f);
                }
                else
                {
                    m_oMidModulation    = oStartOrMidModulation;
                    m_oRangeModulation  = oEndOrRangeModulation;
                    m_oStartModulation  = new LineModulation(1f);
                    m_oEndModulation    = new LineModulation(1f);
                }
            }

            /// <summary>
            /// Initialises a pipe segment based on a spine (frames) and 2 dimension.
            /// For the radial dimension, an inner and an outer radius can be specified.
            /// The circumferential dimension is defined via two line modulation.
            /// The spine replaces the length dimension.
            /// </summary>
            public BasePipeSegment(
                Frames aFrames,
                float fInnerRadius,
                float fOuterRadius,
                LineModulation oStartOrMidModulation,
                LineModulation oEndOrRangeModulation,
                EMethod eMethod) : base(aFrames, fInnerRadius, fOuterRadius)
            {
                m_eMethod = eMethod;
                if (m_eMethod == EMethod.START_END)
                {
                    m_oStartModulation  = oStartOrMidModulation;
                    m_oEndModulation    = oEndOrRangeModulation;
                    m_oMidModulation    = new LineModulation(1f);
                    m_oRangeModulation  = new LineModulation(1f);
                }
                else
                {
                    m_oMidModulation    = oStartOrMidModulation;
                    m_oRangeModulation  = oEndOrRangeModulation;
                    m_oStartModulation  = new LineModulation(1f);
                    m_oEndModulation    = new LineModulation(1f);
                }
            }


            //construction
            public override Mesh mshConstruct()
            {
                Mesh oMesh = new Mesh();
                AddTopSurface(ref oMesh);
                AddBottomSurface(ref oMesh, true);
                AddInnerMantle(ref oMesh);
                AddOuterMantle(ref oMesh, true);
                AddStartSurface(ref oMesh);
                AddEndSurface(ref oMesh, true);
                return oMesh;
            }


            //sides
            protected void AddStartSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and radius
                int iPhiStep    = 0;
                float fPhiRatio = fGetPhiRatioFromStep(iPhiStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio, fRadiusRatio1);

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

            protected void AddEndSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and radius
                int iPhiStep    = (int)m_nPolarSteps - 1;
                float fPhiRatio = fGetPhiRatioFromStep(iPhiStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iRadiusStep = 1; iRadiusStep < m_nRadialSteps; iRadiusStep++)
                    {
                        float fRadiusRatio1 = fGetRadiusRatioFromStep(iRadiusStep - 1);
                        float fRadiusRatio2 = fGetRadiusRatioFromStep(iRadiusStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio, fRadiusRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fLengthRatio1, fPhiRatio, fRadiusRatio2);
                        Vector3 vecPt2 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio, fRadiusRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fLengthRatio2, fPhiRatio, fRadiusRatio1);

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

            /// <summary>
            /// Returns a point on the shape surface if one of the ratios is at the limit.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// All ratios go from 0 to 1.
            /// </summary>
            public override Vector3 vecGetSurfacePoint(float fLengthRatio, float fPhiRatio, float fRadiusRatio)
            {
                Vector3 vecSpinePos = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX   = m_aFrames.vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalY   = m_aFrames.vecGetLocalYAlongLength(fLengthRatio);

                float fPhiRange     = fGetEndPhi(fLengthRatio) - fGetStartPhi(fLengthRatio);
                float fPhi          = (fPhiRange) * fPhiRatio + fGetStartPhi(fLengthRatio);

                float fOuterRadius  = fGetOuterRadius(fPhi, fLengthRatio);
                float fInnerRadius  = fGetInnerRadius(fPhi, fLengthRatio);
                float fRadius       = fRadiusRatio * (fOuterRadius - fInnerRadius) + fInnerRadius;

                float fX            = fRadius * MathF.Cos(fPhi);
                float fY            = fRadius * MathF.Sin(fPhi);
                Vector3 vecPt       = vecSpinePos + fX * vecLocalX + fY * vecLocalY;

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }

            protected float fGetEndPhi(float fLengthRatio)
            {
                float fPhi          = 0;
                if (m_eMethod == EMethod.START_END)
                {
                    fPhi            = m_oEndModulation.fGetModulation(fLengthRatio);
                }
                else
                {
                    float fMid      = m_oMidModulation.fGetModulation(fLengthRatio);
                    float fRange    = m_oRangeModulation.fGetModulation(fLengthRatio);
                    fPhi            = fMid + 0.5f * fRange;
                }
                return fPhi;
            }

            protected float fGetStartPhi(float fLengthRatio)
            {
                float fPhi = 0;
                if (m_eMethod == EMethod.START_END)
                {
                    fPhi = m_oStartModulation.fGetModulation(fLengthRatio);
                }
                else
                {
                    fPhi = m_oMidModulation.fGetModulation(fLengthRatio) - 0.5f * m_oRangeModulation.fGetModulation(fLengthRatio);
                }
                return fPhi;
            }
        }
    }
}