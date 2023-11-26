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
        public class ImplicitGyroid : IImplicit
		{
            protected float         m_fFrequencyScale;
			protected float		    m_fWallThickness;

            /// <summary>
            /// Helper class for an implicit gyroid pattern.
            /// </summary>
            public ImplicitGyroid(float fUnitSize, float fWallThickness)
			{
                m_fFrequencyScale   = (2f * MathF.PI) / fUnitSize;
                m_fWallThickness    = fWallThickness;
            }

			public float fSignedDistance(in Vector3 vecPt)
			{
                double dX = vecPt.X;
                double dY = vecPt.Y;
                double dZ = vecPt.Z;

                //calculate the gyroid surface equation
                double fDist =   Math.Sin(m_fFrequencyScale * dX) * Math.Cos(m_fFrequencyScale * dY) +
                                 Math.Sin(m_fFrequencyScale * dY) * Math.Cos(m_fFrequencyScale * dZ) +
                                 Math.Sin(m_fFrequencyScale * dZ) * Math.Cos(m_fFrequencyScale * dX);

                //apply thickness to the gyroid surface
                return (float)(Math.Abs(fDist) - 0.5f * m_fWallThickness);
            }
		}

        public class ImplicitSphere : IImplicit
        {
			protected Vector3	m_vecCentre;
			protected float		m_fRadius;

            /// <summary>
            /// Helper class for an implicit sphere.
            /// </summary>
            public ImplicitSphere(Vector3 vecCentre, float fRadius)
            {
				m_vecCentre = vecCentre;
				m_fRadius	= fRadius;
            }

            public float fSignedDistance(in Vector3 vecPt)
            {
                return (vecPt - m_vecCentre).Length() - m_fRadius;
            }
        }
    }
}