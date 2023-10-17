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
        public interface ISpline
        {
            public abstract List<Vector3> aGetPoints(uint nSamples = 500);
        }

        public class TangentialControlSpline : ISpline
        {
            protected ControlPointSpline m_oBSpline;

            public TangentialControlSpline(
                Vector3 vecStart,
                Vector3 vecEnd,
                Vector3 vecStartDir,
                Vector3 vecEndDir,
                float   fStartTangentStrength = -1,
                float   fEndTangentStrenth = -1)
            {
                if (fStartTangentStrength == -1)
                {
                    fStartTangentStrength = 0.3f * (vecStart - vecEnd).Length();
                }
                if (fEndTangentStrenth == -1)
                {
                    fEndTangentStrenth = 0.3f * (vecStart - vecEnd).Length();
                }

                Vector3 vecPt1                  = vecStart;
                Vector3 vecPt2                  = vecStart   + fStartTangentStrength * vecStartDir.Normalize();
                Vector3 vecPt3                  = vecEnd     - fEndTangentStrenth    * vecEndDir.Normalize();
                Vector3 vecPt4                  = vecEnd;
                List<Vector3> aControlPoints    = new List<Vector3>() { vecPt1, vecPt2, vecPt3, vecPt4 };
                m_oBSpline                      = new ControlPointSpline(aControlPoints);
            }

            public List<Vector3> aGetPoints(uint nSamples = 500)
            {
                return m_oBSpline.aGetPoints(nSamples);
            }
        }
    }
}