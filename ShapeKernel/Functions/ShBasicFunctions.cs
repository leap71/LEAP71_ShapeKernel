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
            public static Voxels voxOffset(Voxels oVoxels, float fDistInMM)
            {
                Voxels oNewVoxels = new Voxels(oVoxels);
                oNewVoxels.Offset(fDistInMM);
                return oNewVoxels;
            }

            public static Voxels voxSmoothen(Voxels oVoxels, float fSmoothInMM)
            {
                Voxels oNewVoxels = new Voxels(oVoxels);
                oNewVoxels.TripleOffset(fSmoothInMM);
                return oNewVoxels;
            }

            public static Voxels voxOverOffset(Voxels oVoxels, float fInitialDistInMM, float fFinalDistInMM)
            {
                Voxels oNewVoxels = new Voxels(oVoxels);
                oNewVoxels.DoubleOffset(fInitialDistInMM, -(fInitialDistInMM - fFinalDistInMM));
                return oNewVoxels;
            }

            public static Voxels voxExtrudeZSlice(Voxels oVoxels, float fStartHeight, float fEndHeight)
            {
                Voxels oNewVoxels = new Voxels(oVoxels);
                oNewVoxels.ProjectZSlice(fStartHeight, fEndHeight);
                return oNewVoxels;
            }

            public static Voxels voxUnion(Voxels oVoxels1, Voxels oVoxels2)
            {
                Voxels oNewVoxels = new Voxels(oVoxels1);
                oNewVoxels.BoolAdd(oVoxels2);
                return oNewVoxels;
            }

            public static Voxels voxSubtract(in Voxels oVoxels1, Voxels oVoxels2)
            {
                Voxels oNewVoxels = new Voxels(oVoxels1);
                oNewVoxels.BoolSubtract(oVoxels2);
                return oNewVoxels;
            }

            public static Voxels voxIntersect(Voxels oVoxels1, Voxels oVoxels2)
            {
                Voxels oNewVoxels = new Voxels(oVoxels1);
                oNewVoxels.BoolIntersect(oVoxels2);
                return oNewVoxels;
            }

            public static Voxels voxIntersectImplicit(Voxels oVoxels, IImplicit sdfObject)
            {
                Voxels oNewVoxels = new Voxels(oVoxels);
                oNewVoxels.IntersectImplicit(sdfObject);
                return oNewVoxels;
            }
        }
    }
}