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
        public class BaseBox : BaseShape, IMeshBaseShape,ISurfaceBaseShape
        {
            protected uint              m_nLengthSteps;
            protected uint              m_nWidthSteps;
            protected uint              m_nDepthSteps;
            protected LineModulation    m_oWidthModulation;
            protected LineModulation    m_oDepthModulation;
            protected Frames            m_aFrames;

            /// <summary>
            /// Initialises a box based on a local frame and 3 dimensions.
            /// The shape has no spine.
            /// Width correlates to local x dimension.
            /// Depth correlates to local y dimension.
            /// </summary>
            public BaseBox(
                LocalFrame oFrame,
                float fLength = 20,
                float fWidth = 20,
                float fDepth = 20) : base()
            {
                m_aFrames = new Frames(fLength, oFrame);
                SetWidthSteps(5);
                SetDepthSteps(5);
                SetLengthSteps(5);

                m_oWidthModulation  = new LineModulation(fWidth);
                m_oDepthModulation  = new LineModulation(fDepth);
                m_bTransformed      = false;
            }

            protected BaseBox() { }

            /// <summary>
            /// Initialises a box based on a spine (frames) and 2 dimensions.
            /// The spine replaces the length dimension.
            /// Width correlates to local x dimension.
            /// Depth correlates to local y dimension.
            /// </summary>
            public BaseBox(
                Frames aFrames,
                float fWidth = 20,
                float fDepth = 20) : base()
            {
                m_aFrames = aFrames;
                SetWidthSteps(5);
                SetDepthSteps(5);
                SetLengthSteps(500);

                m_oWidthModulation  = new LineModulation(fWidth);
                m_oDepthModulation  = new LineModulation(fDepth);
                m_bTransformed      = false;
            }


            //settings
            public void SetWidth(LineModulation oModulation)
            {
                m_oWidthModulation = oModulation;
                SetWidthSteps(500);
                SetLengthSteps(500);
            }

            public void SetDepth(LineModulation oModulation)
            {
                m_oDepthModulation = oModulation;
                SetDepthSteps(500);
                SetLengthSteps(500);
            }

            public void SetWidthSteps(uint nWidthSteps)
            {
                m_nWidthSteps = Math.Max(5, nWidthSteps);
            }

            public void SetDepthSteps(uint nDepthSteps)
            {
                m_nDepthSteps = Math.Max(5, nDepthSteps);
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
                AddTopSurface(ref oMesh, true);
                AddBottomSurface(ref oMesh);
                AddFrontSurface(ref oMesh, true);
                AddBackSurface(ref oMesh);
                AddRightSurface(ref oMesh, true);
                AddLeftSurface(ref oMesh);
                return oMesh;
            }

            //sides
            protected void AddTopSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across width and depth
                int iLengthStep     = (int)m_nLengthSteps - 1;
                float fLengthRatio  = fGetLengthRatioFromStep(iLengthStep);

                for (int iWidthStep = 1; iWidthStep < m_nWidthSteps; iWidthStep++)
                {
                    float fWidthRatio1 = fGetWidthRatioFromStep(iWidthStep - 1);
                    float fWidthRatio2 = fGetWidthRatioFromStep(iWidthStep);

                    for (int iDepthStep = 1; iDepthStep < m_nDepthSteps; iDepthStep++)
                    {
                        float fDepthRatio1 = fGetDepthRatioFromStep(iDepthStep - 1);
                        float fDepthRatio2 = fGetDepthRatioFromStep(iDepthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio1, fLengthRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio2, fLengthRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio2, fLengthRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio1, fLengthRatio);

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
                //iterate across width and depth
                int iLengthStep     = 0;
                float fLengthRatio  = fGetLengthRatioFromStep(iLengthStep);

                for (int iWidthStep = 1; iWidthStep < m_nWidthSteps; iWidthStep++)
                {
                    float fWidthRatio1 = fGetWidthRatioFromStep(iWidthStep - 1);
                    float fWidthRatio2 = fGetWidthRatioFromStep(iWidthStep);

                    for (int iDepthStep = 1; iDepthStep < m_nDepthSteps; iDepthStep++)
                    {
                        float fDepthRatio1 = fGetDepthRatioFromStep(iDepthStep - 1);
                        float fDepthRatio2 = fGetDepthRatioFromStep(iDepthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio1, fLengthRatio);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio2, fLengthRatio);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio2, fLengthRatio);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio1, fLengthRatio);

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

            protected void AddFrontSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and depth
                int iWidthStep      = 0;
                float fWidthRatio   = fGetWidthRatioFromStep(iWidthStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iDepthStep = 1; iDepthStep < m_nDepthSteps; iDepthStep++)
                    {
                        float fDepthRatio1 = fGetDepthRatioFromStep(iDepthStep - 1);
                        float fDepthRatio2 = fGetDepthRatioFromStep(iDepthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio, fDepthRatio1, fLengthRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio, fDepthRatio2, fLengthRatio1);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio, fDepthRatio2, fLengthRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio, fDepthRatio1, fLengthRatio2);

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

            protected void AddBackSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and depth
                int iWidthStep      = (int)m_nWidthSteps - 1;
                float fWidthRatio   = fGetWidthRatioFromStep(iWidthStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iDepthStep = 1; iDepthStep < m_nDepthSteps; iDepthStep++)
                    {
                        float fDepthRatio1 = fGetDepthRatioFromStep(iDepthStep - 1);
                        float fDepthRatio2 = fGetDepthRatioFromStep(iDepthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio, fDepthRatio1, fLengthRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio, fDepthRatio2, fLengthRatio1);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio, fDepthRatio2, fLengthRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio, fDepthRatio1, fLengthRatio2);

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

            protected void AddLeftSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and width
                int iDepthStep      = 0;
                float fDepthRatio   = fGetDepthRatioFromStep(iDepthStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iWidthStep = 1; iWidthStep < m_nWidthSteps; iWidthStep++)
                    {
                        float fWidthRatio1 = fGetWidthRatioFromStep(iWidthStep - 1);
                        float fWidthRatio2 = fGetWidthRatioFromStep(iWidthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio, fLengthRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio, fLengthRatio1);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio, fLengthRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio, fLengthRatio2);

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

            protected void AddRightSurface(ref Mesh oMesh, bool bFlip = false)
            {
                //iterate across length and width
                int iDepthStep      = (int)m_nDepthSteps - 1;
                float fDepthRatio   = fGetDepthRatioFromStep(iDepthStep);

                for (int iLengthStep = 1; iLengthStep < m_nLengthSteps; iLengthStep++)
                {
                    float fLengthRatio1 = fGetLengthRatioFromStep(iLengthStep - 1);
                    float fLengthRatio2 = fGetLengthRatioFromStep(iLengthStep);

                    for (int iWidthStep = 1; iWidthStep < m_nWidthSteps; iWidthStep++)
                    {
                        float fWidthRatio1 = fGetWidthRatioFromStep(iWidthStep - 1);
                        float fWidthRatio2 = fGetWidthRatioFromStep(iWidthStep);

                        Vector3 vecPt0 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio, fLengthRatio1);
                        Vector3 vecPt1 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio, fLengthRatio1);
                        Vector3 vecPt2 = vecGetSurfacePoint(fWidthRatio2, fDepthRatio, fLengthRatio2);
                        Vector3 vecPt3 = vecGetSurfacePoint(fWidthRatio1, fDepthRatio, fLengthRatio2);

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
            protected float fGetDepthRatioFromStep(int iDepthStep)
            {
                //from -1 to +1
                float fDepthRatio   = (1f) / (m_nDepthSteps - 1) * (iDepthStep);
                fDepthRatio         = 2f * fDepthRatio - 1f;
                return fDepthRatio;
            }

            protected float fGetWidthRatioFromStep(int iWidthStep)
            {
                //from -1 to +1
                float fWidthRatio   = (1f) / (m_nWidthSteps - 1) * (iWidthStep);
                fWidthRatio         = 2f * fWidthRatio - 1f;
                return fWidthRatio;
            }

            protected float fGetLengthRatioFromStep(int iLengthStep)
            {
                //from 0 to +1
                float fLengthRatio = (1f) / (m_nLengthSteps - 1) * (iLengthStep);
                return fLengthRatio;
            }

            /// <summary>
            /// Returns a point on the shape surface if one of the ratios is at the limit.
            /// Returns a point inside the shape if all ratios are within the limits.
            /// Width ratio goes from -1 to 1.
            /// Depth ratio goes from -1 to 1.
            /// Length ratio goes from 0 to 1.
            /// </summary>
            public virtual Vector3 vecGetSurfacePoint(float fWidthRatio, float fDepthRatio, float fLengthRatio)
            {
                Vector3 vecSpinePos = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX   = m_aFrames.vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalY   = m_aFrames.vecGetLocalYAlongLength(fLengthRatio);

                float fX            = 0.5f * fWidthRatio * fGetWidth(fLengthRatio);
                float fY            = 0.5f * fDepthRatio * fGetDepth(fLengthRatio);
                Vector3 vecPt       = vecSpinePos + fX * vecLocalX + fY * vecLocalY;

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }

            protected float fGetDepth(float fLengthRatio)
            {
                float fDepth = m_oDepthModulation.fGetModulation(fLengthRatio);
                return fDepth;
            }

            protected float fGetWidth(float fLengthRatio)
            {
                float fWidth = m_oWidthModulation.fGetModulation(fLengthRatio);
                return fWidth;
            }
        }
    }
}