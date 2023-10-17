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


namespace Leap71
{
    namespace ShapeKernel
    {
        public static class GridOperations
        {
            /// <summary>
            /// Swithes rows (x) and columns (y) within the grid.
            /// </summary>
            public static List<List<Vector3>> aGetInverseGrid(List<List<Vector3>> aGrid)
            {
                List<List<Vector3>> aInverseGrid = new List<List<Vector3>>();
                for (int i = 0; i < aGrid[0].Count; i++)
                {
                    List<Vector3> aPoints = new List<Vector3>();
                    for (int j = 0; j < aGrid.Count; j++)
                    {
                        aPoints.Add(aGrid[j][i]);
                    }
                    aInverseGrid.Add(aPoints);
                }
                return aInverseGrid;
            }

            /// <summary>
            /// Addes a row (in x) at the end.
            /// </summary>
            public static List<List<Vector3>> aAddListInX(List<List<Vector3>> aGrid, List<Vector3> aPoints)
            {
                aGrid.Add(aPoints);
                return aGrid;
            }

            /// <summary>
            /// Removes a row (in x) at the specified index.
            /// </summary>
            public static List<List<Vector3>> aRemoveListInX(List<List<Vector3>> aGrid, uint nIndex)
            {
                aGrid.RemoveAt((int)nIndex);
                return aGrid;
            }

            /// <summary>
            /// Returns the row (in x) at the specified index.
            /// </summary>
            public static List<Vector3> aGetListInX(List<List<Vector3>> aGrid, uint nIndex)
            {
                return aGrid[(int)nIndex];
            }

            /// <summary>
            /// Addes a column (in y) at the end.
            /// </summary>
            public static List<List<Vector3>> aAddListInY(List<List<Vector3>> aGrid, List<Vector3> aPoints)
            {
                List<List<Vector3>> aInverseGrid = aGetInverseGrid(aGrid);
                aInverseGrid = aAddListInX(aInverseGrid, aPoints);
                List<List<Vector3>> aNewGrid = aGetInverseGrid(aInverseGrid);
                return aNewGrid;
            }

            /// <summary>
            /// Removes a column (in y) at the specified index.
            /// </summary>
            public static List<List<Vector3>> aRemoveListInY(List<List<Vector3>> aGrid, uint nIndex)
            {
                List<List<Vector3>> aInverseGrid = aGetInverseGrid(aGrid);
                aInverseGrid.RemoveAt((int)nIndex);
                List<List<Vector3>> aNewGrid = aGetInverseGrid(aInverseGrid);
                return aNewGrid;
            }

            /// <summary>
            /// Returns the column (in y) at the specified index.
            /// </summary>
            public static List<Vector3> aGetListInY(List<List<Vector3>> aGrid, uint nIndex)
            {
                List<List<Vector3>> aInverseGrid = aGetInverseGrid(aGrid);
                return aInverseGrid[(int)nIndex];
            }
        }
    }
}