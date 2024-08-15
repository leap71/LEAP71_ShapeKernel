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


using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        public interface IColorScale
        {
            public ColorFloat   clrGetColor(float fValue);
            public float        fGetMinValue();
            public float        fGetMaxValue();
        }

        public class LinearColorScale2D : IColorScale
        {
            protected float             m_fMinValue;
            protected float             m_fMaxValue;
            protected ColorFloat        m_clrMin;
            protected ColorFloat        m_clrMax;

            /// <summary>
            /// 2D color scale that interpolates linerly between min and max color on RGB basis.
            /// </summary>
            public LinearColorScale2D(  ColorFloat clrMin,
                                        ColorFloat clrMax,
                                        float fMinValue,
                                        float fMaxValue)
            {
                m_fMaxValue         = fMaxValue;
                m_fMinValue         = fMinValue;
                m_clrMin            = clrMin;
                m_clrMax            = clrMax;
            }

            public float fGetMinValue()
            {
                return m_fMinValue;
            }

            public float fGetMaxValue()
            {
                return m_fMaxValue;
            }

            protected virtual float fGetInterpolated(float fValue1, float fValue2, float fValue)
            {
                float fRatio = (fValue - m_fMinValue) / (m_fMaxValue - m_fMinValue);
                return fValue1 + fRatio * (fValue2 - fValue1);
            }

            public ColorFloat clrGetColor(float fValue)
            {
                fValue              = Uf.fLimitValue(fValue, m_fMinValue, m_fMaxValue);
                int R               = (int)(255f * fGetInterpolated(m_clrMin.R, m_clrMax.R, fValue));
                int G               = (int)(255f * fGetInterpolated(m_clrMin.G, m_clrMax.G, fValue));
                int B               = (int)(255f * fGetInterpolated(m_clrMin.B, m_clrMax.B, fValue));
                string strColor     = "#" +
                                        R.ToString("X2") +
                                        G.ToString("X2") +
                                        B.ToString("X2");
                ColorFloat clr      = new ColorFloat(strColor);
                return clr;
            }
        }

        public class SmoothColorScale2D : LinearColorScale2D
        {
            /// <summary>
            /// 2D color scale that interpolates smoothly between min and max color on RGB basis.
            /// Uses Uf.fTransFixed() interpolation internally.
            /// </summary>
            public SmoothColorScale2D(  ColorFloat clrMin,
                                        ColorFloat clrMax,
                                        float fMinValue,
                                        float fMaxValue) : base (clrMin, clrMax, fMinValue, fMaxValue) { }

            protected override float fGetInterpolated(float fValue1, float fValue2, float fValue)
            {
                float fRatio = (fValue - m_fMinValue) / (m_fMaxValue - m_fMinValue);
                return Uf.fTransFixed(fValue1, fValue2, fRatio);
            }
        }

        public class CustomColorScale2D : LinearColorScale2D
        {
            protected float m_fSmoothness;
            protected float m_fTransition;

            /// <summary>
            /// 2D color scale that interpolates smoothly between min and max color on RGB basis.
            /// Uses Uf.fTransSmooth() interpolation internally.
            /// Transition value and smoothness can be customized.
            /// </summary>
            public CustomColorScale2D(  ColorFloat clrMin,
                                        ColorFloat clrMax,
                                        float fMinValue,
                                        float fMaxValue,
                                        float fTransition,
                                        float fSmoothness) : base (clrMin, clrMax, fMinValue, fMaxValue)
            {
                m_fTransition = fTransition;
                m_fSmoothness = fSmoothness;
            }

            protected override float fGetInterpolated(float fValue1, float fValue2, float fValue)
            {
                return Uf.fTransSmooth(fValue1, fValue2, fValue, m_fTransition, m_fSmoothness);
            }
        }
    }
}