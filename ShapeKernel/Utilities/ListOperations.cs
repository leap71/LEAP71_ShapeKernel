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
        public class ListOperations
        {
            /// <summary>
            /// Samples the specified list with an increment.
            /// Note that the count of the list will change!
            /// Start and end value remain constant.
            /// </summary>
            public static List<float> aOverSampleList(List<float> aList, int iSamplesPerStep)
            {
                List<float> aFinalList = new List<float>();
                for (int i = 1; i < aList.Count; i++)
                {
                    for (int j = 0; j < iSamplesPerStep; j++)
                    {
                        float fValue = aList[i - 1] + (float)j / (float)(iSamplesPerStep) * (aList[i] - aList[i - 1]);
                        aFinalList.Add(fValue);
                    }
                }
                aFinalList.Add(aList[^1]);
                return aFinalList;
            }

            /// <summary>
            /// Samples the specified list with an increment.
            /// Note that the count of the list will change!
            /// Start and end value remain constant.
            /// </summary>
            public static List<float> aSubsampleList(List<float> aList, int iSampleSize)
            {
                List<float> aFinalList = new List<float>();
                int i = 0;
                for (i = 0; i < aList.Count; i += iSampleSize)
                {
                    aFinalList.Add(aList[i]);
                }
                aFinalList.Add(aList[^1]);
                return aFinalList;
            }

            /// <summary>
            /// Returns the index of the maximum value within the list.
            /// </summary>
            public static int iGetIndexOfMaxValue(List<float> aList)
            {
                float fMaxValue = float.MinValue;
                int iIndex = -1;

                for (int i = 0; i < aList.Count; i++)
                {
                    if (aList[i] > fMaxValue)
                    {
                        fMaxValue = aList[i];
                        iIndex = i;
                    }
                }
                return iIndex;
            }

            /// <summary>
            /// Returns the index of the minimum value within the list.
            /// </summary>
            public static int iGetIndexOfMinValue(List<float> aList)
            {
                float fMinValue = float.MaxValue;
                int iIndex = -1;

                for (int i = 0; i < aList.Count; i++)
                {
                    if (aList[i] < fMinValue)
                    {
                        fMinValue = aList[i];
                        iIndex = i;
                    }
                }
                return iIndex;
            }
        }
    }
}