//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2024 by LEAP 71
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
        //Modulation in 1D
        public class LineModulation
        {
            public delegate float       RatioFunc(float fRatio);
            public enum                 ECoord { X, Y, Z };
            protected ECoord            m_eValues;
            protected ECoord            m_eAxis;

            public float                m_fConstValue;      // has a constant return value
            protected RatioFunc         m_oFunc;            // has a function to be evaluated
            protected List<Vector3>     m_aDiscretePoints;  // has a discrete point list to be interpolated
            protected List<float>       m_aXValues;
            protected List<float>       m_aYValues;


            /// <summary>
            /// Line modulation built around a constant value.
            /// </summary>
            public LineModulation(float fConstValue)
            {
                m_fConstValue = fConstValue;
                m_oFunc       = fConstLineDummyFunc;

                // unused
                m_aDiscretePoints = new();
                m_aXValues = new();
                m_aYValues = new();
            }

            /// <summary>
            /// Line modulation based on a (continuous) 1D function.
            /// </summary>
            public LineModulation(RatioFunc oModuationFunc)
            {
                m_oFunc = oModuationFunc;

                // unused
                m_aDiscretePoints = new();
                m_aXValues = new();
                m_aYValues = new();
            }

            /// <summary>
            /// Line modulation based on a discrete distribution.
            /// </summary>
            public LineModulation(List<Vector3> aDiscretePoints, ECoord eValues, ECoord eAxis)
            {
                m_eValues           = eValues;
                m_eAxis             = eAxis;
                m_aDiscretePoints   = aDiscretePoints;

                m_aXValues          = new ();
                m_aYValues          = new();
                float fXValue       = fGetCoordinate(m_aDiscretePoints[0], eAxis);
                float fYValue       = fGetCoordinate(m_aDiscretePoints[0], eValues);
                m_aXValues.Add(fXValue);
                m_aYValues.Add(fYValue);

                for (int i = 1; i < m_aDiscretePoints.Count; i++)
                {
                    Vector3 vecPt   = m_aDiscretePoints[i];
                    fXValue         = fGetCoordinate(vecPt, eAxis);
                    fYValue         = fGetCoordinate(vecPt, eValues);

                    if (fXValue > m_aXValues[^1])
                    {
                        m_aXValues.Add(fXValue);
                        m_aYValues.Add(fYValue);
                    }
                }

                // fix start and end
                if (m_aXValues[0] > 0.0f)
                {
                    float fFirstY   = m_aYValues[0];
                     m_aXValues.Insert(0, 0.0f);
                    m_aYValues.Insert(0, fFirstY);
                }
                if (m_aXValues[^1] < 1.0f)
                {
                    float fLastY    = m_aYValues[^1];
                    m_aXValues.Add(1.0f);
                    m_aYValues.Add(fLastY);
                }
                
                // interpolation function
                m_oFunc = oPointsDummyFunc;
            }

            protected float fGetCoordinate(Vector3 vecPt, ECoord eCoord)
            {
                float fCoordinate = vecPt.X;
                if (eCoord == ECoord.Y)
                {
                    fCoordinate = vecPt.Y;
                }
                else if (eCoord == ECoord.Z)
                {
                    fCoordinate = vecPt.Z;
                }
                return fCoordinate;
            }

            protected float fConstLineDummyFunc(float fRatio)
            {
                return m_fConstValue;
            }

            protected float oPointsDummyFunc(float fX)
            {
                fX = float.Clamp(fX, 0, 1);

                int idx = m_aXValues.BinarySearch(fX);
                if (idx < 0)
                {
                    idx = ~idx;
                }

                int iUpper      = Math.Min(idx, m_aXValues.Count - 1);
                int iLower      = Math.Max(iUpper - 1, 0);
                float fLowerX   = m_aXValues[iLower];
                float fUpperX   = m_aXValues[iUpper];

                if (fUpperX == fLowerX)
                {
                    return m_aYValues[iLower];
                }

                float fLR       = (fX - fLowerX) / (fUpperX - fLowerX);
                float fY        = m_aYValues[iLower] + fLR * (m_aYValues[iUpper] - m_aYValues[iLower]);
                return fY;
            }

            /// <summary>
            /// Queries the value of the line modulation at a given ratio.
            /// The ratio should be between 0 and 1.
            /// </summary>
            public float fGetModulation(float fRatio)
            {
                return m_oFunc(fRatio);
            }

            /// <summary>
            /// Multiplies a modulation with a factor.
            /// </summary>
            public static LineModulation operator *(float fFactor, LineModulation oMod)
            {
                return ModulationMultiplication.oGetScaledModulation(fFactor, oMod);
            }

            /// <summary>
            /// Adds one modulation to another.
            /// </summary>
            public static LineModulation operator +(LineModulation oMod1, LineModulation oMod2)
            {
                return ModulationAddition.oGetSumOfModulations(oMod1, oMod2);
            }

            /// <summary>
            /// Subtract one modulation from another.
            /// </summary>
            public static LineModulation operator -(LineModulation oMod1, LineModulation oMod2)
            {
                return ModulationAddition.oGetDifferenceOfModulations(oMod1, oMod2);
            }

            class ModulationAddition
            {
                LineModulation m_oMod1;
                LineModulation m_oMod2;

                ModulationAddition( LineModulation oMod1,
                                    LineModulation oMod2)
                {
                    m_oMod1 = oMod1;
                    m_oMod2 = oMod2;
                }

                public static LineModulation oGetSumOfModulations(  LineModulation oMod1,
                                                                    LineModulation oMod2)
                {
                    ModulationAddition oUtil = new (oMod1, oMod2);
                    return oUtil.oGetSumOfModulations();
                }

                public static LineModulation oGetDifferenceOfModulations(   LineModulation oMod1,
                                                                            LineModulation oMod2)
                {
                    ModulationAddition oUtil = new(oMod1, oMod2);
                    return oUtil.oGetDifferenceOfModulations();
                }

                LineModulation oGetSumOfModulations()
                {
                    return new LineModulation(fGetAddedModulation);
                }

                LineModulation oGetDifferenceOfModulations()
                {
                    return new LineModulation(fGetSubtractedModulation);
                }

                float fGetAddedModulation(float fLengthRatio)
                {
                    return m_oMod1.fGetModulation(fLengthRatio) + m_oMod2.fGetModulation(fLengthRatio);
                }

                float fGetSubtractedModulation(float fLengthRatio)
                {
                    return m_oMod1.fGetModulation(fLengthRatio) - m_oMod2.fGetModulation(fLengthRatio);
                }
            }

            class ModulationMultiplication
            {
                float             m_fFactor;
                LineModulation    m_oMod;
                

                ModulationMultiplication(   float fFactor,
                                            LineModulation oMod)
                {
                    m_fFactor   = fFactor;
                    m_oMod      = oMod;
                }

                public static LineModulation oGetScaledModulation(  float fFactor,
                                                                    LineModulation oMod)
                {
                    ModulationMultiplication oUtil = new (fFactor, oMod);
                    return oUtil.oGetMultipliedModulation();
                }

                LineModulation oGetMultipliedModulation()
                {
                    return new LineModulation(fGetScaledModulation);
                }

                float fGetScaledModulation(float fLengthRatio)
                {
                    return m_fFactor * m_oMod.fGetModulation(fLengthRatio);
                }
            }
        }

        public class Distribution
        {
            public float          m_fTotalLength;
            public LineModulation m_oModulation;

            /// <summary>
            /// This class bundles a line modulation (which is normalized) with a physical length.
            /// It can be used to store data distributions consistently.
            /// </summary>
            public Distribution(float           fTotalLength,
                                LineModulation  oModulation)
            {
                m_fTotalLength = fTotalLength;
                m_oModulation  = oModulation;
            }
        }

        public class GenericContour : Distribution
        {
            /// <summary>
            /// This class bundles a line modulation (which is normalized) with a physical length.
            /// It is a special case of a distribution that describes countours of rotationally symmetric objects.
            /// </summary>
            public GenericContour(  float          fTotalLength,
                                    LineModulation oModulation) : base(fTotalLength, oModulation) { }
        }
    }
}