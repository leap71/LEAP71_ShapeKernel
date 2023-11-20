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
        public partial class Cp
        {
            public static readonly ColorFloat clrBlack        = new ColorFloat("#000000");
            public static readonly ColorFloat clrRacingGreen  = new ColorFloat("#065c35");
            public static readonly ColorFloat clrGreen        = new ColorFloat("#00b800");
            public static readonly ColorFloat clrBillie       = new ColorFloat("#02f70b");
            public static readonly ColorFloat clrLemongrass   = new ColorFloat("#b8e031");
            public static readonly ColorFloat clrYellow       = new ColorFloat("#fcd808");
            public static readonly ColorFloat clrWarning      = new ColorFloat("#fc6608");
            public static readonly ColorFloat clrRed          = new ColorFloat("#ff0000");
            public static readonly ColorFloat clrRuby         = new ColorFloat("#b0002c");
            public static readonly ColorFloat clrOrchid       = new ColorFloat("#c72483");
            public static readonly ColorFloat clrPitaya       = new ColorFloat("#fa2a88");
            public static readonly ColorFloat clrBubblegum    = new ColorFloat("#ff66ce");
            public static readonly ColorFloat clrLavender     = new ColorFloat("#c966ff");
            public static readonly ColorFloat clrGray         = new ColorFloat("#bdbdbd");
            public static readonly ColorFloat clrRock         = new ColorFloat("#6b7178");
            public static readonly ColorFloat clrCrystal      = new ColorFloat("#0cc1f7");
            public static readonly ColorFloat clrFrozen       = new ColorFloat("#6de2fc");
            public static readonly ColorFloat clrBlueberry    = new ColorFloat("#4f0dbf");
            public static readonly ColorFloat clrBlue         = new ColorFloat("#4287f5");
            public static readonly ColorFloat clrToothpaste   = new ColorFloat("#25e6c9");



            /// <summary>
            /// Returns a random color that is always consistent for the specified index.
            /// Color is random but reproducible.
            /// </summary>
            public static ColorFloat clrRandom(int j)
            {
                Random oRand    = new Random(j);
                return ColorFloat.clrRandom(oRand);
            }

            /// <summary>
            /// Returns a random color the is not reproducible.
            /// </summary>
            public static ColorFloat clrRandom()
            {
                return ColorFloat.clrRandom();
            }
        }
    }
}