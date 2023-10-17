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
        public partial class Cp
        {
            public const string strBlack        = "#000000";
            public const string strRacingGreen  = "#065c35";
            public const string strGreen        = "#00b800";
            public const string strBillie       = "#02f70b";
            public const string strLemongrass   = "#b8e031";
            public const string strYellow       = "#fcd808";
            public const string strWarning      = "#fc6608";
            public const string strRed          = "#ff0000";
            public const string strRuby         = "#b0002c";
            public const string strOrchid       = "#c72483";
            public const string strPitaya       = "#fa2a88";
            public const string strBubblegum    = "#ff66ce";
            public const string strLavender     = "#c966ff";
            public const string strGray         = "#bdbdbd";
            public const string strRock         = "#6b7178";
            public const string strCrystal      = "#0cc1f7";
            public const string strFrozen       = "#6de2fc";
            public const string strBlueberry    = "#4f0dbf";
            public const string strBlue         = "#4287f5";
            public const string strToothpaste   = "#25e6c9";
           

            /// <summary>
            /// Combines a 8-digit hex string from a 6-digit hex string for the color and a transparency ratio.
            /// The transparency is specified as a ratio between 0 (fully transparent) and 1 (not transparent).
            /// </summary>
            public static string strGetColor(string strHexCode, float fTransparency)
            {
                uint nDecimalValue  = (uint)(fTransparency * 255);
                strHexCode          += nDecimalValue.ToString("X");
                return strHexCode;
            }

            /// <summary>
            /// Returns a random color that is always consistent for the specified index.
            /// Color is returned as 6-digit hex string.
            /// </summary>
            public static string strRandom(int j)
            {
                Random oRand    = new Random(j);
                string strHex   = "#";
                for (int i = 0; i < 6; i++)
                {
                    int iRandomInteger  = oRand.Next(0, 15);
                    string strChar      = iRandomInteger.ToString();
                    if (iRandomInteger == 10)
                    {
                        strChar = "A";
                    }
                    if (iRandomInteger == 11)
                    {
                        strChar = "B";
                    }
                    if (iRandomInteger == 12)
                    {
                        strChar = "C";
                    }
                    if (iRandomInteger == 13)
                    {
                        strChar = "D";
                    }
                    if (iRandomInteger == 14)
                    {
                        strChar = "E";
                    }
                    if (iRandomInteger == 15)
                    {
                        strChar = "F";
                    }
                    strHex += strChar;
                }
                return strHex;
            }

            /// <summary>
            /// Returns a random color as 6-digit hex string.
            /// </summary>
            public static string strRandom()
            {
                Random oRand    = new Random();
                string strHex   = "#";
                for (int i = 0; i < 6; i++)
                {
                    int iRandomInteger  = oRand.Next(0, 15);
                    string strChar      = iRandomInteger.ToString();
                    if (iRandomInteger == 10)
                    {
                        strChar = "A";
                    }
                    if (iRandomInteger == 11)
                    {
                        strChar = "B";
                    }
                    if (iRandomInteger == 12)
                    {
                        strChar = "C";
                    }
                    if (iRandomInteger == 13)
                    {
                        strChar = "D";
                    }
                    if (iRandomInteger == 14)
                    {
                        strChar = "E";
                    }
                    if (iRandomInteger == 15)
                    {
                        strChar = "F";
                    }
                    strHex += strChar;
                }
                return strHex;
            }
        }
    }
}