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


namespace Leap71
{
    namespace ShapeKernel
    {
        public partial class Uf
        {
            /// <summary>
            /// https://math.stackexchange.com/questions/41940/is-there-an-equation-to-describe-regular-polygons
            /// </summary>
            public enum EPolygon { HEX, QUAD, TRI };

            /// <summary>
            /// Returns the radius at a given polar angle of a regular polygon from custom inputs.
            /// The polygon will sit inside the unit circle and occasionally reach the reference radius = 1.
            /// </summary>
            public static float fGetPolygonRadius(float fPhi, uint nM)
            {
                float dPhi      = fPhi % (2f * MathF.PI / (float)nM);
                float fRadius   = MathF.Cos(MathF.PI / (float)nM) /
                                    MathF.Cos(dPhi - MathF.PI / (float)nM);
                return fRadius;
            }

            /// <summary>
            /// Returns the radius at a given polar angle of a regular polygon from custom inputs.
            /// The polygon will sit inside the unit circle and occasionally reach the reference radius = 1.
            /// The polygon will touch the unit cicle multiple times depending on the symmetry.
            /// The maximum and minimum radius depend on the polygon specified.
            /// </summary>
            public static float fGetPolygonRadius(float fPhi, EPolygon ePolygon)
            {
                if (ePolygon == EPolygon.HEX)
                {
                    return fGetPolygonRadius(fPhi, 6);
                }
                else if (ePolygon == EPolygon.QUAD)
                {
                    return fGetPolygonRadius(fPhi, 4);
                }
                else if (ePolygon == EPolygon.TRI)
                {
                    return fGetPolygonRadius(fPhi, 3);
                }
                else
                {
                    //round
                    return 1f;
                }
            }
        }
    }
}