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
using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        public class ColorScale3D : IColorScale
        {
            protected List<Vector3>    m_aSmoothRGBList;
            protected List<float>      m_aLengths;
            protected float            m_fPathLength;
            protected float            m_fMinValue;
            protected float            m_fMaxValue;


            /// <summary>
            /// 3D color scale that interpolates smoothly between the sequence of colors in the specified spectrum.
            /// </summary>
            public ColorScale3D(    ISpectrum xSpectrum,
                                    float fMinValue,
                                    float fMaxValue)
            {
                m_fMaxValue         = fMaxValue;
                m_fMinValue         = fMinValue;
                m_aSmoothRGBList    = SplineOperations.aGetNURBSpline(xSpectrum.aGetRawRGBList(), 500);
                m_aSmoothRGBList    = SplineOperations.aGetReparametrizedSpline(m_aSmoothRGBList, (uint)500);
                m_fPathLength       = SplineOperations.fGetTotalLength(m_aSmoothRGBList);
            }

            public float fGetMinValue()
            {
                return m_fMinValue;
            }

            public float fGetMaxValue()
            {
                return m_fMaxValue;
            }

            public ColorFloat clrGetColor(float fValue)
            {
                fValue              = Uf.fLimitValue(fValue, m_fMinValue, m_fMaxValue);
                float fLengthRatio  = (fValue - m_fMinValue) / (m_fMaxValue - m_fMinValue);
                Vector3 vecRGB      = m_aSmoothRGBList[(int)(fLengthRatio * (m_aSmoothRGBList.Count - 1))];
                int R               = (int)(vecRGB.X);
                int G               = (int)(vecRGB.Y);
                int B               = (int)(vecRGB.Z);
                string strColor     = "#" +
                                        R.ToString("X2") +
                                        G.ToString("X2") +
                                        B.ToString("X2");
                ColorFloat clr      = new ColorFloat(strColor);
                return clr;
            }
        }
    }
}