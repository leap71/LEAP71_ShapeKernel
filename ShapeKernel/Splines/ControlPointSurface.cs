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
        public class ControlPointSurface
        {
            public enum EEnds       { OPEN, CLOSED };
            protected EEnds         m_eEndsU;
            protected EEnds         m_eEndsV;
            protected List<float>   m_aKnotU;
            protected List<float>   m_aKnotV;
            protected uint          m_nDegreeU;
            protected uint          m_nDegreeV;
            protected List<List<Vector3>>
                                    m_aControlGrid;
            protected float         m_fError = 0.0000001f;


            /// <summary>
            /// BSpline-based control grid surface (2D).
            /// Can have open ends or closed for either of the two dimensions.
            /// Open ends will match the first and last control grid precisely.
            /// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
            /// https://math.stackexchange.com/questions/1296954/b-spline-how-to-generate-a-closed-curve-using-uniform-b-spline-curve
            /// </summary>
            public ControlPointSurface(
                List<List<Vector3>> aControlGrid,
                uint                nDegreeU    = 2,
                uint                nDegreeV    = 2,
                EEnds               eEndsU      = EEnds.OPEN,
                EEnds               eEndsV      = EEnds.OPEN)
            {
                m_aControlGrid  = aControlGrid;
                m_nDegreeU      = nDegreeU;
                m_nDegreeV      = nDegreeV;
                m_eEndsU        = eEndsU;
                m_eEndsV        = eEndsV;

                if (m_eEndsU == EEnds.CLOSED)
                {
                    //check that first and last points are not the same!
                    if ((m_aControlGrid[0][0] - m_aControlGrid[^1][0]).Length() < m_fError)
                    {
                        m_aControlGrid = GridOperations.aRemoveListInX(m_aControlGrid, (uint)m_aControlGrid.Count - 1);
                    }
                }

                if (m_eEndsV == EEnds.CLOSED)
                {
                    //check that first and last points are not the same!
                    if ((m_aControlGrid[0][0] - m_aControlGrid[0][^1]).Length() < m_fError)
                    {
                        m_aControlGrid = GridOperations.aRemoveListInY(m_aControlGrid, (uint)m_aControlGrid.Count - 1);
                    }
                }

                m_aKnotU = aGetUKnotVector((uint)m_aControlGrid.Count, m_nDegreeU, m_eEndsU);
                m_aKnotV = aGetVKnotVector((uint)m_aControlGrid[0].Count, m_nDegreeV, m_eEndsV);
            }

            public List<float> aGetUKnotVector(uint nNumberOfControlPoints, uint nDegree, EEnds eEnds)
            {
                if (eEnds == EEnds.OPEN)
                {
                    //calculate equal spacing
                    uint nNumberOfKnots = nNumberOfControlPoints + nDegree + 1;
                    List<float> aVector = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange    = (nNumberOfKnots - nDegree - (nDegree + 1));
                    float dR            = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue    = -(dR * nDegree) + dR * i;
                        fValue          = Uf.fLimitValue(fValue, 0f, 1f);
                        aVector[i]      = fValue;
                    }
                    return aVector;
                }
                else
                {
                    //add n-1 control points
                    uint nNumberOfControlPointsBefore = (uint)m_aControlGrid.Count;
                    for (int i = 0; i < nNumberOfControlPointsBefore - 1; i++)
                    {
                        List<Vector3> aListToAdd = GridOperations.aGetListInX(m_aControlGrid, (uint)i);
                        m_aControlGrid = GridOperations.aAddListInX(m_aControlGrid, aListToAdd);
                    }

                    //calculate equal spacing
                    nNumberOfControlPoints  = (uint)m_aControlGrid.Count;
                    uint nNumberOfKnots     = nNumberOfControlPoints + nDegree + 1;
                    List<float> aVector     = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange        = (nNumberOfKnots - nDegree - (nDegree + 1));
                    float dR                = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue    = -(dR * nDegree) + dR * i;
                        aVector[i]      = fValue;
                    }
                    return aVector;
                }
            }

            public List<float> aGetVKnotVector(uint nNumberOfControlPoints, uint nDegree, EEnds eEnds)
            {
                if (eEnds == EEnds.OPEN)
                {
                    //calculate equal spacing
                    uint nNumberOfKnots = nNumberOfControlPoints + nDegree + 1;
                    List<float> aVector = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange    = (nNumberOfKnots - nDegree - (nDegree + 1));
                    float dR            = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue    = -(dR * nDegree) + dR * i;
                        fValue          = Uf.fLimitValue(fValue, 0f, 1f);
                        aVector[i]      = fValue;
                    }
                    return aVector;
                }
                else
                {
                    //add n-1 control points
                    uint nNumberOfControlPointsBefore = (uint)m_aControlGrid[0].Count;
                    for (int i = 0; i < nNumberOfControlPointsBefore - 1; i++)
                    {
                        List<Vector3> aListToAdd = GridOperations.aGetListInY(m_aControlGrid, (uint)i);
                        m_aControlGrid = GridOperations.aAddListInY(m_aControlGrid, aListToAdd);
                    }

                    //calculate equal spacing
                    nNumberOfControlPoints  = (uint)m_aControlGrid[0].Count;
                    uint nNumberOfKnots     = nNumberOfControlPoints + nDegree + 1;
                    List<float> aVector     = new List<float>(new float[nNumberOfKnots]);
                    uint nValidRange        = (nNumberOfKnots - nDegree - (nDegree + 1));
                    float dR                = 1f / nValidRange;

                    for (int i = 0; i < nNumberOfKnots; i++)
                    {
                        float fValue    = -(dR * nDegree) + dR * i;
                        aVector[i]      = fValue;
                    }
                    return aVector;
                }
            }

            //render spline with given samples
            public List<List<Vector3>> aGetGrid(uint nUSamples = 500, uint nVSamples = 500)
            {
                List<List<Vector3>> aGrid = new List<List<Vector3>>();
                for (uint i = 0; i < nUSamples; i++)
                {
                    float fURatio           = (float)(i) / (float)(nUSamples - 1);
                    List<Vector3> aPoints   = new List<Vector3>();
                    for (uint j = 0; j < nVSamples; j++)
                    {
                        float fVRatio = (float)(j) / (float)(nVSamples - 1);
                        Vector3 vecPt = vecGetPointAt(fURatio, fVRatio);
                        aPoints.Add(vecPt);
                    }
                    aGrid.Add(aPoints);
                }
                return aGrid;
            }

            //sample dynamically
            public Vector3 vecGetPointAt(float fURatio, float fVRatio)
            {
                Vector3 vecPt = new Vector3();
                for (int u = 0; u < m_aControlGrid.Count; u++)
                {
                    float fUBaseValue = fBaseFunc(m_aKnotU, fURatio, u, (int)m_nDegreeU);
                    for (int v = 0; v < m_aControlGrid[u].Count; v++)
                    {
                        float fVBaseValue = fBaseFunc(m_aKnotV, fVRatio, v, (int)m_nDegreeV);
                        vecPt += fUBaseValue * fVBaseValue * m_aControlGrid[(int)u][(int)v];
                    }
                }
                return vecPt;
            }

            public float fBaseFunc(List<float> aKnot, float fLengthRatio, int nControlPoint, int nDegree)
            {
                float fValue = 0;

                if (nDegree == 0)
                {
                    //last iteration loop
                    if (
                        (fLengthRatio >= aKnot[nControlPoint] && fLengthRatio < aKnot[nControlPoint + 1]) ||
                        ((MathF.Abs(fLengthRatio - aKnot[nControlPoint + 1]) < m_fError) && (MathF.Abs(fLengthRatio - aKnot[^1]) < m_fError))
                        )
                    {
                        fValue = 1f;
                    }
                }
                else
                {
                    //decrease degree with each iteration
                    if (MathF.Abs(aKnot[nControlPoint + nDegree] - aKnot[nControlPoint]) > m_fError)
                    {
                        fValue += (fLengthRatio - aKnot[nControlPoint]) / (aKnot[nControlPoint + nDegree] - aKnot[nControlPoint]) * fBaseFunc(aKnot, fLengthRatio, nControlPoint, nDegree - 1);
                    }
                    if (MathF.Abs(aKnot[nControlPoint + nDegree + 1] - aKnot[nControlPoint + 1]) > m_fError)
                    {
                        fValue += (aKnot[nControlPoint + nDegree + 1] - fLengthRatio) / (aKnot[nControlPoint + nDegree + 1] - aKnot[nControlPoint + 1]) * fBaseFunc(aKnot, fLengthRatio, nControlPoint + 1, nDegree - 1);
                    }
                }

                return fValue;
            }

            public List<List<Vector3>> aGetControlGrid()
            {
                return m_aControlGrid;
            }

            public Vector3 vecGetControlPoint(uint u, uint v)
            {
                return m_aControlGrid[(int)u][(int)v];
            }

            public void UpdateControlPoint(Vector3 vecPt, uint u, uint v)
            {
                m_aControlGrid[(int)u][(int)v] = vecPt;
            }
        }
    }
}