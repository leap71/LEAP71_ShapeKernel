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
        public class CylindricalControlSpline : ISpline
        {
            protected List<Vector3> m_aControlPoints;
            public enum EDirection { RADIAL, TANGENTIAL, Z };

            public CylindricalControlSpline(Vector3 vecStart)
            {
                m_aControlPoints = new List<Vector3>();
                m_aControlPoints.Add(vecStart);
            }

            public void AddRelativeStep(EDirection eDir, float fStepLength)
            {
                Vector3 vecLastPos              = m_aControlPoints[^1];
                if (eDir == EDirection.Z)
                {
                    Vector3 vecZDir             = Vector3.UnitZ;
                    Vector3 vecNewPos           = vecLastPos + fStepLength * vecZDir;
                    m_aControlPoints.Add(vecNewPos);
                }
                else if (eDir == EDirection.RADIAL)
                {
                    Vector3 vecRadialDir        = VecOperations.vecGetPlanarDir(vecLastPos);
                    Vector3 vecNewPos           = vecLastPos + fStepLength * vecRadialDir;
                    m_aControlPoints.Add(vecNewPos);
                }
                else
                {
                    Vector3 vecRadialDir        = VecOperations.vecGetPlanarDir(vecLastPos);
                    Vector3 vecTangentialDir    = Vector3.Cross(Vector3.UnitZ, vecRadialDir);
                    Vector3 vecNewPos           = vecLastPos + fStepLength * vecTangentialDir;
                    m_aControlPoints.Add(vecNewPos);
                }
            }

            public void AddAbsoluteStep(EDirection eDir, float fNewValue)
            {
                Vector3 vecLastPos      = m_aControlPoints[^1];
                if (eDir == EDirection.Z)
                {
                    Vector3 vecNewPos   = VecOperations.vecSetZ(vecLastPos, fNewValue);
                    m_aControlPoints.Add(vecNewPos);
                }
                else if (eDir == EDirection.RADIAL)
                {
                    Vector3 vecNewPos   = VecOperations.vecSetRadius(vecLastPos, fNewValue);
                    m_aControlPoints.Add(vecNewPos);
                }
            }

            public List<Vector3> aGetPoints(uint nSamples = 500)
            {
                ControlPointSpline oBSpline = new ControlPointSpline(m_aControlPoints);
                return oBSpline.aGetPoints(nSamples);
            }
        }
    }
}