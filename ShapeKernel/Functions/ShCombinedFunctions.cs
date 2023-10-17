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


using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        public partial class Sh
        {
            public static Voxels voxShell(Voxels oVoxels, float fNegOffset, float fPosOffset, float fSmooth = 0f)
            {
                if (fNegOffset > fPosOffset)
                {
                    float fTemp     = fNegOffset;
                    fNegOffset      = fPosOffset;
                    fPosOffset      = fTemp;
                }
                Voxels voxInner     = voxOffset(oVoxels, fNegOffset);
                if (fSmooth > 0)
                {
                    voxInner        = voxSmoothen(voxInner, fSmooth);
                }
                Voxels voxOuter     = voxOffset(oVoxels, fPosOffset);
                Voxels voxGResult   = voxSubtract(voxOuter, voxInner);
                return voxGResult;
            }

            public static Voxels voxUnion(List<Voxels> aVoxelList)
            {
                Voxels voxAccum = aVoxelList[0];
                for (int i = 1; i < aVoxelList.Count; i++)
                {
                    voxAccum = voxUnion(voxAccum, aVoxelList[i]);
                }
                return voxAccum;
            }
        }
    }
}