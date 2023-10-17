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
        public class ControlPointSpline : ISpline
        {
            public enum EEnds       { OPEN, CLOSED };
            protected EEnds         m_eEnds;
            protected List<float>   m_aKnot;
            protected List<Vector3> m_aControlPoints;
            protected uint          m_nDegree;
            protected float         m_fError = 0.0000001f;


            /// <summary>
            /// BSpline-based control point spline.
            /// Can have open ends or closed.
            /// Open ends will match the first and last control point precisely.
            /// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
            /// https://math.stackexchange.com/questions/1296954/b-spline-how-to-generate-a-closed-curve-using-uniform-b-spline-curve
            /// </summary>
            public ControlPointSpline(List<Vector3> aControlPoints, uint nDegree = 2, EEnds eEnds = EEnds.OPEN)
            {
                m_aControlPoints    = aControlPoints;
                m_eEnds             = eEnds;

                if (m_eEnds == EEnds.CLOSED)
                {
                    //check that first and last points are not the same!
                    if ((m_aControlPoints[0] - m_aControlPoints[^1]).Length() < m_fError)
                    {
                        m_aControlPoints.RemoveAt(m_aControlPoints.Count - 1);
                    }
                }

                m_nDegree           = nDegree;
                m_aKnot             = aGetKnotVector();
            }

            public List<float> aGetKnotVector()
            {
                if (m_eEnds == EEnds.OPEN)
                {
                    //calculate equal spacing
                    uint nNumberOfControlPoints = (uint)m_aControlPoints.Count;
                    uint nNumberOfKnots         = nNumberOfControlPoints + m_nDegree + 1;
                    List<float> aVector         = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange            = (nNumberOfKnots - m_nDegree - (m_nDegree + 1));
                    float dR                    = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue = -(dR * m_nDegree) + dR * i;
                        fValue       = Uf.fLimitValue(fValue, 0f, 1f);
                        aVector[i]   = fValue;
                    }
                    return aVector;
                }
                else
                {
                    //add n-1 control points
                    uint nNumberOfControlPointsBefore = (uint)m_aControlPoints.Count;
                    for (int i = 0; i < nNumberOfControlPointsBefore - 1; i++)
                    {
                        m_aControlPoints.Add(m_aControlPoints[i]);
                    }

                    //calculate equal spacing
                    uint nNumberOfControlPoints = (uint)m_aControlPoints.Count;
                    uint nNumberOfKnots         = nNumberOfControlPoints + m_nDegree + 1;
                    List<float> aVector         = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange            = (nNumberOfKnots - m_nDegree - (m_nDegree + 1));
                    float dR                    = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue    = -(dR * m_nDegree) + dR * i;
                        aVector[i]      = fValue;
                    }
                    return aVector;
                }
            }

            //render spline with given samples
            public List<Vector3> aGetPoints(uint nSamples)
            {
                List<Vector3> aPoints = new List<Vector3>();
                for (uint i = 0; i < nSamples; i++)
                {
                    float fLengthRatio  = (float)(i) / (float)(nSamples - 1);
                    Vector3 vecPt       = vecGetPointAt(fLengthRatio);
                    aPoints.Add(vecPt);
                }
                return aPoints;
            }

            //sample dynamically
            public Vector3 vecGetPointAt(float fLengthRatio)
            {
                Vector3 vecPt = new Vector3();
                for (int i = 0; i < m_aControlPoints.Count; ++i)
                {
                    float fBaseValue = fBaseFunc(fLengthRatio, i, (int)m_nDegree);
                    vecPt += fBaseValue * m_aControlPoints[(int)i];
                }
                return vecPt;
            }

            public float fBaseFunc(float fLengthRatio, int nControlPoint, int nDegree)
            {
                float fValue = 0;

                if (nDegree == 0)
                {
                    //last iteration loop
                    if (
                        (fLengthRatio >= m_aKnot[nControlPoint] && fLengthRatio < m_aKnot[nControlPoint + 1]) ||
                        ((MathF.Abs(fLengthRatio - m_aKnot[nControlPoint + 1]) < m_fError) && (MathF.Abs(fLengthRatio - m_aKnot[^1]) < m_fError))
                        )
                    {
                        fValue = 1f;
                    }
                }
                else
                {
                    //decrease degree with each iteration
                    if (MathF.Abs(m_aKnot[nControlPoint + nDegree] - m_aKnot[nControlPoint]) > m_fError)
                    {
                        fValue += (fLengthRatio - m_aKnot[nControlPoint]) / (m_aKnot[nControlPoint + nDegree] - m_aKnot[nControlPoint]) * fBaseFunc(fLengthRatio, nControlPoint, nDegree - 1);
                    }
                    if (MathF.Abs(m_aKnot[nControlPoint + nDegree + 1] - m_aKnot[nControlPoint + 1]) > m_fError)
                    {
                        fValue += (m_aKnot[nControlPoint + nDegree + 1] - fLengthRatio) / (m_aKnot[nControlPoint + nDegree + 1] - m_aKnot[nControlPoint + 1]) * fBaseFunc(fLengthRatio, nControlPoint + 1, nDegree - 1);
                    }
                }

                return fValue;
            }
        }
    }
}