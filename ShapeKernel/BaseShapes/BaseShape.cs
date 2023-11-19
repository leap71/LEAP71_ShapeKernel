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
        public interface ISurfaceBaseShape
        {
            public Vector3 vecGetSurfacePoint(float fRatio1, float fRatio2, float fRatio3);
        }

        public interface ISpineBaseShape
        {
            public Vector3 vecGetSpinePoint(float fRatio1);
        }

        public interface IMeshBaseShape
        {
            public Mesh mshConstruct();
        }

        public interface ILatticeBaseShape
        {
            public Lattice latConstruct();
        }

        public abstract class BaseShape
        {
            public delegate Vector3     TrafoFunc(Vector3 vecPt);
            protected TrafoFunc         m_oTrafo;
            protected bool              m_bTransformed;

            public BaseShape() { }

            //settings
            /// <summary>
            /// Set a transformation to the shape that will be applies point-wise during construction.
            /// </summary>
            public void SetTransformation(TrafoFunc oTrafo)
            {
                m_oTrafo        = oTrafo;
                m_bTransformed  = true;
            }

            //construction
            public abstract Voxels voxConstruct();
        }
    }
}