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
        //Modulation in 2D
        public class SurfaceModulation
        {
            public delegate float       MappingFunc(float fGrayValue);
            public delegate float       RatioFunc(float fPhi, float fLengthRatio);
            public enum                 EInput { FUNC, IMAGE };
            public enum                 ELine { FIRST, SECOND };

            protected Image             m_oImage;
            protected RatioFunc         m_oFunc;
            protected MappingFunc       m_oMappingFunc;
            protected EInput            m_eInput;
            protected ELine             m_eLine;
            protected LineModulation    m_oLineModulation;
            protected float             m_fConstValue;


            /// <summary>
            /// Surface modulation based on a discrete distribution.
            /// </summary>
            public SurfaceModulation(float fConstValue)
            {
                m_fConstValue           = fConstValue;
                m_oFunc                 = fConstSurfaceDummyFunc;
                m_eInput                = EInput.FUNC;
            }

            /// <summary>
            /// Surface modulation based on a (continuous) 2D function.
            /// </summary>
            public SurfaceModulation(RatioFunc oFunc)
            {
                m_oFunc                 = oFunc;
                m_eInput                = EInput.FUNC;
            }

            /// <summary>
            /// Surface modulation derived from a line modulation.
            /// The enum dictates for which dimension the 1D line modulation applies.
            /// </summary>
            public SurfaceModulation(LineModulation oLineModulation, ELine eLine = ELine.SECOND)
            {
                m_oLineModulation       = oLineModulation;
                m_eLine                 = eLine;
                m_oFunc                 = fRatioSurfaceDummyFunc;
                m_eInput                = EInput.FUNC;
            }

            /// <summary>
            /// Surface modulation derived from an image.
            /// The mapping function indicates how an image grayscale value shall be translated into physical modulation.
            /// </summary>
            public SurfaceModulation(Image oImage, MappingFunc oMappingFunc)
            {
                m_oImage                = oImage;
                m_oMappingFunc          = oMappingFunc;
                m_eInput                = EInput.IMAGE;
            }

            protected float fConstSurfaceDummyFunc(float fPhi, float fLengthRatio)
            {
                return m_fConstValue;
            }

            protected float fRatioSurfaceDummyFunc(float fPhi, float fLengthRatio)
            {
                if (m_eLine == ELine.FIRST)
                {
                    return m_oLineModulation.fGetModulation(fPhi);
                }
                else
                {
                    return m_oLineModulation.fGetModulation(fLengthRatio);
                }
            }

            public EInput oGetInputType()
            {
                return m_eInput;
            }

            /// <summary>
            /// Queries the value of the surface modulation at given ratios.
            /// The ratios should be between 0 and 1.
            /// </summary>
            public float fGetModulation(float fPhi, float fLengthRatio)
            {
                if (m_eInput == EInput.FUNC)
                {
                    return m_oFunc(fPhi, fLengthRatio);
                }
                else if (m_eInput == EInput.IMAGE)
                {
                    int nXRange         = m_oImage.nWidth - 1;
                    int nYRange         = m_oImage.nHeight - 1;
                    int x               = (int)float.Round(nXRange - fPhi * nXRange);
                    int y               = (int)float.Round(fLengthRatio * nYRange);
                    float fGrayValue    = m_oImage.fValue(x, y);
                    float fValue        = m_oMappingFunc(fGrayValue);
                    return fValue;
                }
                else
                {
                    throw new Exception("Invalid Surface Modulation type.");
                }
            }

            /// <summary>
            /// Adds one modulation to another.
            /// </summary>
            public static SurfaceModulation operator +(SurfaceModulation oMod1, SurfaceModulation oMod2)
            {
                return ModulationUtil.oGetSumOfModulations(oMod1, oMod2);
            }

            /// <summary>
            /// Subtract one modulation from another.
            /// </summary>
            public static SurfaceModulation operator -(SurfaceModulation oMod1, SurfaceModulation oMod2)
            {
                return ModulationUtil.oGetDifferenceOfModulations(oMod1, oMod2);
            }

            protected class ModulationUtil
            {
                protected SurfaceModulation m_oMod1;
                protected SurfaceModulation m_oMod2;

                protected ModulationUtil(   SurfaceModulation oMod1,
                                            SurfaceModulation oMod2)
                {
                    m_oMod1 = oMod1;
                    m_oMod2 = oMod2;
                }

                public static SurfaceModulation oGetSumOfModulations(  SurfaceModulation oMod1,
                                                                    SurfaceModulation oMod2)
                {
                    ModulationUtil oUtil = new(oMod1, oMod2);
                    return oUtil.oGetSumOfModulations();
                }

                public static SurfaceModulation oGetDifferenceOfModulations(   SurfaceModulation oMod1,
                                                                            SurfaceModulation oMod2)
                {
                    ModulationUtil oUtil = new(oMod1, oMod2);
                    return oUtil.oGetDifferenceOfModulations();
                }

                protected SurfaceModulation oGetSumOfModulations()
                {
                    return new SurfaceModulation(fGetAddedModulation);
                }

                protected SurfaceModulation oGetDifferenceOfModulations()
                {
                    return new SurfaceModulation(fGetSubtractedModulation);
                }

                protected float fGetAddedModulation(float fPhi, float fLengthRatio)
                {
                    return m_oMod1.fGetModulation(fPhi, fLengthRatio) + m_oMod2.fGetModulation(fPhi, fLengthRatio);
                }

                protected float fGetSubtractedModulation(float fPhi, float fLengthRatio)
                {
                    return m_oMod1.fGetModulation(fPhi, fLengthRatio) - m_oMod2.fGetModulation(fPhi, fLengthRatio);
                }
            }
        }
    }
}