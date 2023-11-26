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
        public partial class Uf
        {
            /// <summary>
            /// Pauses the program execution for a given number of seconds.
            /// </summary>
            public static void Wait(float fSeconds)
            {
                Thread.Sleep((int)(1000 * fSeconds));
            }

            /// <summary>
            /// Returns an intermediate point between the two specified values.
            /// The transition is determined by an open BSpline.
            /// </summary>
            /// <param name="fS"> Current position along a dimension. </param>
            /// <returns></returns>
            protected static ControlPointSpline m_oBSpline;
            public static float fTransFixed(float fValue1, float fValue2, float fS)
            {
                if (m_oBSpline == null)
                {
                    List<Vector3> aControlPoints = new List<Vector3>();
                    aControlPoints.Add(new Vector3(0, 0, 0.0f));
                    aControlPoints.Add(new Vector3(0, 0, 0.5f));
                    aControlPoints.Add(new Vector3(1, 0, 0.5f));
                    aControlPoints.Add(new Vector3(1, 0, 1.0f));
                    m_oBSpline = new ControlPointSpline(aControlPoints);
                }
                Vector3 vecPt = m_oBSpline.vecGetPointAt(fS);
                float fRatio = vecPt.X;
                float fValue = fValue1 + fRatio * (fValue2 - fValue1);
                return fValue;
            }

            /// <summary>
            /// https://www.j-raedler.de/2010/10/smooth-transition-between-functions-with-tanh/
            /// </summary>
            protected static float fGetNormalizedTangensHyperbolicus(float fS, float fTransitionS, float fSmooth)
            {
                float fValue = (float)(0.5 + 0.5 * Math.Tanh((fS - fTransitionS) / fSmooth));
                return fValue;
            }

            /// <summary>
            /// Returns an intermediate point between the two specified values.
            /// The transition is determined by a smooth tanh function.
            /// </summary>
            /// <param name="fS"> Current position along a dimension. </param>
            /// <param name="fTransitionS"> Position of transition. </param>
            /// <param name="fSmooth"> Smoothness of transition. The lower the value, the sharper the transition and the shorter the transition interval. </param>
            /// <returns></returns>
            public static float fTransSmooth(float fValue1, float fValue2, float fS, float fTransitionS, float fSmooth)
            {
                float fValue =  fValue1 * (1 - fGetNormalizedTangensHyperbolicus(fS, fTransitionS, fSmooth)) +
                                fValue2 * (fGetNormalizedTangensHyperbolicus(fS, fTransitionS, fSmooth));
                return fValue;
            }

            /// <summary>
            /// Returns an intermediate point between the two specified vectors.
            /// The transition is determined by a smooth tanh function.
            /// </summary>
            /// <param name="fS"> Current position along a dimension. </param>
            /// <param name="fTransitionS"> Position of transition. </param>
            /// <param name="fSmooth"> Smoothness of transition. The lower the value, the sharper the transition and the shorter the transition interval. </param>
            /// <returns></returns>
            public static Vector3 vecTransSmooth(Vector3 fValue1, Vector3 fValue2, float fS, float fTransitionS, float fSmooth)
            {
                Vector3 fValue = fValue1 * (1 - fGetNormalizedTangensHyperbolicus(fS, fTransitionS, fSmooth)) +
                                 fValue2 * (fGetNormalizedTangensHyperbolicus(fS, fTransitionS, fSmooth));
                return fValue;
            }

            /// <summary>
            /// Clips a value at min and max limits.
            /// </summary>
            public static float fLimitValue(float fValue, float fMin, float fMax)
            {
                fValue = MathF.Min(fValue, fMax);
                fValue = MathF.Max(fValue, fMin);
                return fValue;
            }

            /// <summary>
            /// Returns a random sample according to the specified gauss distribution.
            /// Mean±1 SD contains 68.2% of all values.
            /// Mean±2 SD contains 95.5% of all values.
            /// Mean±3 SD contains 99.7% of all values.
            /// This randomness is not reproducable.
            /// https://gist.github.com/tansey/1444070
            /// </summary>
            public static float fGetRandomGaussian(float fMean, float fStdDev)
            {
                double dX1      = 1 - m_oRandom.NextDouble();
                double dX2      = 1 - m_oRandom.NextDouble();
                double dY1      = Math.Sqrt(-2.0 * Math.Log(dX1)) * Math.Cos(2.0 * Math.PI * dX2);
                float fValue    = (float)dY1 * fStdDev + fMean;
                return fValue;
            }

            /// <summary>
            /// Returns a random sample according to the specified gauss distribution.
            /// Mean±1 SD contains 68.2% of all values.
            /// Mean±2 SD contains 95.5% of all values.
            /// Mean±3 SD contains 99.7% of all values.
            /// This randomness can be made reproducable by passing custom randomness with a seed.
            /// https://gist.github.com/tansey/1444070
            /// </summary>
            public static float fGetRandomGaussian(float fMean, float fStdDev, Random oRandom)
            {
                double dX1      = 1 - oRandom.NextDouble();
                double dX2      = 1 - oRandom.NextDouble();
                double dY1      = Math.Sqrt(-2.0 * Math.Log(dX1)) * Math.Cos(2.0 * Math.PI * dX2);
                float fValue    = (float)dY1 * fStdDev + fMean;
                return fValue;
            }

            /// <summary>
            /// Renturns a random sample according to a linear propability distribution.
            /// This randomness is not reproducable.
            /// </summary>
            protected static Random m_oRandom = new Random();
            public static float fGetRandomLinear(float fMin, float fMax)
            {
                float fValue = (float)(fMin + (fMax - fMin) * m_oRandom.NextDouble());
                return fValue;
            }

            /// <summary>
            /// Renturns a random sample according to a linear propability distribution.
            /// This randomness can be made reproducable by passing custom randomness with a seed.
            /// </summary>
            public static float fGetRandomLinear(float fMin, float fMax, Random oRandom)
            {
                float fValue = (float)(fMin + (fMax - fMin) * oRandom.NextDouble());
                return fValue;
            }

            /// <summary>
            /// Renturns a random boolean value according to a linear propability distribution.
            /// This randomness is not reproducable.
            /// </summary>
            public static bool bGetRandomBool()
            {
                float fValue = (float)m_oRandom.NextDouble();
                if (fValue > 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Renturns a random boolean value according to a linear propability distribution.
            /// This randomness can be made reproducable by passing custom randomness with a seed.
            /// </summary>
            public static bool bGetRandomBool(Random oRandom)
            {
                float fValue = (float)oRandom.NextDouble();
                if (fValue > 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Distributes points within a given 2D outer radius according to the fibonacci sequence.
            /// https://medium.com/@vagnerseibert/distributing-points-on-a-sphere-6b593cc05b42
            /// </summary>
            public static List<Vector3> aGetFibonacciCirlePoints(float fOuterRadius, uint nSamples)
            {
                List<Vector3> aPoints = new List<Vector3>();
                for (int i = 0; i < nSamples; i++)
                {
                    float fK        = i + 0.5f;
                    float fR        = MathF.Sqrt((fK) / nSamples);
                    float fPhi      = MathF.PI * (1 + MathF.Sqrt(5f)) * fK;
                    float fX        = fR * fOuterRadius * MathF.Cos(fPhi);
                    float fY        = fR * fOuterRadius * MathF.Sin(fPhi);
                    Vector3 vecPt   = new Vector3(fX, fY, 0f);
                    aPoints.Add(vecPt);
                }
                return aPoints;
            }
        }
    }
}