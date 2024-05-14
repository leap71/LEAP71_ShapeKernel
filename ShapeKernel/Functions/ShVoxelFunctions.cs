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
        public partial class Sh
        {
            /// <summary>
            /// Returns a point on the voxelfield that is closest to the specified point.
            /// "Snapp to surface".
            /// </summary>
            public static Vector3 vecGetClosestSurfacePoint(Voxels oVoxels, Vector3 vecPt)
            {
                bool bValid = oVoxels.bClosestPointOnSurface(in vecPt, out Vector3 vecSurface);
                return vecSurface;
            }

            /// <summary>
            /// Returns a point on the voxelfield as a result from raycasting from the specified point into the specidied direction.
            /// "Project to surface".
            /// </summary>
            public static Vector3 vecGetProjectedSurfacePoint(Voxels oVoxels, Vector3 vecPt, Vector3 vecDir)
            {
                bool bValid = oVoxels.bRayCastToSurface(in vecPt, in vecDir, out Vector3 vecSurface);
                return vecSurface;
            }

            /// <summary>
            /// Returns the axis-aligned bounding box of a voxelfield.
            /// </summary>
            public static BBox3 oGetBoundingBox(Voxels oVoxels)
            {
                oVoxels.CalculateProperties(out float fCubicMM, out BBox3 oBox);
                return oBox;
            }
        }
    }
}