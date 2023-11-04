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


namespace Leap71
{
    namespace ShapeKernel
    {
        public class Bisection
        {
            public delegate float Func(float fValue);

            protected float m_fEpsilon;
            protected float m_fMinInput;
            protected float m_fMaxInput;
            protected float m_fTargetOutput;
            protected Func  m_oFunc;
            protected uint  m_nIterations;
            protected uint  m_nMaxIterations;
            protected float m_fRemainingDiff;


            /// <summary>
            /// Class to perform a bisection method to approximate a input point for the given function that returns the desired target output value.
            /// https://en.wikipedia.org/wiki/Bisection_method
            /// </summary>
            public Bisection(
                Func  oFunc,
                float fMinInput,
                float fMaxInput,
                float fTargetOutput,
                float fEpsilon      = 0.01f,
                uint nMaxIterations = 500)
            {
                m_oFunc             = oFunc;
                m_fEpsilon          = fEpsilon;
                m_fMinInput         = fMinInput;
                m_fMaxInput         = fMaxInput;
                m_fTargetOutput     = fTargetOutput;
                m_nIterations       = 0;
                m_nMaxIterations    = nMaxIterations;
            }

            protected float fGetOutputFromFunc(float fInput)
            {
                return m_oFunc(fInput) - m_fTargetOutput;
            }

            public float fFindOptimalInput()
            {
                float fMin = m_fMinInput;
                float fMax = m_fMaxInput;

                if (fGetOutputFromFunc(fMin) * fGetOutputFromFunc(fMax) >= 0)
                {
                    throw new BisectionException("No valid limits.");
                }

                float fMid              = fMin;
                m_fRemainingDiff        = fMax - fMin;
                while (m_fRemainingDiff >= m_fEpsilon)
                {
                    fMid                = 0.5f * (fMin + fMax);
                    if (fGetOutputFromFunc(fMid) == 0f)
                    {
                        break;
                    }
                    else if (fGetOutputFromFunc(fMid) * fGetOutputFromFunc(fMin) < 0)
                    {
                        fMax = fMid;
                    }
                    else
                    {
                        fMin = fMid;
                    }

                    m_fRemainingDiff = fMax - fMin;
                    m_nIterations++;

                    if (m_nIterations == m_nMaxIterations)
                    {
                        throw new BisectionException("No solution reached after max number of interations.");
                    }
                }
                return fMid;
            }

            public uint nGetIterations()
            {
                return m_nIterations;
            }

            public float fGetRemainingDiff()
            {
                return m_fRemainingDiff;
            }
        }
    }

    public class BisectionException : Exception
    {
        public BisectionException() { }

        public BisectionException(string message) : base(message)
        {

        }
    }
}