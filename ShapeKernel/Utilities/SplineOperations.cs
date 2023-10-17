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
        public class SplineOperations
        {
            /// <summary>
            /// Returns a list of linearly interpolated vectors between specified start and end.
            /// </summary>
            public static List<Vector3> aGetLinearInterpolation(Vector3 vecStart, Vector3 vecEnd, uint nSamples)
            {
                List<Vector3> aPoints = new List<Vector3>();
                for (int i = 0; i < nSamples; i++)
                {
                    float fRatio = 1f / (float)(nSamples - 1) * i;
                    Vector3 vecPt = vecStart + fRatio * (vecEnd - vecStart);
                    aPoints.Add(vecPt);
                }
                return aPoints;
            }

            /// <summary>
            /// Resamples the spline such that all points within have a constant spacing and the number of points match the specified target.
            /// Start and end points remain constant.
            /// </summary>
            public static List<Vector3> aGetReparametrizedSpline(List<Vector3> aPoints, uint nTargetSamples)
            {
                List<float> aLengthsAtIndices   = aGetLengthsAtIndices(aPoints);
                float fSpineLength              = aLengthsAtIndices[aLengthsAtIndices.Count - 1];
                float fTargetStep               = fSpineLength / nTargetSamples;
                List<Vector3> aNewPoints        = new List<Vector3>();

                aNewPoints.Add(aPoints[0]);
                for (int j = 1; j < nTargetSamples; j++)
                {
                    float fTargetLength = fTargetStep * j;
                    int iUpperIndex     = 0;
                    float fUpperLength  = 0;

                    for (iUpperIndex = 1; iUpperIndex < aPoints.Count; iUpperIndex++)
                    {
                        fUpperLength    = aLengthsAtIndices[iUpperIndex];
                        if (fUpperLength >= fTargetLength)
                        {
                            break;
                        }
                    }
                    int iLowerIndex     = iUpperIndex - 1;
                    float fLowerLength  = aLengthsAtIndices[iLowerIndex];
                    float dS            = (fTargetLength - fLowerLength) / (fUpperLength - fLowerLength);

                    Vector3 vecLowerPt  = aPoints[iLowerIndex];
                    Vector3 vecUpperPt  = aPoints[iUpperIndex];

                    //linear interpolation
                    Vector3 vecInterPol = vecLowerPt + dS * (vecUpperPt - vecLowerPt);
                    aNewPoints.Add(vecInterPol);
                }
                aNewPoints.Add(aPoints[aPoints.Count - 1]);
                return aNewPoints;
            }

            /// <summary>
            /// Resamples the spline such that all points within have a constant spacing.
            /// Start and end points remain constant.
            /// </summary>
            public static List<Vector3> aGetReparametrizedSpline(List<Vector3> aPoints, float fTargetSpacing)
            {
                float fTotalLength          = fGetTotalLength(aPoints);
                uint nTargetSamples         = (uint)(MathF.Max(10, fTotalLength / fTargetSpacing));
                List<Vector3> aNewPoints    = aGetReparametrizedSpline(aPoints, nTargetSamples);
                return aNewPoints;
            }

            /// <summary>
            /// Returns a list of values, each value representing the distance of the point at that index along the spline.
            /// </summary>
            public static List<float> aGetLengthsAtIndices(List<Vector3> aPoints)
            {
                List<float> aLengthsAtIndices = new List<float>();

                float fTotalSpineLength = 0;
                aLengthsAtIndices.Add(0);

                for (int i = 1; i < aPoints.Count; i++)
                {
                    Vector3 vecPt1      = aPoints[i - 1];
                    Vector3 vecPt2      = aPoints[i];
                    fTotalSpineLength   += (vecPt2 - vecPt1).Length();
                    aLengthsAtIndices.Add(fTotalSpineLength);
                }
                return aLengthsAtIndices;
            }

            /// <summary>
            /// Calculates the average spacing between points of a spline.
            /// </summary>
            public static float fGetAveragePointSpacing(List<Vector3> aPoints)
            {
                float fTotalLength  = fGetTotalLength(aPoints);
                float fAvgSpacing   = fTotalLength / (aPoints.Count - 1);
                return fAvgSpacing;
            }

            /// <summary>
            /// Calculates the length of a spline.
            /// </summary>
            public static float fGetTotalLength(List<Vector3> aPoints)
            {
                float fTotalSpineLength = 0;

                for (int i = 1; i < aPoints.Count; i++)
                {
                    Vector3 vecPt1      = aPoints[i - 1];
                    Vector3 vecPt2      = aPoints[i];
                    fTotalSpineLength   += (vecPt2 - vecPt1).Length();
                }
                return fTotalSpineLength;
            }

            /// <summary>
            /// Splits the specified list at the index, returns two separate listes witout overlap.
            /// </summary>
            public static (List<Vector3>, List<Vector3>) aSplitLists(List<Vector3> aList, int iFirstIndexOfSecondList)
            {
                List<Vector3> aFirstList    = new List<Vector3>();
                List<Vector3> aSecondList   = new List<Vector3>();
                for (int i = 0; i < aList.Count; i++)
                {
                    if (i < iFirstIndexOfSecondList)
                    {
                        aFirstList.Add(aList[i]);
                    }
                    else
                    {
                        aSecondList.Add(aList[i]);
                    }
                }
                return (aFirstList, aSecondList);
            }

            /// <summary>
            /// Combines multiple lists of points into one list.
            /// </summary>
            public static List<Vector3> aCombineLists(List<List<Vector3>> aLists)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                foreach (List<Vector3> aList in aLists)
                {
                    foreach (Vector3 vecPt in aList)
                    {
                        aFinalList.Add(vecPt);
                    }
                }
                return aFinalList;
            }

            /// <summary>
            /// Rotates the spline around the absolute z-axis by a given angle.
            /// The angle is measured in radiant.
            /// </summary>
            public static List<Vector3> aRotateListAroundZ(List<Vector3> aList, float fAngle)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    aFinalList.Add(VecOperations.vecRotateAroundZ(vecPt, fAngle));
                }
                return aFinalList;
            }

            /// <summary>
            /// Translates the spline by a given vector.
            /// </summary>
            public static List<Vector3> aTranslateList(List<Vector3> aList, Vector3 vecShift)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    aFinalList.Add(vecPt + vecShift);
                }
                return aFinalList;
            }

            /// <summary>
            /// Scales the spline by a given factor with respect to the absolute origin.
            /// </summary>
            public static List<Vector3> aScaleList(List<Vector3> aList, float fScale)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    aFinalList.Add(vecPt * fScale);
                }
                return aFinalList;
            }

            /// <summary>
            /// Applies nurb-subdivision to smoothen the points witin the list.
            /// Note that the count of the list will change!
            /// Start and end value remain constant.
            /// https://en.wikipedia.org/wiki/Non-uniform_rational_B-spline
            /// </summary>
            public static List<Vector3> aGetNURBSpline(List<Vector3> aControlPoints, uint nSamples)
            {
                ControlPointSpline oBSpline = new ControlPointSpline(aControlPoints, 2, ControlPointSpline.EEnds.OPEN);
                return oBSpline.aGetPoints(nSamples);
            }
            
            /// <summary>
            /// Samples the specified list with an increment.
            /// Note that the count of the list will change!
            /// Start and end value remain constant.
            /// </summary>
            public static List<Vector3> aOverSampleList(List<Vector3> aList, int iSamplesPerStep)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                for (int i = 1; i < aList.Count; i++)
                {
                    for (int j = 0; j < iSamplesPerStep; j++)
                    {
                        Vector3 vecPt = aList[i - 1] + (float)j / (float)(iSamplesPerStep) * (aList[i] - aList[i - 1]);
                        aFinalList.Add(vecPt);
                    }
                }
                aFinalList.Add(aList[^1]);
                return aFinalList;
            }

            /// <summary>
            /// Samples the specified list with an increment.
            /// Note that the count of the list will change!
            /// Start and end value remain constant.
            /// </summary>
            public static List<Vector3> aSubSampleList(List<Vector3> aList, int iSampleSize)
            {
                List<Vector3> aFinalList = new List<Vector3>();
                int i = 0;
                for (i = 0; i < aList.Count; i += iSampleSize)
                {
                    aFinalList.Add(aList[i]);
                }
                aFinalList.Add(aList[^1]);
                return aFinalList;
            }

            /// <summary>
            /// Rotates and translates the spline onto a new local frame of reference.
            /// </summary>
            public static List<Vector3> aTranslateListOntoFrame(LocalFrame oFrame, List<Vector3> aList)
            {
                List<Vector3> aNewList  = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    Vector3 vecNewPt    = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt);
                    aNewList.Add(vecNewPt);
                }
                return aNewList;
            }

            /// <summary>
            /// Expresses the spline relative to the specified local frame of reference.
            /// </summary>
            public static List<Vector3> aExpressListInFrame(LocalFrame oFrame, List<Vector3> aList)
            {
                List<Vector3> aNewList  = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    Vector3 vecNewPt    = VecOperations.vecExpressPointInFrame(oFrame, vecPt);
                    aNewList.Add(vecNewPt);
                }
                return aNewList;
            }

            /// <summary>
            /// Rotates the spline around an arbiary 3D axis by a given angle.
            /// The angle is measured in radiant.
            /// </summary>
            public static List<Vector3> aRotateListAroundAxis(List<Vector3> aList, float dPhi, Vector3 vecAxis, Vector3 vecAxisOrigin = new Vector3())
            {
                List<Vector3> aNewList  = new List<Vector3>();
                foreach (Vector3 vecPt in aList)
                {
                    Vector3 vecNewPt    = VecOperations.vecRotateAroundAxis(vecPt, dPhi, vecAxis, vecAxisOrigin);
                    aNewList.Add(vecNewPt);
                }
                return aNewList;
            }

            /// <summary>
            /// Returns the point from the list that is closest to the given start position.
            /// </summary>
            public static Vector3 vecGetClosestPoint(List<Vector3> aPoints, Vector3 vecStart)
            {
                float fMinDist      = float.MaxValue;
                Vector3 vecNear     = Vector3.Zero;

                for (int i = 0; i < aPoints.Count; i++)
                {
                    Vector3 vecPt   = aPoints[i];
                    float fDist     = (vecPt - vecStart).LengthSquared();
                    if (fDist < fMinDist)
                    {
                        fMinDist    = fDist;
                        vecNear     = vecPt;
                    }
                }
                return vecNear;
            }

            /// <summary>
            /// Returns the distance to the point from the list that is closest to the given start position.
            /// </summary>
            public static float fGetDistanceToClosestPoint(List<Vector3> aPoints, Vector3 vecStart)
            {
                float fMinDist      = float.MaxValue;
                for (int i = 0; i < aPoints.Count; i++)
                {
                    Vector3 vecPt   = aPoints[i];
                    float fDist     = (vecPt - vecStart).LengthSquared();
                    if (fDist < fMinDist)
                    {
                        fMinDist    = fDist;
                    }
                }
                return MathF.Sqrt(fMinDist);
            }

            /// <summary>
            /// Returns a list of points that make up centres if the input points were clustered by the given range.
            /// All input points that are within the clustering range to an already existing point will be skipped.
            /// </summary>
            public static List<Vector3> aGetClusteredPoints(List<Vector3> aPoints, float fClusteringRange)
            {
                float fDist                     = fClusteringRange * fClusteringRange;
                List<Vector3> aClusteredPoints  = new List<Vector3>() { aPoints[0] };

                for (int i = 1; i < aPoints.Count; i++)
                {
                    //check it point is in range of existing cluster
                    Vector3 vecPt   = aPoints[i];
                    Vector3 vecNear = vecGetClosestPoint(aClusteredPoints, vecPt);
                    if ((vecNear - vecPt).LengthSquared() > fDist)
                    {
                        aClusteredPoints.Add(vecPt);
                    }
                }
                return aClusteredPoints;
            }
        }
    }
}