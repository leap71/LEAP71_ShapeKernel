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
using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        using static SurfaceModulation;

        public class BaseLogoBox : BaseBox
        {
            protected SurfaceModulation m_oTopModulation;

            /// <summary>
            /// Initialises a box based on a local frame and 3 dimensions.
            /// The top surface of the box will be modulated according to the specified image.
            /// The mapping functions describes the relationship between grayscale pixel values and physical modulation value.
            /// The shape has no spine.
            /// Width correlates to local x dimension.
            /// Depth correlates to local y dimension.
            /// </summary>
            public BaseLogoBox(
                LocalFrame oFrame,
                float fLength,
                float fRefWidth,
                Image oImage,
                MappingFunc oMappingFunc) : base()
            {
                m_aFrames           = new Frames(fLength, oFrame);
                int iImageWidth     = oImage.nWidth;
                int iImageHeight    = oImage.nHeight;
                SetWidthSteps((uint)iImageWidth);
                SetDepthSteps((uint)iImageHeight);
                SetLengthSteps(5);

                float fWidth        = fRefWidth;
                float fDepth        = (float)iImageHeight / (float)iImageWidth * fRefWidth;

                m_oWidthModulation  = new LineModulation(fWidth);
                m_oDepthModulation  = new LineModulation(fDepth);
                m_bTransformed      = false;

                m_oTopModulation    = new SurfaceModulation(oImage, oMappingFunc);
            }

            public override Vector3 vecGetSurfacePoint(float fWidthRatio, float fDepthRatio, float fLengthRatio)
            {
                Vector3 vecSpinePos = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                Vector3 vecLocalX   = m_aFrames.vecGetLocalXAlongLength(fLengthRatio);
                Vector3 vecLocalY   = m_aFrames.vecGetLocalYAlongLength(fLengthRatio);
                Vector3 vecLocalZ   = m_aFrames.vecGetLocalZAlongLength(fLengthRatio);

                float fX            = 0.5f * fWidthRatio * fGetWidth(fLengthRatio);
                float fY            = 0.5f * fDepthRatio * fGetDepth(fLengthRatio);
                Vector3 vecPt       = vecSpinePos + fX * vecLocalX + fY * vecLocalY;

                //top surface modulation
                if (MathF.Abs(fLengthRatio - 1f) < 0.0003f)
                {
                    float fImageWidthRatio  = 1f - (0.5f + 0.5f * fWidthRatio);
                    float fImageHeightRatio = 1f - (0.5f + 0.5f * fDepthRatio);
                    float dZ                = m_oTopModulation.fGetModulation(fImageWidthRatio, fImageHeightRatio);
                    vecPt += dZ * vecLocalZ;
                }

                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }
        }
    }
}