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
		public class BaseCone : BaseShape
        {
			protected BaseCylinder	m_oCyl;
			protected float			m_fStartRadius;
			protected float			m_fEndRadius;

			/// <summary>
			/// Simple cone shape with a linear radius distribution between start and end radius.
			/// Derived from BaseCylinder.
			/// </summary>
			public BaseCone(LocalFrame oFrame, float fLength, float fStartRadius, float fEndRadius)
			{
				m_fStartRadius	= fStartRadius;
				m_fEndRadius	= fEndRadius;
				m_oCyl			= new BaseCylinder(oFrame, fLength);
				m_oCyl.SetRadius(new SurfaceModulation(fGetLinearRadius));
			}

			protected float fGetLinearRadius(float fPhi, float fLengthRatio)
			{
				fLengthRatio = Uf.fLimitValue(fLengthRatio, 0f, 1f);
				return m_fStartRadius + fLengthRatio * (m_fEndRadius - m_fStartRadius);
			}

			public override Voxels voxConstruct()
			{
				m_oCyl.SetTransformation(m_fnTrafo);
                return m_oCyl.voxConstruct();
            }

			public BaseCylinder oGetBaseCylinder()
			{
				return m_oCyl;
			}
		}
	}
}