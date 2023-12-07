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


namespace Leap71
{
    namespace ShapeKernel
    {
        public partial class Uf
        {
            /// <summary>
            /// http://paulbourke.net/geometry/supershape/
            /// https://en.wikipedia.org/wiki/Superformula
            /// </summary>
            public enum ESuperShape { ROUND, HEX, QUAD, TRI };

            /// <summary>
            /// Returns the radius at a given polar angle of a supershape from custom inputs.
            /// The supershape has a reference radius = 1.
            /// </summary>
            public static float fGetSuperShapeRadius(float fPhi, float fM, float fN1, float fN2, float fN3)
            {
                double dA       = Math.Pow(Math.Abs(Math.Cos(0.25f * fM * fPhi)), fN2);
                double dB       = Math.Pow(Math.Abs(Math.Sin(0.25f * fM * fPhi)), fN3);
                float fRadius   = (float)Math.Pow(dA + dB, -(1 / fN1));
                return fRadius;
            }

            /// <summary>
            /// Returns the radius at a given polar angle of a supershape from preset inputs.
            /// The supershape has a reference radius = 1.
            /// </summary>
            public static float fGetSuperShapeRadius(float fPhi, ESuperShape eSuperShape)
            {
                if (eSuperShape == ESuperShape.HEX)
                {
                    return fGetSuperShapeRadius(fPhi, 6f, 2f, 1f, 1f);
                }
                else if (eSuperShape == ESuperShape.QUAD)
                {
                    return fGetSuperShapeRadius(fPhi, 4f, 20f, 15f, 15f);
                }
                else if (eSuperShape == ESuperShape.TRI)
                {
                    return fGetSuperShapeRadius(fPhi, 3f, 10f, 4f, 4f);
                }
                else
                {
                    //round
                    return fGetSuperShapeRadius(fPhi, 2f, 2f, 2f, 2f);
                }
            }
        }
    }
}