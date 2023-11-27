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


namespace Leap71
{
    namespace ShapeKernel
    {
        public class Frames : ISpline
        {
            public enum EFrameType { CYLINDRICAL, SPHERICAL, Z, MIN_ROTATION };

            protected Vector3       m_vecTargetX;
            protected List<Vector3> m_aPoints;
            protected List<Vector3> m_aLocalX;
            protected List<Vector3> m_aLocalY;
            protected List<Vector3> m_aLocalZ;
            protected Vector3?      m_vecLastLocalX;


            /// <summary>
            /// Extrudes a const local frame along a straight line.
            /// </summary>
            public Frames(
                float       fLength,
                LocalFrame  oConstLocalFrame,
                float       fReparametrisationSpacing = 1f)
            {
                Vector3 vecStart    = oConstLocalFrame.vecGetPosition();
                Vector3 vecDir      = oConstLocalFrame.vecGetLocalZ();
                Vector3 vecEnd      = vecStart + fLength * vecDir;

                List<Vector3> aPoints = new List<Vector3>() { vecStart, vecEnd };
                m_aPoints = SplineOperations.aGetReparametrizedSpline(aPoints, fReparametrisationSpacing);

                m_aLocalX = new List<Vector3>();
                m_aLocalY = new List<Vector3>();
                m_aLocalZ = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i++)
                {
                    m_aLocalZ.Add(oConstLocalFrame.vecGetLocalZ());
                    m_aLocalX.Add(oConstLocalFrame.vecGetLocalX());
                    m_aLocalY.Add(oConstLocalFrame.vecGetLocalY());
                }
            }

            /// <summary>
            /// Extrudes a const local frame along a spline.
            /// </summary>
            public Frames(
                List<Vector3>   aPoints,
                LocalFrame      oConstLocalFrame,
                float           fReparametrisationSpacing = 1f)
            {
                m_aPoints = SplineOperations.aGetReparametrizedSpline(aPoints, fReparametrisationSpacing);

                m_aLocalX = new List<Vector3>();
                m_aLocalY = new List<Vector3>();
                m_aLocalZ = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i++)
                {
                    m_aLocalZ.Add(oConstLocalFrame.vecGetLocalZ());
                    m_aLocalX.Add(oConstLocalFrame.vecGetLocalX());
                    m_aLocalY.Add(oConstLocalFrame.vecGetLocalY());
                }
            }

            /// <summary>
            /// Aligns local frames along a spline for localZ and a const target direction for localX.
            /// </summary>
            public Frames(
                List<Vector3>   aPoints,
                Vector3         vecTargetX,
                float           fReparametrisationSpacing = 1f)
            {
                vecTargetX = vecTargetX.Normalize();

                m_aPoints = SplineOperations.aGetReparametrizedSpline(aPoints, fReparametrisationSpacing);
                m_aLocalZ = aGetTangentDirections();

                m_aLocalX = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i += 1)
                {
                    Vector3 vecPt = m_aPoints[i];
                    Vector3 vecLocalZ = m_aLocalZ[i];
                    Vector3 vecLocalX = vecAlignWithTargetX(vecLocalZ, vecTargetX);
                    m_aLocalX.Add(vecLocalX);
                }

                //fill in localY using convention
                m_aLocalY = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i++)
                {
                    Vector3 vecLocalY = LocalFrame.vecGetLocalY(m_aLocalZ[i], m_aLocalX[i]);
                    m_aLocalY.Add(vecLocalY);
                }

                //post processing
                uint nSamples = (uint)m_aPoints.Count;
                m_aPoints = SplineOperations.aGetNURBSpline(m_aPoints, nSamples);
                m_aLocalX = SplineOperations.aGetNURBSpline(m_aLocalX, nSamples);
                m_aLocalY = SplineOperations.aGetNURBSpline(m_aLocalY, nSamples);
                m_aLocalZ = SplineOperations.aGetNURBSpline(m_aLocalZ, nSamples);
            }

            /// <summary>
            /// Aligns local frames along a spline for localZ and a coord-system-dependant target direction for localX.
            /// </summary>
            public Frames(
                List<Vector3>   aPoints,
                EFrameType      eFrameType,
                float           fReparametrisationSpacing = 1f)
            {
                m_aPoints = SplineOperations.aGetReparametrizedSpline(aPoints, fReparametrisationSpacing);
                m_aLocalZ = aGetTangentDirections();

                m_aLocalX = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i += 1)
                {
                    Vector3 vecPt       = m_aPoints[i];
                    Vector3 vecLocalZ   = m_aLocalZ[i];
                    Vector3 vecLocalX   = vecAlignWithFramesType(vecPt, vecLocalZ, eFrameType);
                    m_aLocalX.Add(vecLocalX);
                }

                //fill in localY using convention
                m_aLocalY = new List<Vector3>();
                for (int i = 0; i < m_aPoints.Count; i++)
                {
                    Vector3 vecLocalY   = LocalFrame.vecGetLocalY(m_aLocalZ[i], m_aLocalX[i]);
                    m_aLocalY.Add(vecLocalY);
                }
            }


            //utility
            protected Vector3 vecAlignWithFramesType(Vector3 vecPt, Vector3 vecLocalZ, EFrameType eFrameType)
            {
                if (eFrameType == EFrameType.MIN_ROTATION)
                {
                    /////////////////////////////////
                    //minimize rotation
                    if (m_vecLastLocalX == null)
                    {
                        Vector3 vecTargetX      = vecGetTargetX(vecPt, EFrameType.Z);
                        Vector3 vecDummyLocalX  = vecAlignWithTargetX(vecLocalZ, vecTargetX);
                        m_vecLastLocalX         = vecDummyLocalX;
                    }
                    Vector3 vecLocalX   = vecAlignWithTargetX(vecLocalZ, (Vector3)m_vecLastLocalX);
                    m_vecLastLocalX     = vecLocalX;
                    return vecLocalX;
                    /////////////////////////////////
                }
                else
                {
                    Vector3 vecTargetX  = vecGetTargetX(vecPt, eFrameType);
                    Vector3 vecLocalX   = vecAlignWithTargetX(vecLocalZ, vecTargetX);
                    return vecLocalX;
                }
            }

            public static Vector3 vecAlignWithTargetX(Vector3 vecLocalZ, Vector3 vecTargetX)
            {
                Vector3 vecInitLocalX   = VecOperations.vecGetOrthogonalDir(vecLocalZ);
                Vector3 vecInitLocalY   = Vector3.Cross(vecInitLocalX, vecLocalZ);
                float fMaxDotProduct    = MathF.Abs(Vector3.Dot(vecInitLocalX, vecTargetX));
                Vector3 vecFinalLocalX  = vecInitLocalX;

                for (int j = 0; j < 180; j++)
                {
                    float fPhi              = (2 * MathF.PI) / 360 * j;
                    Vector3 vecNewLocalX    = MathF.Cos(fPhi) * vecInitLocalX + MathF.Sin(fPhi) * vecInitLocalY;
                    float fDotProduct       = MathF.Abs(Vector3.Dot(vecNewLocalX, vecTargetX));
                    if (fDotProduct > fMaxDotProduct)
                    {
                        vecFinalLocalX      = vecNewLocalX;
                        fMaxDotProduct      = fDotProduct;
                    }
                }
                vecFinalLocalX          = VecOperations.vecFlipForAlignment(vecFinalLocalX, vecTargetX);
                vecFinalLocalX          = vecFinalLocalX.Normalize();
                return vecFinalLocalX;
            }

            public static Vector3 vecGetTargetX(Vector3 vecPt, EFrameType eFrameType)
            {
                if (eFrameType == EFrameType.CYLINDRICAL)
                {
                    Vector3 vecRadial   = new Vector3(vecPt.X, vecPt.Y, 0);
                    vecRadial           = vecRadial.Normalize();
                    return vecRadial;
                }
                else if (eFrameType == EFrameType.SPHERICAL)
                {
                    Vector3 vecRadial   = new Vector3(vecPt.X, vecPt.Y, vecPt.Z);
                    vecRadial           = vecRadial.Normalize();
                    return vecRadial;
                }
                else
                {
                    return Vector3.UnitZ;
                }
            }

            protected List<Vector3> aGetTangentDirections()
            {
                List<Vector3> aTangents = new List<Vector3>();
                for (int i = 1; i < m_aPoints.Count - 1; i++)
                {
                    Vector3 vecLocalZ   = m_aPoints[i] - m_aPoints[i - 1];
                    vecLocalZ           = vecLocalZ.Normalize();
                    aTangents.Add(vecLocalZ);
                }

                //add continuous start and end
                aTangents.Insert(0, aTangents[0]);
                aTangents.Add(aTangents[^1]);
                return aTangents;
            }


            //access
            public Vector3 vecGetSpineAlongLength(float fLengthRatio)
            {
                fLengthRatio    = Uf.fLimitValue(fLengthRatio, 0f, 1f);
                float fStep     = fLengthRatio * (m_aPoints.Count - 1);

                int iLowerStep  = (int)Math.Min(fStep, m_aPoints.Count - 1);
                int iUpperStep  = (int)Math.Min(fStep + 1, m_aPoints.Count - 1);
                float dS        = fStep - iLowerStep;

                Vector3 vecLowerVector = m_aPoints[iLowerStep];
                Vector3 vecUpperVector = m_aPoints[iUpperStep];

                Vector3 vecVector = vecLowerVector + dS * (vecUpperVector - vecLowerVector);
                return vecVector;
            }

            public Vector3 vecGetLocalXAlongLength(float fLengthRatio)
            {
                fLengthRatio    = Uf.fLimitValue(fLengthRatio, 0f, 1f);
                float fStep     = fLengthRatio * (m_aPoints.Count - 1);

                int iLowerStep  = (int)Math.Min(fStep, m_aPoints.Count - 1);
                int iUpperStep  = (int)Math.Min(fStep + 1, m_aPoints.Count - 1);
                float dS        = fStep - iLowerStep;

                Vector3 vecLowerVector = m_aLocalX[iLowerStep];
                Vector3 vecUpperVector = m_aLocalX[iUpperStep];

                Vector3 vecVector = vecLowerVector + dS * (vecUpperVector - vecLowerVector);
                return vecVector;
            }

            public Vector3 vecGetLocalYAlongLength(float fLengthRatio)
            {
                fLengthRatio    = Uf.fLimitValue(fLengthRatio, 0f, 1f);
                float fStep     = fLengthRatio * (m_aPoints.Count - 1);

                int iLowerStep  = (int)Math.Min(fStep, m_aPoints.Count - 1);
                int iUpperStep  = (int)Math.Min(fStep + 1, m_aPoints.Count - 1);
                float dS        = fStep - iLowerStep;

                Vector3 vecLowerVector = m_aLocalY[iLowerStep];
                Vector3 vecUpperVector = m_aLocalY[iUpperStep];

                Vector3 vecVector = vecLowerVector + dS * (vecUpperVector - vecLowerVector);
                return vecVector;
            }

            public Vector3 vecGetLocalZAlongLength(float fLengthRatio)
            {
                fLengthRatio    = Uf.fLimitValue(fLengthRatio, 0f, 1f);
                float fStep     = fLengthRatio * (m_aPoints.Count - 1);

                int iLowerStep  = (int)Math.Min(fStep, m_aPoints.Count - 1);
                int iUpperStep  = (int)Math.Min(fStep + 1, m_aPoints.Count - 1);
                float dS        = fStep - iLowerStep;

                Vector3 vecLowerVector = m_aLocalZ[iLowerStep];
                Vector3 vecUpperVector = m_aLocalZ[iUpperStep];

                Vector3 vecVector = vecLowerVector + dS * (vecUpperVector - vecLowerVector);
                return vecVector;
            }

            public LocalFrame oGetLocalFrame(float fLengthRatio)
            {
                Vector3 vecPos      = vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX   = vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalZ   = vecGetLocalZAlongLength(fLengthRatio);
                LocalFrame oFrame   = new LocalFrame(vecPos, vecLocalZ, vecLocalX);
                return oFrame;
            }

            public List<Vector3> aGetPoints()
            {
                return m_aPoints;
            }

            public List<Vector3> aGetPoints(uint nSamples)
            {
                return SplineOperations.aGetReparametrizedSpline(m_aPoints, nSamples);
            }
        }
    }
}