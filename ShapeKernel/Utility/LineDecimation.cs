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
		public class LineDecimation
		{
			protected List<Vector3> m_aPoints;
			protected List<Vector3> m_aDecimatedPoints;


			/// <summary>
			/// Class to resample a spline by decimating points that lie in a staight line given the error range.
			/// Start and end points remain constant.
			/// The error is measured as a points deviation distance perpendicular to the idea line (in mm).
			/// </summary>
			public LineDecimation(List<Vector3> aPoints, float fMaxError)
			{
				m_aPoints = aPoints;


				//decimate and record point indices
				int iStartIndex		= 0;
				int iEndIndex		= 0;
				List<int> aIndices	= new List<int>() { iStartIndex };
				while (iEndIndex < aPoints.Count - 1)
				{
					iEndIndex		= iGetNextIndex(iStartIndex, fMaxError);
					iStartIndex		= iEndIndex;
					aIndices.Add(iStartIndex);
				}


				//assemble decimated points
				m_aDecimatedPoints = new List<Vector3>();
				foreach (int iIndex in aIndices)
				{
					m_aDecimatedPoints.Add(m_aPoints[iIndex]);
				}
			}

			protected int iGetNextIndex(int iStartIndex, float fMaxError)
			{
				if (iStartIndex > m_aPoints.Count - 2)
				{
					int iEndIndex = m_aPoints.Count - 1;
					return iEndIndex;
				}
				else
				{
					int iCount		= 1;
					int iEndIndex	= iStartIndex + iCount;
					List<Vector3> aLinePoints = m_aPoints.GetRange(iStartIndex, 2);
					for (int iIndex = iStartIndex + 2; iIndex < m_aPoints.Count; iIndex++)
					{
						aLinePoints.Add(m_aPoints[iIndex]);
						float fError = fGetMaxError(aLinePoints);

						if (fError > fMaxError)
						{
							break;
						}
						iEndIndex = iIndex;
					}
					return iEndIndex;
				}
			}

			protected static float fGetMaxError(List<Vector3> aLinePoints)
			{
				Vector3 vecLineDir		= aLinePoints[^1] - aLinePoints[0];
				float fMaxError			= 0;
				int iCounter			= 0;
				for (int i = 1; i < aLinePoints.Count - 1; i++)
				{
					Vector3 vecPointer	= (aLinePoints[i] - aLinePoints[0]);
					float fAngle		= VecOperations.fGetAngleBetween(vecPointer, vecLineDir);
					fMaxError			= MathF.Max(fMaxError, MathF.Sin(fAngle) * vecPointer.Length());
					iCounter++;
				}
				return fMaxError;
			}

			public List<Vector3> aGetDecimatedPoints()
			{
				return m_aDecimatedPoints;
			}
		}
	}
}