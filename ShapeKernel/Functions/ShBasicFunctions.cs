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
        public partial class Sh
        {
            [Obsolete("Use PicoGK.Voxels.voxOffset instead")]
            public static Voxels voxOffset(Voxels vox, float fDistInMM)
                => vox.voxOffset(fDistInMM);
            
            [Obsolete("Use PicoGK.Voxels.voxSmoothen instead")]
            public static Voxels voxSmoothen(Voxels vox, float fSmoothInMM)
                => vox.voxSmoothen(fSmoothInMM);

            [Obsolete("Use PicoGK.Voxels.voxOverOffset instead")]
            public static Voxels voxOverOffset( Voxels vox, 
                                                float fInitialDistInMM, 
                                                float fFinalDistInMM)
                => vox.voxOverOffset(fInitialDistInMM, fFinalDistInMM);

            [Obsolete("Use PicoGK.Voxels.voxProjectZSlice instead")]
            public static Voxels voxExtrudeZSlice(Voxels vox, float fStartHeight, float fEndHeight)
                => vox.voxProjectZSlice(fStartHeight, fEndHeight);

            [Obsolete("Use PicoGK.Voxels.voxBoolAdd instead")]
            public static Voxels voxUnion(Voxels vox1, Voxels vox2)
                => vox1.voxBoolAdd(vox2);

            [Obsolete("Use PicoGK.Voxels.voxBoolSubtract instead")]
            public static Voxels voxSubtract(in Voxels vox1, Voxels vox2)
                => vox1.voxBoolSubtract(vox2);

            [Obsolete("Use PicoGK.Voxels.voxBoolIntersect instead")]
            public static Voxels voxIntersect(Voxels vox1, Voxels vox2)
                => vox1.voxBoolIntersect(vox2);

            [Obsolete("Use PicoGK.Voxels.voxIntersectImplicit instead")]
            public static Voxels voxIntersectImplicit(Voxels vox, IImplicit sdfObject)
                => vox.voxIntersectImplicit(sdfObject);
        }
    }
}